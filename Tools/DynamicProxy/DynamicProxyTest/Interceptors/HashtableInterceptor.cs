namespace Castle.DynamicProxy.Test.Interceptors
{
	using System;

	public class HashtableInterceptor : StandardInterceptor
	{
		public override object Intercept(IInvocation invocation, params object[] args)
		{
			if (invocation.Method.Name.Equals("get_Item"))
			{
				object item = base.Intercept(invocation, args);
				return (item == null) ? "default" : item;
			}
			return base.Intercept(invocation, args);
		}
	}
}
