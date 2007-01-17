
using Castle.DynamicProxy.Tests.Interceptors;

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using NUnit.Framework;


	[TestFixture]
	public class RhinoMocksTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void UsingEvents_Interface()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor(); 
			IWithEvents proxy = (IWithEvents)generator.CreateInterfaceProxyWithTarget(typeof (IWithEvents),
				new FakeWithEvents(),
				logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void UsingEvents_Class()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			FakeWithEvents proxy = (FakeWithEvents)generator.CreateClassProxy(
				typeof(FakeWithEvents),
				ProxyGenerationOptions.Default, 
				logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		public interface IWithEvents
		{
			event EventHandler Foo;
		}

		public class FakeWithEvents : IWithEvents
		{
			public virtual event EventHandler Foo;

			public void MethodToGetAroundFooNotUsedError()
			{
				Foo(this,EventArgs.Empty);
			}
		}
	}
}
