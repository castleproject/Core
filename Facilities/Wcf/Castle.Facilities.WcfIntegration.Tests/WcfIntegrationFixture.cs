using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.Facilities.WcfIntegration.Tests.Behaviors;
using Castle.MicroKernel;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Facilities.WcfIntegration.Tests
{
	[TestFixture]
	public class WcfIntegrationFixture
	{
		private WindsorServiceHost host;
		private IOperations client;

		[SetUp]
		public void TestInitialize()
		{
			WindsorContainer windsorContainer = new WindsorContainer();
			windsorContainer.AddComponent("logging", typeof(LoggingInterceptor));
			windsorContainer.AddComponent("call_count", typeof (IServiceBehavior), typeof (CallCountServiceBehavior));
			windsorContainer.AddComponent("operations", typeof (IOperations), typeof (Operations));
			windsorContainer.AddComponent("unit_of_work", typeof(IEndpointBehavior), typeof(UnitOfworkEndPointBehavior));

			IHandler handler = windsorContainer.Kernel.GetHandler("operations");
			handler.ComponentModel.Interceptors.Add(new InterceptorReference("logging"));
			handler.ComponentModel.Parameters.Add("number", "42");

			Uri uri = new Uri("net.tcp://localhost/WCF.Facility");
			host = new WindsorServiceHost(windsorContainer, typeof (Operations),
			                              uri);
			EndpointAddress endpointAddress = new EndpointAddress(uri);
			this.host.Description.Endpoints.Add(
				new ServiceEndpoint(ContractDescription.GetContract(typeof (IOperations)),
				                    new NetTcpBinding(),
				                    endpointAddress));
			CallCountServiceBehavior.CallCount = 0;
			LoggingInterceptor.Calls.Clear();
			this.host.Open();
			client = ChannelFactory<IOperations>.CreateChannel(new NetTcpBinding(),endpointAddress);
		}

		[TearDown]
		public void TestCleanup()
		{
			host.Close();
		}

		[Test]
		public void CanCallServiceAndGetValueFromWindsorConfig()
		{
			int result = client.GetValueFromConstructor();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void WillApplyServiceBehaviors()
		{
			Assert.AreEqual(0, CallCountServiceBehavior.CallCount );
			client.GetValueFromConstructor();
			Assert.AreEqual(1, CallCountServiceBehavior.CallCount );
		}

		[Test]
		public void CanUseStandardDynamicProxyInterceptorsOnServices()
		{
			Assert.AreEqual(0, LoggingInterceptor.Calls.Count );
			client.GetValueFromConstructor();
			Assert.AreEqual(1, LoggingInterceptor.Calls.Count );
		}

		[Test]
		public void WillApplyEndPointBehaviors()
		{
			Assert.IsFalse(UnitOfWork.initialized, "Should be false before starting");
			bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
			Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
			Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
		}
	}

	public class LoggingInterceptor : IInterceptor
	{
		public static List<string> Calls = new List<string>();

		public void Intercept(IInvocation invocation)
		{
			Calls.Add(invocation.Method.Name);
			invocation.Proceed();
		}
	}
}