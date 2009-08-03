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

namespace Castle.Facilities.WcfIntegration.Tests.Duplex
{
	using System.Collections;
	using System.ServiceModel;
	using Castle.Facilities.WcfIntegration.Tests.Components;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class DuplexClientFixture
	{
		private IWindsorContainer windsorContainer;

		#region Setup/Teardown

		[SetUp]
		public void TestInitialize()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("number", 42);

			windsorContainer = new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<IServiceWithCallback>()
					.ImplementedBy<ServiceWithCallback>()
					.DependsOn(parameters)
					.ActAs(new DefaultServiceModel().AddEndpoints(
							WcfEndpoint.BoundTo(new NetTcpBinding())
								.At("net.tcp://localhost/ServiceWithCallback")
							   )
				));
		}

		[TearDown]
		public void TestCleanup()
		{
			windsorContainer.Dispose();
		}

		#endregion

		[Test]
		public void CanCreateDuplexProxyAndHandleCallback()
		{
			CallbackService callbackService = new CallbackService();

			IWindsorContainer localContainer = new WindsorContainer();

			localContainer.AddFacility<WcfFacility>();

			DuplexClientModel model = new DuplexClientModel
			{
				Endpoint = WcfEndpoint.ForContract<IServiceWithCallback>()
					.BoundTo(new NetTcpBinding())
					.At("net.tcp://localhost/ServiceWithCallback")
			}.Callback(callbackService);

		    localContainer.Register(WcfClient.ForChannels(model));

			IServiceWithCallback proxy = localContainer.Resolve<IServiceWithCallback>();
			proxy.DoSomething(21);

			Assert.AreEqual(42, callbackService.ValueFromTheOtherSide);
		}

		[Test]
		public void CanCallDuplexChannelAsynchronously()
		{
			CallbackService callbackService = new CallbackService();

			IWindsorContainer localContainer = new WindsorContainer();

			localContainer.AddFacility<WcfFacility>();

			DuplexClientModel model = new DuplexClientModel
			{
				Endpoint = WcfEndpoint.ForContract<IServiceWithCallback>()
					.BoundTo(new NetTcpBinding())
					.At("net.tcp://localhost/ServiceWithCallback")
			}.Callback(callbackService);

			localContainer.Register(WcfClient.ForChannels(model));

			IServiceWithCallback proxy = localContainer.Resolve<IServiceWithCallback>();
			proxy.BeginWcfCall(p => p.DoSomething(21)).End();

			Assert.AreEqual(42, callbackService.ValueFromTheOtherSide);
		}

		[Test, Ignore("Not implemented yet")]
		public void CanCreateDuplexProxyAndHandleCallbackAsynchronously()
		{
			CallbackService callbackService = new CallbackService();

			IWindsorContainer localContainer = new WindsorContainer();

			localContainer.AddFacility<WcfFacility>();

			DuplexClientModel model = new DuplexClientModel
			{
				Endpoint = WcfEndpoint.ForContract<IServiceWithCallback>()
					.BoundTo(new NetTcpBinding())
					.At("net.tcp://localhost/ServiceWithCallback")
			}.Callback(callbackService);

			localContainer.Register(WcfClient.ForChannels(model));

			IServiceWithCallback proxy = localContainer.Resolve<IServiceWithCallback>();
			proxy.DoSomethingElse(21);

			Assert.AreEqual(84, callbackService.ValueFromTheOtherSide);
		}
	}
}