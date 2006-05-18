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

namespace Castle.Windsor.Tests.Configuration
{
	using Castle.Model.Resource;
	using Castle.Model.Configuration;
	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor.Configuration.Interpreters;

	using NUnit.Framework;

	[TestFixture]
	public class ConfigXmlInterpreterTestCase
	{
		[Test]
		public void ProperDeserialization()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();

			XmlInterpreter interpreter = new XmlInterpreter("../Castle.Windsor.Tests/sample_config.xml");
			interpreter.ProcessResource(interpreter.Source, store);

			Assert.AreEqual(2, store.GetFacilities().Length);
			Assert.AreEqual(2, store.GetComponents().Length);

			IConfiguration config = store.GetFacilityConfiguration("testidengine");
			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value", childItem.Value);

			config = store.GetFacilityConfiguration("testidengine2");
			Assert.IsNotNull(config);
			Assert.AreEqual("value within CDATA section", config.Value);

			config = store.GetComponentConfiguration("testidcomponent1");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value1", childItem.Value);

			config = store.GetComponentConfiguration("testidcomponent2");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value2", childItem.Value);
		}

		[Test]
		public void CorrectConfigurationMapping()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			XmlInterpreter interpreter = new XmlInterpreter("../Castle.Windsor.Tests/sample_config.xml");
			interpreter.ProcessResource(interpreter.Source, store);

			WindsorContainer container = new WindsorContainer(store);			

			container.AddFacility("testidengine", new DummyFacility());
		}

		[Test]
		public void ProperManifestDeserialization()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			XmlInterpreter interpreter = new XmlInterpreter(
				new AssemblyResource("assembly://Castle.Windsor.Tests/Configuration/sample_config.xml"));
			interpreter.ProcessResource(interpreter.Source, store);

			Assert.AreEqual(2, store.GetFacilities().Length);
			Assert.AreEqual(2, store.GetComponents().Length);

			IConfiguration config = store.GetFacilityConfiguration("testidengine");
			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value", childItem.Value);

			config = store.GetFacilityConfiguration("testidengine2");
			Assert.IsNotNull(config);
			Assert.AreEqual("value within CDATA section", config.Value);

			config = store.GetComponentConfiguration("testidcomponent1");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value1", childItem.Value);

			config = store.GetComponentConfiguration("testidcomponent2");
			childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value2", childItem.Value);;
		}

		[Test]
		[ExpectedException(typeof(ResourceException))]
		public void MissingManifestResourceConfiguration()
		{
			new XmlInterpreter(new AssemblyResource("assembly://Castle.Windsor.Tests/missing_config.xml"));
		}
	}

	public class DummyFacility : IFacility
	{
		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			Assert.IsNotNull(facilityConfig);
			IConfiguration childItem = facilityConfig.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value", childItem.Value);
		}

		public void Terminate()
		{
		}
	}
}
