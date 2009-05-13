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
	using Framework.Config;
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class EnumModelTestCase : AbstractActiveRecordTest 
	{
		[SetUp]
		public override void Init() {
			base.Init();
			
			XmlConfigurationSource config = (XmlConfigurationSource)GetConfigSource();

			ActiveRecordStarter.Initialize(config, typeof(EnumModel));
			ActiveRecordStarter.CreateSchema();
		}

		[Test]
		public void SaveAndQueryEnumModel()
		{
			EnumModel model = new EnumModel();
			model.Roles.Add(Role.Admin);
			model.Roles.Add(Role.Admin);
			model.Roles.Add(Role.User);

			ActiveRecordMediator.SaveAndFlush(model);

			EnumModel first = ActiveRecordMediator<EnumModel>.FindFirst();

			Assert.AreEqual(first.Roles.Count, 3);
			Assert.IsTrue(first.Roles.Contains(Role.Admin));
			Assert.IsTrue(first.Roles.Contains(Role.User));
		}
	}
}