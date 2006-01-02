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

namespace Castle.Windsor.Tests
{
	using System;

	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Model.Configuration;
	using Castle.Windsor.Tests.Components;

	using NUnit.Framework;


	[TestFixture]
	public class AsyncInitializationContainerTestCase
	{
		[Test]
		public void OperationsLocked()
		{
			IConfigurationStore configurationStore = new DefaultConfigurationStore();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "slow";
			facNode.Attributes["type"] = "Castle.Windsor.Tests.Facilities.SlowlyInitFacility, Castle.Windsor.Tests";

			configurationStore.AddFacilityConfiguration("slow", facNode);

			MutableConfiguration compNode = new MutableConfiguration("component");
			
			compNode.Attributes["id"] = "a";
			compNode.Attributes["type"] = "Castle.Windsor.Tests.Components.CalculatorService, Castle.Windsor.Tests";

			configurationStore.AddComponentConfiguration("a", compNode);

			AsyncInitializationContainer container = new AsyncInitializationContainer(configurationStore);

			Assert.AreEqual(1, container.Kernel.GraphNodes.Length);
			Assert.AreEqual(1, container.Kernel.GraphNodes.Length);

			CalculatorService service = (CalculatorService) 
				container[typeof(CalculatorService)];
			Assert.IsNotNull(service);
			service = (CalculatorService) 
				container[typeof(CalculatorService)];
		}

		[Test]
		[ExpectedException(typeof(InitializationException), "The initialization of the container threw an exception")]
		public void ExceptionBeingSaved()
		{
			IConfigurationStore configurationStore = new DefaultConfigurationStore();

			MutableConfiguration facNode = new MutableConfiguration("facility");
			facNode.Attributes["id"] = "slow";
			facNode.Attributes["type"] = "Castle.Windsor.Tests.Facilities.IDontExist, Castle.Windsor.Tests";

			configurationStore.AddFacilityConfiguration("slow", facNode);

			AsyncInitializationContainer container = new AsyncInitializationContainer(configurationStore);

			Assert.AreEqual(1, container.Kernel.GraphNodes.Length);
		}
	}
}
