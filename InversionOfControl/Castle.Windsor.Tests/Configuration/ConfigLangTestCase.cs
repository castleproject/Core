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

namespace Castle.Windsor.Tests.Configuration
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.SubSystems.Configuration;
	
	using Castle.Model.Configuration;
	
	using Castle.Windsor.Configuration.Interpreters;


	[TestFixture]
	public class ConfigLangTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			ConfigLanguageInterpreter interpreter = new ConfigLanguageInterpreter("ConfigContents.lang");
			interpreter.Process(store);

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
	}
}
