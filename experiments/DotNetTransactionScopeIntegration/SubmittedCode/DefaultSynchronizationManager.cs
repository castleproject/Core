

internal class DefaultSynchronizationManager: ISynchronizationManager {

    [ThreadStatic]
    private readonly Stack<IList<ISynchronization>> synchronizations = new Stack<IList<ISynchronization>>();
    private readonly ILogger logger;

    public DefaultSynchronizationManager(ILogger logger) {
        this.logger = logger;
    }

    #region ISynchronizationManager Members

    public void CreateSynchronizationBlock() {
        if (logger.IsDebugEnabled)
            logger.Debug("Beginning synchronization block in thread [{0}]", Thread.CurrentThread.GetHashCode());
        // add new list to stack
        synchronizations.Push(new List<ISynchronization>());
    }

    public void PerformSynchronizations(bool commited) {
        if (synchronizations.Count > 0) {
            if (logger.IsDebugEnabled)
                logger.Debug("Executing synchronizations in thread [{0}]", Thread.CurrentThread.GetHashCode());
            // get synchronization list on top of the stack
            IList<ISynchronization> synchronizationList = synchronizations.Pop();
            IList<Exception> exceptions = new List<Exception>();
            foreach (ISynchronization synchronization in synchronizationList) {
                try {
                    synchronization.AfterCompletion(commited);
                }
                catch (Exception ex) {
                    if (logger.IsDebugEnabled)
                        logger.Debug("Error performing syncronization", ex);
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0) {
                // log exceptions
                foreach (Exception exception in exceptions)
                    logger.Error("Error performing synchronization", exception);
                // throw exception
                throw new TransactionException("Error performing synchronizations", exceptions);
            }
        }
        else {
            // log error
            logger.Error("Cannot perform synchronizations without creating a synchronization block in thread [{0}]", Thread.CurrentThread.GetHashCode());
            // throw exception
            throw new TransactionException("Cannot perform synchronizations without creating a synchronization block");
        }
    }

    public void RegisterSynchronization(ISynchronization synchronization) {
        if (synchronizations.Count > 0) {
            if (logger.IsDebugEnabled)
                logger.Debug("Adding synchronization object [{0}] in thread [{1}]", synchronization.ToString(), Thread.CurrentThread.GetHashCode());
            // add to list
            synchronizations.Peek().Add(synchronization);
        }
        else {
            // log error
            logger.Error("Cannot add synchronization without creating a synchronization block in thread [{0}]", Thread.CurrentThread.GetHashCode());
            // throw exception
            throw new TransactionException("Cannot add synchronization without creating a synchronization block");
        }
    }

    #endregion
}