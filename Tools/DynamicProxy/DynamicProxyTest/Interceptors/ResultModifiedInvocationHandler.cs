namespace Castle.DynamicProxy.Test.Interceptors
{
	using System;

	/// <summary>
	/// Summary description for ResultModifiedInvocationHandler.
	/// </summary>
	public class ResultModifiedInvocationHandler : StandardInterceptor
	{
		protected override void PostProceed(IInvocation invocation, ref object returnValue, params object[] arguments)
		{
			if ( returnValue != null && returnValue.GetType() == typeof(int))
			{
				int value = (int) returnValue;
				returnValue = --value;
			}
		}
	}
}
