namespace Castle.DynamicProxy.Tests.BugsReported
{
	public interface IMyInterface
	{
		void MyTestMethod(string myParam);
	}

	public class MyClass : IMyInterface
	{
		private string myProperty;

		public virtual string MyProperty
		{
			get { return myProperty; }
			set { myProperty = value; }
		}

		public virtual void MyTestMethod(string myParam)
		{
		}
	}
}
