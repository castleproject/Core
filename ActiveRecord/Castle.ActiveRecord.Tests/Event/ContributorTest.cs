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

namespace Castle.ActiveRecord.Tests.Event
{
	using NUnit.Framework;
	using NHibernate.Event;
	using Castle.ActiveRecord.Framework;
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord.Tests.Model;
	using NHibernate.Cfg;

	[TestFixture]
	public class ContributorTest : AbstractActiveRecordTest
	{
		[Test]
		public void Appliability_is_tested() 
		{
			var contributor = new MockContributor(true);
			ActiveRecordStarter.AddContributor(contributor);
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Assert.IsTrue(contributor.Tested);
		}
	
		[Test]
		public void Contributor_gets_called()
		{
			var contributor = new MockContributor(true);
			ActiveRecordStarter.AddContributor(contributor);
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Assert.IsTrue(contributor.Called);
		}

		[Test]
		public void Contributor_that_doesnt_apply_is_not_called()
		{
			var contributor = new MockContributor(false);
			ActiveRecordStarter.AddContributor(contributor);
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Assert.IsTrue(contributor.Tested);
			Assert.IsFalse(contributor.Called);
		}
		
		private class MockContributor : AbstractNHContributor
		{
			public MockContributor(bool isAppliable)
			{
				AppliesToRootType = ((t) => { Tested = true; return isAppliable; });
			}

			public override void Contribute(Configuration configuration)
			{
				Called = true;
			}

			public Boolean Called { get; private set; }
			public Boolean Tested { get; private set; }
		}
	}
}
