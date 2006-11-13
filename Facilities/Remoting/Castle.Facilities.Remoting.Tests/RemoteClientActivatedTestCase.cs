// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.Remoting.Tests
{
	using System;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Activation;
	using Castle.Windsor;

	using Castle.Facilities.Remoting.TestComponents;

	using NUnit.Framework;


	[TestFixture, Serializable]
	public class RemoteClientActivatedTestCase : AbstractRemoteTestCase
	{
		protected override String GetServerConfigFile()
		{
			return BuildConfigPath("server_clientactivated.xml");
		}

		[Test]
		public void CommonAppConsumingRemoteComponents()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(CommonAppConsumingRemoteComponentsCallback));
		}

		public void CommonAppConsumingRemoteComponentsCallback()
		{
			ICalcService service = (ICalcService) 
				Activator.CreateInstance( 
					typeof(CalcServiceImpl), null, 
					new object[] { new UrlAttribute("tcp://localhost:2133/") } );

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}

		[Test]
		public void ClientContainerConsumingRemoteComponent()
		{
			clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteComponentCallback));
		}

		public void ClientContainerConsumingRemoteComponentCallback()
		{
			IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_clientactivated.xml"));

			ICalcService service = (ICalcService) clientContainer[ typeof(ICalcService) ];

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual(10, service.Sum(7,3));
		}
	}
}
