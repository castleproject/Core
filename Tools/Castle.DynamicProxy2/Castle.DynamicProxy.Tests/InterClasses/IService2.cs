namespace Castle.DynamicProxy.Tests.InterClasses
{
	using System;

	public interface IService2
	{
		void DoOperation2();
	}

	public class Service2 : MarshalByRefObject, IService2
	{
		public void DoOperation2()
		{
		}
	}
}
