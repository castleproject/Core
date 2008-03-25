using System;
using Castle.Facilities.Remoting.TestComponents;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Facilities.Remoting.Tests
{
	[TestFixture, Serializable]
	public class ConfigurableRegistrationTestCase : AbstractRemoteTestCase
	{
		protected override string GetServerConfigFile()
		{
			return BuildConfigPath("server_confreg_clientactivated.xml");
		}

		[Test]
		public void ClientContainerConsumingRemoteComponents()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteComponentsCallback));
		}

		public void ClientContainerConsumingRemoteComponentsCallback()
		{
			IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_confreg_clientactivated.xml"));

			Assert.IsNotNull(clientContainer.Kernel.ResolveAll<ICalcService>());
		}
	}
}
