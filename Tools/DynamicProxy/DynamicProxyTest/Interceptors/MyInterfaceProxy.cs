namespace Castle.DynamicProxy.Test.Interceptors
{
	using System;

	public class MyInterfaceProxy : StandardInterceptor
	{
		protected override void PreProceed(IInvocation invocation, params object[] args)
		{
			base.PreProceed(invocation, args);
		}
	}
}
