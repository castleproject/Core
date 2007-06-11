using System;
using System.ServiceModel;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Facilities.WcfIntegration.Tests
{
	[TestFixture]
	public class ServiceHostFactoryFixture
	{
		[Test]
		public void CanCreateWindsorHostFactory()
		{
			WindsorServiceHostFactory factory = new WindsorServiceHostFactory(new WindsorContainer());
			Assert.IsNotNull(factory);
		}

		[Test]
		public void CanCreateServiceByName()
		{
			WindsorContainer windsorContainer = new WindsorContainer();
			windsorContainer.AddComponent("operations", typeof(IOperations), typeof(Operations));
			WindsorServiceHostFactory factory = new WindsorServiceHostFactory(windsorContainer);
			ServiceHostBase serviceHost = factory.CreateServiceHost("operations", new Uri[]{new Uri("http://localhost/Foo.svc")});
			Assert.IsInstanceOfType(typeof(WindsorServiceHost), serviceHost);
		}
	}
}
    