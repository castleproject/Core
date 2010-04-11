namespace Castle.DynamicProxy.Tests.Interceptors
{
    using System;

#if !SILVERLIGHT
    [Serializable]
#endif
    public class AddTwoInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            int ret = (int)invocation.ReturnValue;
            ret += 2;
            invocation.ReturnValue = ret;
        }

        #endregion
    }
}
