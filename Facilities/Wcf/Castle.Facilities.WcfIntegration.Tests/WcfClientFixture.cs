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
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using Castle.Facilities.WcfIntegration.Demo;
	using Castle.Facilities.WcfIntegration.Tests.Behaviors;
	using NUnit.Framework;

#if DOTNET35

	[TestFixture]
	public class WcfClientFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void TestInitialize()
		{
			windsorContainer = new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(WcfClient.ForChannels(
					new WcfClientModel()
					{
						Endpoint = WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding())
							.At("net.tcp://localhost/Operations")
					}))
				.Register(
					Component.For<IServiceBehavior>().Instance(new ServiceDebugBehavior()
						{
							IncludeExceptionDetailInFaults = true
						}),
					Component.For<IOperationBehavior>().ImplementedBy<NetDataContractFormatBehavior>(),
					Component.For<Operations>()
						.CustomDependencies(new
						{
							number = 42,
							serviceModel = new WcfServiceModel()
								.AddEndpoints(
									WcfEndpoint.ForContract<IOperations>()
										.BoundTo(new NetTcpBinding())
										.At("net.tcp://localhost/Operations"),
									WcfEndpoint.ForContract<IOperationsEx>()
										.BoundTo(new NetTcpBinding())
										.At("net.tcp://localhost/Operations/Ex")
										)
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
								Endpoint = WcfEndpoint
									.BoundTo(new NetTcpBinding())
									.At("net.tcp://localhost/Operations")
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
					clientModel = new WcfClientModel()
					{
						Endpoint = WcfEndpoint
							.FromConfiguration("WSHttpBinding_IAmUsingWindsor")
					}
				}));

			IAmUsingWindsor client = windsorContainer.Resolve<IAmUsingWindsor>("usingWindsor");
			Assert.AreEqual(42, client.GetValueFromWindsorConfig());
		}

		[Test]
		public void CanResolveClientWhenListedInTheFacility()
		{
			windsorContainer.Register(Component.For<ClassNeedingService>());
			ClassNeedingService component = windsorContainer.Resolve<ClassNeedingService>();
			Assert.IsNotNull(component.Operations);
			Assert.AreEqual(42, component.Operations.GetValueFromConstructor());
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingRelativeAddress()
		{
			using (new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(Component.For<Operations>()
					.CustomDependencies(new
					{
						number = 28,
						serviceModel = new WcfServiceModel()
							.AddBaseAddresses(
								"net.tcp://localhost/Operations")
							.AddEndpoints(
								WcfEndpoint.ForContract<IOperations>()
									.BoundTo(new NetTcpBinding())
									.At("Extended")
								)
					})
				))
			{
				using (IWindsorContainer clientContainer = new WindsorContainer()
					.AddFacility("wcf_facility", new WcfFacility())
					.Register(Component.For<IOperations>()
						.Named("operations")
						.CustomDependencies(new
						{
							clientModel = new WcfClientModel()
							{
								Endpoint = WcfEndpoint
									.BoundTo(new NetTcpBinding())
									.At("net.tcp://localhost/Operations/Extended")
							}
						})
					))
				{
					IOperations client = clientContainer.Resolve<IOperations>("operations");
					Assert.AreEqual(28, client.GetValueFromConstructor());
				}
			}
		}

		[Test]
		public void CanResolveClientAssociatedWithChannelUsingViaAddress()
		{
			using (IWindsorContainer localContainer = new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(
					Component.For<IOperations>().ImplementedBy<Operations>()
						.CustomDependencies(new
						{
							number = 22,
							serviceModel = new WcfServiceModel()
								.AddEndpoints(
									WcfEndpoint.BoundTo(new NetTcpBinding())
										.At("urn:castle:operations")
										.Via("net.tcp://localhost/OperationsVia")
										)
						}),
					Component.For<IOperations>()
						.Named("operations")
						.CustomDependencies(new
						{
							clientModel = new WcfClientModel()
							{
								Endpoint = WcfEndpoint
									.BoundTo(new NetTcpBinding())
									.At("urn:castle:operations")
									.Via("net.tcp://localhost/OperationsVia")
							}
						})
					))
			{
				IOperations client = localContainer.Resolve<IOperations>("operations");
				Assert.AreEqual(22, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void WillApplyOperationBehaviors()
		{
			windsorContainer.Register(
				Component.For<IOperationsEx>()
					.Named("operations")
					.CustomDependencies(new
					{
						clientModel = new WcfClientModel()
						{
							Endpoint = WcfEndpoint
								.BoundTo(new NetTcpBinding())
								.At("net.tcp://localhost/Operations/Ex")
						}
					})
				);

			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			client.Backup(new Dictionary<string, object>());
		}

		[Test]
		public void WillRecoverFromAnUnhandledException()
		{
			windsorContainer.Register(
				Component.For<IOperationsEx>()
					.Named("operations")
					.CustomDependencies(new
					{
						clientModel = new WcfClientModel()
						{
							Endpoint = WcfEndpoint
								.BoundTo(new NetTcpBinding())
								.At("net.tcp://localhost/Operations/Ex")
						}
					})
				);

			IOperationsEx client = windsorContainer.Resolve<IOperationsEx>("operations");
			try
			{
				client.ThrowException();
			}
			catch (Exception)
			{
				client.Backup(new Dictionary<string, object>());
			}
		}
			
		[Test]
		public void CanResolveClientAssociatedWithChannelFromXmlConfiguration()
		{
			using (IWindsorContainer container = new WindsorContainer()
					.Install(Configuration.FromXmlFile("..\\..\\ConfigureClients.xml"))
					)
			{
				IAmUsingWindsor client = container.Resolve<IAmUsingWindsor>("usingWindsor");
				Assert.AreEqual(42, client.GetValueFromWindsorConfig());
			}
		}
	}
#endif // DOTNET35
}