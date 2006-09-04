
public class AutomaticSessionInterceptor: IMethodInterceptor {

    private IKernel kernel;
    private ILogger logger;

    public AutomaticSessionInterceptor(IKernel kernel, ILogger logger) {
        this.kernel = kernel;
        this.logger = logger;
    }

    public object Intercept(IMethodInvocation invocation, params object[] args) {
        // get method info
        MethodInfo info = invocation.MethodInvocationTarget;
        if (info.IsDefined(typeof(SessionAttribute), true)) {
            // get SqlMapper key in invocation method
            String key = ObtainSqlMapperKeyFor(info);
            // get SqlMapper for this key
            ISqlMapper sqlMap = ObtainSqlMapperFor(key);
            if (!sqlMap.IsSessionStarted) {
                // log debug info
                if (logger.IsDebugEnabled)
                    logger.Debug("Automatic Open connection on method: " + invocation.Method.Name);
                // open connection
                sqlMap.OpenConnection();
                // enlist in outer transaction if any
                Transaction transaction = Transaction.Current;
                if (transaction != null) {
                    if (logger.IsDebugEnabled)
                        logger.Debug("Enlisting resource in transaction [{0}] to close connection after commit/rollback", transaction.GetHashCode());
                    // get synchronization manager
                    ISynchronizationManager synchronizationManager = (ISynchronizationManager)kernel[typeof(ISynchronizationManager)];
                    // enlist in transaction to close DB conection after commit
                    synchronizationManager.RegisterSynchronization(new SqlMapperSynchronization(sqlMap));
                }
                else {
                    try {
                        return invocation.Proceed(args);
                    }
                    finally {
                        if (logger.IsDebugEnabled)
                            logger.Debug("Closing connection on method: " + invocation.Method.Name);
                        sqlMap.CloseConnection();
                    }
                }
            }            
        }
        // invoke target method
        return invocation.Proceed(args);
    }

    private String ObtainSqlMapperKeyFor(MethodInfo info) {
        SessionAttribute[] attributes = info.GetCustomAttributes(typeof(SessionAttribute), true) as SessionAttribute[];
        return attributes[0].SqlMapId != null ? attributes[0].SqlMapId : string.Empty;
    }

    private ISqlMapper ObtainSqlMapperFor(String key) {
        if (String.Empty.Equals(key))
            return (ISqlMapper)kernel[typeof(ISqlMapper)];
        return (ISqlMapper)kernel[key];
    }
}