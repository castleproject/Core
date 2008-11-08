// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
#region

using Castle.Core.Configuration;
using Castle.Facilities.NHibernateIntegration.Internal;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;
using NUnit.Framework;

#endregion

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Issue106
{
	[TestFixture]
	public class Fixture : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "DummyConfig.xml";
			}
		}
		[Test]
		public void CanReadNHConfigFileAsTheSourceOfSessionFactory()
		{
			IConfiguration castleConfiguration = new MutableConfiguration("myConfig");
			castleConfiguration.Attributes["nhibernateConfigFile"] = ConfigHelper.ResolvePath("Issues/Issue106/factory1.xml");
			XmlConfigurationBuilder b=new XmlConfigurationBuilder();
			NHibernate.Cfg.Configuration cfg = b.GetConfiguration(castleConfiguration);
			Assert.IsNotNull(cfg);
			string str=cfg.Properties["connection.provider"];
			Assert.AreEqual("DummyProvider",str);

		}
	}
}
