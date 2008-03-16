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
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Facilities.WcfIntegration.Demo;
	using NUnit.Framework;

	[TestFixture]
	public class ServiceHostFixture
	{
		[Test]
		public void CanCreateServiceHost()
		{
			WindsorServiceHost host = new WindsorServiceHost(
				new WindsorContainer().Kernel, typeof (Operations));
			Assert.IsNotNull(host);
		}

		[Test]
		public void CanCreateServiceHostAndOpenHost()
		{
			using (new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(Component.For<IOperations>().ImplementedBy<Operations>()
					.CustomDependencies(new
					{
						number = 42,
						serviceModel = new WcfServiceModel()
							.AddEndpoints(
								WcfEndpoint.WithBinding(new NetTcpBinding())
									.At("net.tcp://localhost/Operations")
									)
					})
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding(), new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostFromConfiguration()
		{
			using (new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(Component.For<UsingWindsor>()
					.CustomDependencies(new
					{
						number = 42,
						serviceModel = new WcfServiceModel()
					})
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
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(Component.For<Operations>()
					.CustomDependencies(new
					{
						number = 42,
						serviceModel = new WcfServiceModel()
							.AddEndpoints(
								WcfEndpoint.ForContract<IOperations>()
									.WithBinding(new NetTcpBinding())
									.At("net.tcp://localhost/Operations"),
								WcfEndpoint.ForContract<IOperationsEx>()
									.WithBinding(new BasicHttpBinding())
									.At("http://localhost:27198/UsingWindsor.svc")

								)
					})
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding(), new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				IOperationsEx clientEx = ChannelFactory<IOperationsEx>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc"));
				clientEx.Backup();
			}
		}

		[Test]
		public void CanCreateServiceHostAndOpenHostWithRelativeEndpoints()
		{
			using (new WindsorContainer()
				.AddFacility("wcf_facility", new WcfFacility())
				.Register(Component.For<Operations>()
					.CustomDependencies(new
					{
						number = 42,
						serviceModel = new WcfServiceModel()
							.AddBaseAddresses(
								"net.tcp://localhost/Operations",
								"http://localhost:27198/UsingWindsor.svc")
							.AddEndpoints(
								WcfEndpoint.ForContract<IOperations>()
									.WithBinding(new NetTcpBinding()),
								WcfEndpoint.ForContract<IOperationsEx>()
									.WithBinding(new BasicHttpBinding())
									.At("Extended")
								)
					})
				))
			{
				IOperations client = ChannelFactory<IOperations>.CreateChannel(
					new NetTcpBinding(), new EndpointAddress("net.tcp://localhost/Operations"));
				Assert.AreEqual(42, client.GetValueFromConstructor());

				IOperationsEx clientEx = ChannelFactory<IOperationsEx>.CreateChannel(
					new BasicHttpBinding(), new EndpointAddress("http://localhost:27198/UsingWindsor.svc/Extended"));
				clientEx.Backup();
			}
		}
	}
}