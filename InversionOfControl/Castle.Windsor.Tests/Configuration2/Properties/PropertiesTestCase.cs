// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests.Configuration2.Properties
{
	using System;
	using System.IO;
	
	using Castle.MicroKernel;
	using Castle.Model.Configuration;
	
	using NUnit.Framework;

	[TestFixture]
	public class PropertiesTestCase
	{
		String dir = "../Castle.Windsor.Tests/Configuration2/Properties/";

		private IWindsorContainer container;

		[Test]
		public void CorrectEval()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_properties.xml");

			container = new WindsorContainer(file);

			AssertConfiguration();
		}

		[Test]
		public void PropertiesAndIncludes()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_properties_and_includes.xml");

			container = new WindsorContainer(file);

			AssertConfiguration();
		}

		private void AssertConfiguration()
		{
			IConfigurationStore store = container.Kernel.ConfigurationStore;

			Assert.AreEqual(2, store.GetFacilities().Length);
			Assert.AreEqual(2, store.GetComponents().Length);

			IConfiguration config = store.GetFacilityConfiguration("testidengine");
			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);

			config = store.GetFacilityConfiguration("testidengine2");
			Assert.IsNotNull(config);
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop2 value", childItem.Attributes["value"]);

			config = store.GetComponentConfiguration("testidcomponent1");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);

			config = store.GetComponentConfiguration("testidcomponent2");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop2 value", childItem.Attributes["value"]);
		}
	}
}
