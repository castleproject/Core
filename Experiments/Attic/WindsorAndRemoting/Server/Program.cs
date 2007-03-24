namespace Server
{
	using System;
	using System.Runtime.Remoting;
	using System.Threading;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.Windsor;

	[Transient]
	public class AuditInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			Console.WriteLine("Intercepted {0}", invocation.Method.Name);
			
			invocation.Proceed();
		}
	}

	[Interceptor(typeof(AuditInterceptor))]
	public class Service1 : MarshalByRefObject
	{
		public virtual void DoOperation()
		{
			Console.WriteLine("Done");
		}
	}

	public interface IService2
	{
		void DoOperation2();
	}
	
	[Interceptor(typeof(AuditInterceptor))]
	public class Service2 : MarshalByRefObject, IService2
	{
		public void DoOperation2()
		{
			Console.WriteLine("Done2");
		}
	}

	class Program
	{
		public static void Main(string[] args)
		{
			RemotingConfiguration.Configure("RemotingTcpConfig.config", false);
			
			WindsorContainer container = new WindsorContainer();

			container.AddComponent("interceptor1", typeof(AuditInterceptor));
			container.AddComponent("service1", typeof(Service1));
			container.AddComponent("service2", typeof(IService2), typeof(Service2));
			
			Service1 serv1 = container.Resolve<Service1>();
			MarshalByRefObject serv2 = 
				(MarshalByRefObject) container.Resolve<IService2>();

			RemotingServices.Marshal(serv1, "serv1");
			RemotingServices.Marshal(serv2, "serv2");

			Thread.CurrentThread.Join();
		}
	}
}
