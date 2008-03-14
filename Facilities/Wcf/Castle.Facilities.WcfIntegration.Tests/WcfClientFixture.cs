// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Castle.Core;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Facilities.WcfIntegration.Demo;
	using NUnit.Framework;

	[TestFixture]
	public class WcfClientFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void TestInitialize()
		{
			windsorContainer = new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(
					Component.For<IOperations>().ImplementedBy<Operations>()
						.CustomDependencies(new
						{
							number = 42,
							serviceModel = new WcfServiceModel()
								.AddEndpoints(new WcfEndpoint()
								{
									Binding = new NetTcpBinding(),
									Address = "net.tcp://localhost/Operations"
								})
						}),
					Component.For<IAmUsingWindsor>().ImplementedBy<UsingWindsor>()
						.CustomDependencies(new
						{
							number = 42,
							serviceModel = new WcfServiceModel()
						})
				);
		}

		[TearDown]
		public void TestCleanup()
		{
			windsorContainer.Dispose();
		}

		#endregion

		private IWindsorContainer windsorContainer;

		[Test]
		public void CanResolveClientAssociatedWithChannel()
		{
			windsorContainer.Register(
				Component.For<IOperations>()
					.Named("operations")
					.CustomDependencies(new
						{
							clientModel = new WcfClientModel()
							{
								Binding = new NetTcpBinding(),
								Address = "net.tcp://localhost/Operations"
							}
						})
				);

			IOperations client = windsorContainer.Resolve<IOperations>("operations");
			Assert.AreEqual(42, client.GetValueFromConstructor());
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelFromConfiguration()
		{
			windsorContainer.Register(Component.For<IAmUsingWindsor>()
				.Named("usingWindsor")
				.CustomDependencies(new
				{
					clientModel = new WcfClientModel("WSHttpBinding_IAmUsingWindsor")
				}));

			IAmUsingWindsor client = windsorContainer.Resolve<IAmUsingWindsor>("usingWindsor");
			Assert.AreEqual(42, client.GetValueFromWindsorConfig());
		}
	}
}