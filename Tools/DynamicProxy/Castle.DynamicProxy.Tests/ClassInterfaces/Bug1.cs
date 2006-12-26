namespace Castle.DynamicProxy.Test.ClassInterfaces
{
	public class SimpleInterceptor : IInterceptor
	{
		public object Intercept(IInvocation invocation, params object[] args)
		{
			return null;
		}
	}

	namespace A.B.C
	{
		public interface IBubu
		{
			void OperationA();
		}
	}

	namespace A.B.D
	{
		public interface IBubu
		{
			void OperationB();
		}
	}

}
