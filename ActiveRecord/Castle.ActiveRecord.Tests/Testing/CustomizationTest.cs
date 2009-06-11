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

namespace Castle.ActiveRecord.Tests.Testing
{
	using System;
	using System.Linq;

	using NUnit.Framework;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Testing;
	using Castle.ActiveRecord.Tests.Model;
using NHibernate;


	[TestFixture]
	public class CustomizationTest : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new[] { typeof(Blog), typeof(Post) };
		}

		public override void Configure(InPlaceConfigurationSource config)
		{
			config.DefaultFlushType = DefaultFlushType.Leave;
		}

		[Test]
		public void ConfigurationIsCustomizable()
		{
			using (new SessionScope())
			{
				Blog.FindAll();
				Assert.AreEqual(FlushMode.Commit, Blog.Holder.CreateSession(typeof(Blog)).FlushMode);
			}
		}
	}
}
