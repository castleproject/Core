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

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Description;
	using Castle.Core.Configuration;
	using Castle.Core.Resource;
	using Castle.Facilities.Logging;
	using Castle.Facilities.WcfIntegration.Behaviors;
	using Castle.Facilities.WcfIntegration.Demo;
	using Castle.Facilities.WcfIntegration.Tests.Behaviors;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using log4net.Appender;
	using log4net.Config;
	using log4net.Core;
	using NUnit.Framework;

	[TestFixture]
	public class ServiceHostFixture
	{
		private MemoryAppender memoryAppender;

		[Test]
		public void CanCreateServiceHostAndOpenHost()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test, Explicit]
		public void CanCreateServiceHostPerCallAndOpenHost()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<IServiceBehavior>()
					.Instance(new ServiceBehaviorAttribute()
					{
						InstanceContextMode = InstanceContextMode.PerCall,
						ConcurrencyMode = ConcurrencyMode.Multiple
					}),
					Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true },
					new EndpointAddress("net.tcp://localhost/Operations"));
				((IClientChannel)client).Open();

				for (int i = 0; i < 10; i++)
				{
					new System.Threading.Thread(() =>
					{
						int refValue = 0, outValue;
						Assert.AreEqual(42, client.GetValueFromConstructorAsRefAndOut(ref refValue, out outValue));
						//var result = client.BeginGetValueFromConstructorAsRefAndOut(ref refValue, null, null);
						//Assert.AreEqual(42, client.EndGetValueFromConstructorAsRefAndOut(ref refValue, out outValue, result));
					}).Start();
				}
				System.Threading.Thread.CurrentThread.Join();
				//((ICommunicationObject)client).Close();
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostUsingDefaultBinding()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => 
					{ 
						f.DefaultBinding = new NetTcpBinding { PortSharingEnabled = true };
						f.CloseTimeout = TimeSpan.Zero;
					}
				)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.ForContract<IOperations>()
							.At("net.tcp://localhost/Operations")
						)
				)))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostFromConfiguration()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<UsingWindsor>()
					.DependsOn(new { number = 42 })
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
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.ForContract<IOperations>()
							.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"),
						WcfEndpoint.ForContract<IOperationsEx>()
							.BoundTo(new BasicHttpBinding())
							.At("http://localhost:27198/UsingWindsor.svc")
						)
				)))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
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
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddBaseAddresses(
							"net.tcp://localhost/Operations",
							"http://localhost:27198/UsingWindsor.svc")
						.AddEndpoints(
							WcfEndpoint.ForContract<IOperations>()
								.BoundTo(new NetTcpBinding { PortSharingEnabled = true }),
							WcfEndpoint.ForContract<IOperationsEx>()
								.BoundTo(new BasicHttpBinding())
								.At("Extended")
							)
					)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
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
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("urn:castle:operations")
							.Via("net.tcp://localhost/Operations")
							)
				)))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("urn:castle:operations"),
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
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(Component.For<IOperations>()
					.ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(
						new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
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
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
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
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Clients),
					Component.For<UnitOfworkEndPointBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Services),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddEndpoints(
						WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
							.At("net.tcp://localhost/Operations"))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
				Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
				Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
				Assert.AreEqual(0, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyServiceScopedBehaviorsToDefaultEndpoint()
		{
			CallCountServiceBehavior.CallCount = 0;
			Assert.IsFalse(UnitOfWork.initialized, "Should be false before starting");

			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Services),
					Component.For<UnitOfworkEndPointBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Services),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel().AddBaseAddresses(
						"net.tcp://localhost/Operations"))
					)
				)
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
				Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
				Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyServiceScopedBehaviorsToMultipleEndpoints()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<DummyContractBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Services),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"),
							WcfEndpoint.BoundTo(new BasicHttpBinding())
								.At("http://localhost/Operations")
							)
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void WillApplyExplcitScopedKeyBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Named("callcounts")
						.Attribute("scope").Eq(WcfExtensionScope.Explicit),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddEndpoints(
							WcfEndpoint.BoundTo(new BasicHttpBinding())
								.At("http://localhost/Operations"))
						.AddExtensions("callcounts")
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyExplcitScopedServiceBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<CallCountServiceBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Explicit),
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddExtensions(typeof(CallCountServiceBehavior))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void WillApplyInstanceBehaviors()
		{
			CallCountServiceBehavior.CallCount = 0;

			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<IOperations>().ImplementedBy<Operations>()
					.DependsOn(new { number = 42 })
					.ActAs(new DefaultServiceModel()
						.AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						.AddExtensions(new CallCountServiceBehavior(),
									   new UnitOfworkEndPointBehavior())
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
				Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
				Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
				Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesAtEndpointLevel()
		{
			using (IWindsorContainer container = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<LogMessageEndpointBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Explicit)
						.Named("logMessageBehavior"),
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations")
								.LogMessages()
							))
				))
			{
				RegisterLoggingFacility(container);

				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(4, memoryAppender.GetEvents().Length);

				foreach (LoggingEvent log in memoryAppender.GetEvents())
				{
					Assert.AreEqual(typeof(Operations).FullName, log.LoggerName);
					Assert.IsTrue(log.Properties.Contains("NDC"));
				}
			}
		}

		[Test]
		public void CanCaptureRequestsAndResponsesAtServiceLevel()
		{
			using (IWindsorContainer container = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<LogMessageEndpointBehavior>()
						.Attribute("scope").Eq(WcfExtensionScope.Explicit)
						.Named("logMessageBehavior"),
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
							.LogMessages()
							)
				))
			{
				RegisterLoggingFacility(container);

				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
				Assert.AreEqual(4, memoryAppender.GetEvents().Length);
			}
		}

		[Test]
		public void CanModifyRequestsAndResponses()
		{
			using (IWindsorContainer container = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<MessageLifecycleBehavior>(),
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations")
								.AddExtensions(new ReplaceOperationsResult("100")))
							)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, 
					new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(100, client.GetValueFromConstructor());
			}
		}

		[Test, ExpectedException(typeof(FaultException<ExceptionDetail>))]
		public void CanGiveFriendlyErrorMessageForUunresolvedServiceDependenciesIfOpenEagerly()
		{
			using (IWindsorContainer container = new WindsorContainer()
				.AddFacility<WcfFacility>(f => f.CloseTimeout = TimeSpan.Zero)
				.Register(
					Component.For<IServiceBehavior>()
						.Instance(new ServiceDebugBehavior()
						{
							IncludeExceptionDetailInFaults = true
						}),
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.ActAs(new DefaultServiceModel()
							.OpenEagerly()
							.AddEndpoints(WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
						)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, 
					new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void CanCreateServiceHostWithAspNetCompatibility()
		{
			var captureServiceHost = new CaptureServiceHost();

			using (new WindsorContainer()
				.AddFacility<WcfFacility>(f => 
					{
						f.CloseTimeout = TimeSpan.Zero;
						f.Services.AspNetCompatibility = AspNetCompatibilityRequirementsMode.Allowed;
					})
				.Register(
					Component.For<CaptureServiceHost>().Instance(captureServiceHost),
					Component.For<IOperations>()
						.ImplementedBy<Operations>()
						.DependsOn(new { number = 42 })
						.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding { PortSharingEnabled = true })
								.At("net.tcp://localhost/Operations"))
							)
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding { PortSharingEnabled = true }, new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				AspNetCompatibilityRequirementsAttribute aspNetCompat =
					captureServiceHost.ServiceHost.Description.Behaviors.Find<AspNetCompatibilityRequirementsAttribute>();
				Assert.IsNotNull(aspNetCompat);
				Assert.AreEqual(AspNetCompatibilityRequirementsMode.Allowed, aspNetCompat.RequirementsMode);
			}
		}

		protected void RegisterLoggingFacility(IWindsorContainer container)
		{
			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "logging";
			facNode.Attributes["loggingApi"] = "ExtendedLog4net";
			facNode.Attributes["configFile"] = "";
			container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", facNode);
			container.AddFacility("logging", new LoggingFacility());

			memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(memoryAppender);
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
}
