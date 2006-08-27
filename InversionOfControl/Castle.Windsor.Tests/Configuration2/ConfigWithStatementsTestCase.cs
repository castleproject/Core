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

namespace Castle.Windsor.Tests.Configuration2
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.Core.Configuration;

	using Castle.MicroKernel;		

	[TestFixture]
	public class ConfigWithStatementsTestCase
	{
		String dir = "../Castle.Windsor.Tests/Configuration2/";

		private IWindsorContainer container;

		[Test]
		public void SimpleIf()
		{
			String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
				"config_with_if_stmt.xml");

			container = new WindsorContainer(file);

			IConfigurationStore store = container.Kernel.ConfigurationStore;

			Assert.AreEqual(4, store.GetComponents().Length);

			IConfiguration config = store.GetComponentConfiguration("debug");
			Assert.IsNotNull(config);

			IConfiguration childItem = config.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("some value", childItem.Value);

			childItem = config.Children["item2"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("some <&> value2", childItem.Value);

			config = store.GetComponentConfiguration("qa");
			Assert.IsNotNull(config);

			config = store.GetComponentConfiguration("default");
			Assert.IsNotNull(config);

			config = store.GetComponentConfiguration("notprod");
			Assert.IsNotNull(config);
		}

		[Test]
		public void SimpleChoose()
		{
			foreach(string flag in new string[]{ "debug", "prod", "qa", "default" } )
			{
				String file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + 
					"config_with_define_" + flag + ".xml");

				container = new WindsorContainer(file);

				IConfigurationStore store = container.Kernel.ConfigurationStore;

				Assert.AreEqual(1, store.GetComponents().Length);

				IConfiguration config = store.GetComponentConfiguration(flag);

				Assert.IsNotNull(config);				
			}
		}
	}
}
