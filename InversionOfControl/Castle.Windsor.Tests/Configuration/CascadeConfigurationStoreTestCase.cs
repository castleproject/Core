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

	using Castle.Windsor.Configuration;
	using Castle.Windsor.Configuration.Interpreters;


	[TestFixture]
	public class CascadeConfigurationStoreTestCase
	{
		[Test]
		public void SimpleUsage()
		{
			CascadeConfigurationStore store = new CascadeConfigurationStore( 
				new ConfigLanguageInterpreter("ConfigContents.lang"), 
				new ConfigLanguageInterpreter("ConfigContents2.lang") );

			Assert.AreEqual(2, store.GetFacilities().Length);
			Assert.AreEqual(4, store.GetComponents().Length);
		}
	}
}
