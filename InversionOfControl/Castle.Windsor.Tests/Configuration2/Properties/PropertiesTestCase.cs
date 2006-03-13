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

namespace Castle.Windsor.Tests.Configuration2.Properties
{
	using System;
	using System.Configuration;
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
		public void SilentProperties()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_silent_properties.xml");

			container = new WindsorContainer(file);

			IConfigurationStore store = container.Kernel.ConfigurationStore;

			Assert.AreEqual(1, store.GetFacilities().Length, "Diff num of facilities");
			Assert.AreEqual(1, store.GetComponents().Length, "Diff num of components");

			IConfiguration config = store.GetFacilityConfiguration("facility1");
			IConfiguration childItem = config.Children["param1"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);
			Assert.AreEqual("", childItem.Attributes["attr"]);

			config = store.GetComponentConfiguration("component1");
			childItem = config.Children["param1"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual(null, childItem.Value);
			Assert.AreEqual("prop1 value", childItem.Attributes["attr"]);
		}

		[Test]
		public void MissingProperties()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_missing_properties.xml");

			try
			{
				container = new WindsorContainer(file);	
				Assert.Fail("MissingProperties should had throw config exception");
			}
			catch(ConfigurationException c)
			{				
			}
		}

		[Test]
		public void PropertiesWithinProperties()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"properties_using_properties.xml");

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

		[Test]
		public void PropertiesAndDefines()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_properties_and_defines.xml");

			container = new WindsorContainer(file);

			AssertConfiguration();
		}

		[Test]
		public void PropertiesAndDefines2()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_properties_and_defines2.xml");

			container = new WindsorContainer(file);

			AssertConfiguration();
		}

		private void AssertConfiguration()
		{
			IConfigurationStore store = container.Kernel.ConfigurationStore;

			Assert.AreEqual(3, store.GetFacilities().Length, "Diff num of facilities");
			Assert.AreEqual(2, store.GetComponents().Length, "Diff num of components");

			IConfiguration config = store.GetFacilityConfiguration("facility1");
			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);

			config = store.GetFacilityConfiguration("facility2");
			Assert.IsNotNull(config);
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop2 value", childItem.Attributes["value"]);
			Assert.IsNull(childItem.Value);

			config = store.GetFacilityConfiguration("facility3");
			Assert.IsNotNull(config);
			Assert.AreEqual( 3, config.Children.Count, "facility3 should have 3 children" );

			childItem = config.Children["param1"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop2 value", childItem.Value);
			Assert.AreEqual("prop1 value", childItem.Attributes["attr"]);

			childItem = config.Children["param2"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);
			Assert.AreEqual("prop2 value", childItem.Attributes["attr"]);

			childItem = config.Children["param3"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("param3 attr", childItem.Attributes["attr"]);

			childItem = childItem.Children["value"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("param3 value", childItem.Value);
			Assert.AreEqual("param3 value attr", childItem.Attributes["attr"]);

			config = store.GetComponentConfiguration("component1");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop1 value", childItem.Value);

			config = store.GetComponentConfiguration("component2");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("prop2 value", childItem.Attributes["value"]);
		}
	}
}
