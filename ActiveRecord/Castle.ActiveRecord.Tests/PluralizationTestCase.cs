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

namespace Castle.ActiveRecord.Tests
{
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Tests.Model;
	using NUnit.Framework;

	[TestFixture]
	public class PluralizationTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void PluralizationOn()
		{
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.PluralizeTableNames = true;

			ActiveRecordStarter.Initialize(config, typeof(Post), typeof(Blog), typeof(Snippet), typeof(Octopus));

			Assert.AreEqual("PostTable", ActiveRecordModel.GetModel(typeof(Post)).ActiveRecordAtt.Table);
			Assert.AreEqual("BlogTable", ActiveRecordModel.GetModel(typeof(Blog)).ActiveRecordAtt.Table);
			Assert.AreEqual("Snippets", ActiveRecordModel.GetModel(typeof(Snippet)).ActiveRecordAtt.Table);
			Assert.AreEqual("Octopi", ActiveRecordModel.GetModel(typeof(Octopus)).ActiveRecordAtt.Table);
		}

		[Test]
		public void PluralizationOff()
		{
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.PluralizeTableNames = false;

			ActiveRecordStarter.Initialize(config, typeof(Post), typeof(Blog), typeof(Snippet), typeof(Octopus));

			Assert.AreEqual("PostTable", ActiveRecordModel.GetModel(typeof(Post)).ActiveRecordAtt.Table);
			Assert.AreEqual("BlogTable", ActiveRecordModel.GetModel(typeof(Blog)).ActiveRecordAtt.Table);
			Assert.AreEqual("Snippet", ActiveRecordModel.GetModel(typeof(Snippet)).ActiveRecordAtt.Table);
			Assert.AreEqual("Octopus", ActiveRecordModel.GetModel(typeof(Octopus)).ActiveRecordAtt.Table);
		}
	}
}
