
[Transient]
public class TransactionInterceptor: MarshalByRefObject, IMethodInterceptor, IOnBehalfAware {

    private readonly IKernel kernel;
    private readonly ILogger logger;
    private readonly TransactionMetaInfoStore infoStore;
    private readonly ISynchronizationManager synchronizationManager;
    private TransactionMetaInfo metaInfo;

    public TransactionInterceptor(IKernel kernel, ILogger logger, ISynchronizationManager synchronizationManager, TransactionMetaInfoStore infoStore) {
        this.kernel = kernel;
        this.logger = logger;
        this.synchronizationManager = synchronizationManager;
        this.infoStore = infoStore;
    }

    #region MarshalByRefObject

    public override object InitializeLifetimeService() {
        return null;
    }

    #endregion

    public object Intercept(IMethodInvocation invocation, params object[] args) {
        // get method info of invocation target
        MethodInfo methodInfo = invocation.MethodInvocationTarget;
        if (metaInfo == null || !metaInfo.Contains(methodInfo))
            return invocation.Proceed(args);
        else {
            // get transaction attribute
            TransactionAttribute transactionAttribute = metaInfo.GetTransactionAttributeFor(methodInfo);
            // get transaction mode
            TransactionMode transactionMode = transactionAttribute.TransactionMode;
            // set default transaction mode if needed
            if (transactionMode == TransactionMode.Unspecified)
                transactionMode = TransactionMode.Required;
            // get reference to current transaction if any
            Transaction currentTransaction = Transaction.Current;
            // return value
            object result = null;
            // log debug information
            if (logger.IsDebugEnabled)
                logger.Debug("Processing method [{0}] invocation with TransactionMode [{1}]", methodInfo.Name, transactionMode);
            // process method invocation
            switch (transactionMode) {
                case TransactionMode.NotSupported:
                    if (currentTransaction != null) {
                        string message = String.Format("Method [{0}] cannot be invoked while a transaction is in progress", methodInfo.Name);
                        logger.Error(message);
                        throw new TransactionException(message);
                    }
                    result = invocation.Proceed(args);
                    break;
                case TransactionMode.Required:
                case TransactionMode.RequiresNew:
                    // transaction options
                    TransactionOptions transactionOptions = new TransactionOptions();
                    transactionOptions.IsolationLevel = transactionAttribute.IsolationLevel;
                    // transaction scope options
                    TransactionScopeOption transactionScopeOption = (transactionMode == TransactionMode.Required) ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew;
                    if (transactionMode == TransactionMode.Required && Transaction.Current != null)
                        result = invocation.Proceed(args);
                    else {
                        try {
                            // create new synchronization block
                            synchronizationManager.CreateSynchronizationBlock();
                            // create new transaction scope
                            using (TransactionScope transactionScope = new TransactionScope(transactionScopeOption, transactionOptions)) {
                                if (logger.IsDebugEnabled)
                                    logger.Debug("Transaction [{0}] started in thread [{1}]", Transaction.Current.GetHashCode(), Thread.CurrentThread.GetHashCode());
                                // invoke target method
                                result = invocation.Proceed(args);
                                if (logger.IsDebugEnabled)
                                    logger.Debug("Commiting transaction [{0}] in thread [{1}]", Transaction.Current.GetHashCode(), Thread.CurrentThread.GetHashCode());
                                // set transaction as complete
                                transactionScope.Complete();
                            }
                            // perform synchronizations
                            synchronizationManager.PerformSynchronizations(true);
                        }
                        catch (TransactionException) {
                            throw;
                        }
                        catch (Exception ex) {
                            logger.Error("Error detected in transactional block", ex);
                            try {
                                // perform synchronizations
                                synchronizationManager.PerformSynchronizations(false);
                            }
                            catch (Exception ex1) {
                                logger.Error("Error performing synchronization", ex1);
                            }
                            throw;
                        }
                    }
                    break;
                case TransactionMode.Supported:
                    result = invocation.Proceed(args);
                    break;
            }
            return result;
        }
    }

    public void SetInterceptedComponentModel(Castle.Model.ComponentModel target) {
        metaInfo = infoStore.GetMetaFor(target.Implementation);
    }
}
