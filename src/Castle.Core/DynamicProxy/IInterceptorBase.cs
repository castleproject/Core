
namespace Castle.DynamicProxy
{
	using System;

	public class ExceptionContext
	{

		public Exception Ex { get; set; }
		public IInvocation Invocation { get; set; }
	}

	public abstract class InterceptorBase : IInterceptor
	{
		protected virtual void OnExcuting(IInvocation invocation)
		{
		}

		protected virtual void OnExcuted(IInvocation invocation)
		{
		}

		protected virtual void OnException(ExceptionContext context)
		{
		}

		private bool NotVoidHasValue(IInvocation invocation)
		{
			return !(invocation.Method.ReturnType == typeof(void) ||
			         invocation.ReturnValue == null);
		}

		public void Intercept(IInvocation invocation)
		{
			try
			{
				OnExcuting(invocation);

				if (NotVoidHasValue(invocation))
					return;

				invocation.Proceed();

				OnExcuting(invocation);
			}
			catch (Exception ex)
			{
				OnException(new ExceptionContext()
				{
					Ex = ex,
					Invocation = invocation
				});
			}
		}
	}
}
