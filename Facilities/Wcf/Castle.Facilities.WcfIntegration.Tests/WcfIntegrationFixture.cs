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
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Behaviors;
	using Castle.Core.Interceptor;
	using Core;
	using MicroKernel;
	using NUnit.Framework;
	using Windsor;

	[TestFixture]
	public class WcfIntegrationFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void TestInitialize()
		{
			WindsorContainer windsorContainer = new WindsorContainer();
			windsorContainer.AddComponent("logging", typeof (LoggingInterceptor));
			windsorContainer.AddComponent("call_count", typeof (IServiceBehavior), typeof (CallCountServiceBehavior));
			windsorContainer.AddComponent("operations", typeof (IOperations), typeof (Operations));
			windsorContainer.AddComponent("unit_of_work", typeof (IEndpointBehavior), typeof (UnitOfworkEndPointBehavior));

			IHandler handler = windsorContainer.Kernel.GetHandler("operations");
			handler.ComponentModel.Interceptors.Add(new InterceptorReference("logging"));
			handler.ComponentModel.Parameters.Add("number", "42");

			Uri uri = new Uri("net.tcp://localhost/WCF.Facility");
			host = new WindsorServiceHost(windsorContainer.Kernel, typeof (Operations),
			                              uri);
			EndpointAddress endpointAddress = new EndpointAddress(uri);
			host.Description.Endpoints.Add(
				new ServiceEndpoint(ContractDescription.GetContract(typeof (IOperations)),
				                    new NetTcpBinding(),
				                    endpointAddress));
			CallCountServiceBehavior.CallCount = 0;
			LoggingInterceptor.Calls.Clear();
			host.Open();
			client = ChannelFactory<IOperations>.CreateChannel(new NetTcpBinding(), endpointAddress);
		}

		[TearDown]
		public void TestCleanup()
		{
			host.Close();
		}

		#endregion

		private WindsorServiceHost host;
		private IOperations client;

		[Test]
		public void CanCallServiceAndGetValueFromWindsorConfig()
		{
			int result = client.GetValueFromConstructor();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void CanUseStandardDynamicProxyInterceptorsOnServices()
		{
			Assert.AreEqual(0, LoggingInterceptor.Calls.Count);
			client.GetValueFromConstructor();
			Assert.AreEqual(1, LoggingInterceptor.Calls.Count);
		}

		[Test]
		public void WillApplyEndPointBehaviors()
		{
			Assert.IsFalse(UnitOfWork.initialized, "Should be false before starting");
			bool unitOfWorkIsInitialized_DuringCall = client.UnitOfWorkIsInitialized();
			Assert.IsTrue(unitOfWorkIsInitialized_DuringCall);
			Assert.IsFalse(UnitOfWork.initialized, "Should be false after call");
		}

		[Test]
		public void WillApplyServiceBehaviors()
		{
			Assert.AreEqual(0, CallCountServiceBehavior.CallCount);
			client.GetValueFromConstructor();
			Assert.AreEqual(1, CallCountServiceBehavior.CallCount);
		}
	}

	public class LoggingInterceptor : IInterceptor
	{
		public static List<string> Calls = new List<string>();

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			Calls.Add(invocation.Method.Name);
			invocation.Proceed();
		}

		#endregion
	}
}