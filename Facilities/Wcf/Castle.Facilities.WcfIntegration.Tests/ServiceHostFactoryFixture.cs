// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System;
	using System.ServiceModel;
	using NUnit.Framework;
	using Windsor;

	[TestFixture]
	public class ServiceHostFactoryFixture
	{
		[Test]
		public void CanCreateServiceByName()
		{
			WindsorContainer windsorContainer = new WindsorContainer();
			windsorContainer.AddComponent("operations", typeof (IOperations), typeof (Operations));
			WindsorServiceHostFactory factory = new WindsorServiceHostFactory(windsorContainer.Kernel);
			ServiceHostBase serviceHost = factory.CreateServiceHost("operations", new Uri[] {new Uri("http://localhost/Foo.svc")});
			Assert.IsInstanceOfType(typeof (WindsorServiceHost), serviceHost);
		}

		[Test]
		public void CanCreateWindsorHostFactory()
		{
			WindsorServiceHostFactory factory = new WindsorServiceHostFactory(new WindsorContainer().Kernel);
			Assert.IsNotNull(factory);
		}
	}
}