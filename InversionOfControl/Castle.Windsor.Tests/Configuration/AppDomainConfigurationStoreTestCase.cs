// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	using Castle.Model.Configuration;
	using Castle.Windsor.Configuration.AppDomain;

	/// <summary>
	/// Summary description for AppDomainConfigurationStoreTestCase.
	/// </summary>
	[TestFixture]
	public class AppDomainConfigurationStoreTestCase
	{
		[Test]
		public void ProperDeserialization()
		{
			AppDomainConfigurationStore store = new AppDomainConfigurationStore();

			Assert.AreEqual(1, store.GetFacilities().Length);
			Assert.AreEqual(2, store.GetComponents().Length);

			IConfiguration config = store.GetFacilityConfiguration("testidengine");
			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value", childItem.Value);

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
