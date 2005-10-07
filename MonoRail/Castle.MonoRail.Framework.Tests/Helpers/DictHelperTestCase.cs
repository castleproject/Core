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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework.Helpers;
	
	using NUnit.Framework;


	[TestFixture]
	public class DictHelperTestCase
	{
		private DictHelper helper;

		[SetUp]
		public void Init()
		{
			helper = new DictHelper();
		}

		[Test]
		public void EmptyDict()
		{
			IDictionary dict = helper.CreateDict();

			Assert.IsNotNull(dict);
			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void SimpleDict()
		{
			IDictionary dict = helper.CreateDict("name=value", "other=somethingelse");

			Assert.IsNotNull(dict);
			Assert.AreEqual(2, dict.Count);

			foreach(String key in dict.Keys)
			{
				if (key.Equals("name"))
				{
					Assert.AreEqual( "value", dict["name"] );
				}
				else if (key.Equals("other"))
				{
					Assert.AreEqual( "somethingelse", dict["other"] );
				}
				else
				{
					Assert.Fail("unexpected key? " + key);
				}
			}
		}

		[Test]
		public void PushingParser()
		{
			IDictionary dict = helper.CreateDict("name=value=aa");

			Assert.IsNotNull(dict);
			Assert.AreEqual(1, dict.Count);

			foreach(String key in dict.Keys)
			{
				if (key.Equals("name"))
				{
					Assert.AreEqual( "value=aa", dict["name"] );
				}
				else
				{
					Assert.Fail("unexpected key? " + key);
				}
			}
		}
	}
}
