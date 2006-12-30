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
#if DOTNET2
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteGenericComponentCallback));
#endif
		}

		public void ClientContainerConsumingRemoteGenericComponentCallback()
		{
#if DOTNET2
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            IGenericToStringService<String> service = clientContainer.Resolve<IGenericToStringService<String>>();

			Assert.IsTrue( RemotingServices.IsTransparentProxy(service) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			Assert.AreEqual("onetwo", service.ToString("one", "two"));
#endif
		}

        [Test]
        [ExpectedException(typeof(Castle.MicroKernel.ComponentNotFoundException))]
        public void ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServer()
        {
#if DOTNET2
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServerCallback));
#endif
        }

        public void ClientContainerConsumingRemoteGenericComponentWhichDoesNotExistOnServerCallback()
        {
#if DOTNET2
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            GenericToStringServiceImpl<String> service = clientContainer.Resolve<GenericToStringServiceImpl<String>>();        
#endif
        }


        [Test]
        public void ClientContainerConsumingRemoteCustomComponentUsingGenericInterface()
        {
#if DOTNET2            
            clientDomain.DoCallBack(new CrossAppDomainDelegate(ClientContainerConsumingRemoteCustomComponentUsingGenericInterfaceCallback));
#endif
        }

        public void ClientContainerConsumingRemoteCustomComponentUsingGenericInterfaceCallback()
        {
#if DOTNET2            
            IWindsorContainer clientContainer = CreateRemoteContainer(clientDomain, BuildConfigPath("client_kernelgenericcomponent.xml"));

            
            IGenericToStringService<StringBuilder> service = clientContainer.Resolve<IGenericToStringService<StringBuilder>>();

            Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
            Assert.IsTrue(RemotingServices.IsObjectOutOfAppDomain(service));

            Assert.AreEqual("33", service.ToString(new StringBuilder("one"), new StringBuilder("two")));
#endif
        }
	}
}
