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
    using Castle.Core.Resource;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using Castle.Facilities.WcfIntegration.Demo;
	using NUnit.Framework;
	using Castle.Facilities.WcfIntegration.Tests.Behaviors;

#if DOTNET35

	[TestFixture]
	public class ServiceHostFixture
	{
		[Test]
		public void CanCreateServiceHostAndOpenHost()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 } )
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"))
						)	
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostFromConfiguration()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<UsingWindsor>()
					.DependsOn(new { number = 42 } )
					.ActAs(new DefaultServiceModel())
				))
			{
				IAmUsingWindsor client = ChannelFactory<IAmUsingWindsor>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc"));
				Assert.AreEqual(42, client.GetValueFromWindsorConfig());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostWithMultipleEndpoints()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"),
						WcfEndpoint.ForContract<IOperationsEx>()
							.BoundTo(new BasicHttpBinding())
							.At("http://localhost:27198/UsingWindsor.svc")
						)
				)))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				IOperationsEx clientEx = ChannelFactory<IOperationsEx>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc"));
				clientEx.Backup(new Dictionary<string, object>());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostWithRelativeEndpoints()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddBaseAddresses(
							"net.tcp://localhost/Operations",
							"http://localhost:27198/UsingWindsor.svc")
						.AddEndpoints(
							WcfEndpoint.ForContract<IOperations>()
								.BoundTo(new NetTcpBinding{PortSharingEnabled = true }),
							WcfEndpoint.ForContract<IOperationsEx>()
								.BoundTo(new BasicHttpBinding())
								.At("Extended")
							)
					)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				IOperationsEx clientEx = ChannelFactory<IOperationsEx>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc/Extended"));
				clientEx.Backup(new Dictionary<string, object>());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostWithListenAddress()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("urn:castle:operations")
							.Via("net.tcp://localhost/Operations")
							)
				)))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("urn:castle:operations"),
					new Uri("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostFromXmlConfiguration()
		{
			using (new WindsorContainer()
                    .Install(Configuration.FromXml(new StaticContentResource(xmlConfiguration))))
			{
				IAmUsingWindsor client = ChannelFactory<IAmUsingWindsor>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc"));
				Assert.AreEqual(42, client.GetValueFromWindsorConfig());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostWithMultipleServiceModels()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(
						new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations")
								),
						new DefaultServiceModel()
							.AddBaseAddresses(
								"http://localhost:27198/UsingWindsor.svc")
							.AddEndpoints(
								WcfEndpoint.ForContract<IOperationsEx>()
									.BoundTo(new BasicHttpBinding())
									.At("Extended")
								)
							)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				IOperationsEx clientEx = ChannelFactory<IOperationsEx>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc/Extended"));
				clientEx.Backup(new Dictionary<string, object>());
			}
		}

		[Test]
		public void WillApplyServiceScopedBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;
			Assert.IsFalse(UnitOfWork.initialized, "Should be false before starting");

			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Configuration(Attrib.ForName("scope").Eq(WcfBehaviorScope.Clients)),
					Component.For<UnitOfworkEndPointBehavior>()
						.Configuration(Attrib.ForName("scope").Eq(WcfBehaviorScope.Services)),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
				Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
				Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
				Assert.AreEqual(0, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyExplcitScopedKeyBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Named("callcounts")
						.Configuration(Attrib.ForName("scope").Eq(WcfBehaviorScope.Explicit)),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddBehaviors("callcounts")
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyExplcitScopedServiceBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Configuration(Attrib.ForName("scope").Eq(WcfBehaviorScope.Explicit)),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddBehaviors(typeof(CallCountServiceBehavior))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyInstanceBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding{PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddBehaviors(new CallCountServiceBehavior(),
						              new UnitOfworkEndPointBehavior())
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding{PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
				Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
				Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

        private static string xmlConfiguration = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<facilities>
		<facility id='wcf' 
				  type='Castle.Facilities.WcfIntegration.WcfFacility,
				        Castle.Facilities.WcfIntegration' />
	</facilities>

	<components>
		<component id='usingwindsor_svc'
			       service='Castle.Facilities.WcfIntegration.Demo.IAmUsingWindsor, 
				            Castle.Facilities.WcfIntegration.Demo'
			       type='Castle.Facilities.WcfIntegration.Demo.UsingWindsor, 
				         Castle.Facilities.WcfIntegration.Demo'
			       wcfServiceHost='true'>
			<parameters>
				<number>42</number>
			</parameters>
		</component>
	</components>
</configuration>";
	}

#endif // DOTNET35
}
