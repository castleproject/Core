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
	using Framework;
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class VerifyModelsAgainstDBSchemaTestCase : AbstractActiveRecordTest
	{
		[Test, ExpectedExceptionAttribute(typeof(ActiveRecordException), ExpectedMessage = "Error verifying the schema for model Post")]
		public void VerificationOnWithMissingTableError()
		{
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.VerifyModelsAgainstDBSchema = true;

			ActiveRecordStarter.Initialize(config, typeof(Post));
		}

		[Test,
		 ExpectedExceptionAttribute(typeof(ActiveRecordException),
			ExpectedMessage = "Error verifying the schema for model ModelClassWithBrokenField")]
		public void VerificationOnWithMissingFieldError()
		{
			// Create the tables first
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.VerifyModelsAgainstDBSchema = false;
			ActiveRecordStarter.Initialize(config, typeof(ModelClassUsedToCreateTableForClassWithBrokenField));
			Recreate();

			// Then test Broken field
			config.VerifyModelsAgainstDBSchema = true;

			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(config, typeof(ModelClassWithBrokenField));
		}

		[Test]
		public void VerificationOnWithNoError()
		{
			// Create the tables first
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.VerifyModelsAgainstDBSchema = false;
			ActiveRecordStarter.Initialize(config, typeof(Blog), typeof(Post));
			Recreate();

			// Then run again with verification
			config.VerifyModelsAgainstDBSchema = true;
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(config, typeof(Blog), typeof(Post));
		}

		[Test]
		public void VerificationOffWithMissingTableError()
		{
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.VerifyModelsAgainstDBSchema = false;

			ActiveRecordStarter.Initialize(config, typeof(Post));
		}

		[Test]
		public void VerificationOffWithMissingFieldError()
		{
			// Create the tables first
			XmlConfigurationSource config = (XmlConfigurationSource) GetConfigSource();
			config.VerifyModelsAgainstDBSchema = false;
			ActiveRecordStarter.Initialize(config, typeof(ModelClassUsedToCreateTableForClassWithBrokenField));
			Recreate();

			// Then test Broken field
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(config, typeof(ModelClassWithBrokenField));
		}
	}
}