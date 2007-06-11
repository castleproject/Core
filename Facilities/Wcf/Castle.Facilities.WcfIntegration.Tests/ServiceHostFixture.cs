using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Facilities.WcfIntegration.Tests
{
	[TestFixture]
	public class ServiceHostFixture
	{
		[Test]
		public void CanCreateServiceHost()
		{
			WindsorServiceHost host = new WindsorServiceHost(new WindsorContainer(),typeof(Operations));
			Assert.IsNotNull(host);
		}

		[Test]
		public void CanCreateServiceHostAndOpenHost()
		{
			WindsorContainer windsorContainer = new WindsorContainer();
			windsorContainer.AddComponent("operations", typeof(IOperations), typeof(Operations));
			Uri uri = new Uri("net.tcp://localhost/WCF.Facility");
			WindsorServiceHost host = new WindsorServiceHost(windsorContainer,typeof(Operations),
				uri);
			host.Description.Endpoints.Add(
				new ServiceEndpoint(ContractDescription.GetContract(typeof(IOperations)), 
				new NetTcpBinding(),
				new EndpointAddress(uri)));
			host.Open();
			host.Close();
		}
	}
}