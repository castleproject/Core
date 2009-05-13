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
namespace Castle.Windsor.Tests.Proxy
{
	using System;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Core.Interceptor;
	using Castle.Facilities.FactorySupport;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class FactorySupportTestCase
	{
		private WindsorContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new WindsorContainer();
		}

		[Test]
		public void FactorySupport_UsingProxiedFactory_WorksFine()
		{
			container.AddFacility("factories", new FactorySupportFacility());
			container.AddComponent("standard.interceptor", typeof(StandardInterceptor));
			container.AddComponent("factory", typeof(CalulcatorFactory));

			AddComponent("calculator", typeof(ICalcService), typeof(CalculatorService), "Create");

			ICalcService service = (ICalcService) container["calculator"];

			Assert.IsNotNull(service);
		}

		private void AddComponent(string key, Type service, Type type, string factoryMethod)
		{
			MutableConfiguration config = new MutableConfiguration(key);
			config.Attributes["factoryId"] = "factory";
			config.Attributes["factoryCreate"] = factoryMethod;
			container.Kernel.ConfigurationStore.AddComponentConfiguration(key, config);
			container.Kernel.AddComponent(key, service, type);
		}
		
		[Interceptor(typeof(StandardInterceptor))]
		public class CalulcatorFactory
		{
			public virtual ICalcService Create()
			{
				return new CalculatorService();
			}
		}
	}
}
