// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
    using System.Text;
	using System.Runtime.Remoting;

	using Castle.Windsor;

	using Castle.Facilities.Remoting.TestComponents;

	using NUnit.Framework;

	[TestFixture, Serializable]
	public class RemoteGenericComponentTestCase : AbstractRemoteTestCase
	{
		protected override String GetServerConfigFile()
		{
			return BuildConfigPath("server_kernelgenericcomponent.xml");
		}

		[Test]
        public void ClientContainerConsumingRemoteGenericComponent()
		{
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteGenericComponentCallback));
		}

		public void ClientContainerConsumingRemoteGenericComponentCallback()
		{
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            IGenericToStringService<String> service = clientContainer.Resolve<IGenericToStringService<String>>();

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual("onetwo", service.ToString("one", "two"));
		}

        [Test]
        [ExpectedException(typeof(Castle.MicroKernel.ComponentNotFoundException))]
        public void ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServer()
        {
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServerCallback));
        }

        public void ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServerCallback()
        {
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            GenericToStringServiceImpl<String> service = clientContainer.Resolve<GenericToStringServiceImpl<String>>();        
        }


        [Test]
        public void ClientContainerConsumingRemoteCustomComponentUsingGenericInterface()
        {
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteCustomComponentUsingGenericInterfaceCallback));
        }

        public void ClientContainerConsumingRemoteCustomComponentUsingGenericInterfaceCallback()
        {
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            
            IGenericToStringService<StringBuilder> service = clientContainer.Resolve<IGenericToStringService<StringBuilder>>();

            Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
            Assert.IsTrue(RemotingServices.IsObjectOutOfAppDomain(service));

            Assert.AreEqual("33", service.ToString(new StringBuilder("one"), new StringBuilder("two")));
        }
	}
}
