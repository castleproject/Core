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

namespace Castle.MonoRail.ActiveRecordSupport.Tests
{
	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;

	[TestFixture]
	public class ARFetchTestCase : AbstractMRTestCase
	{
		public override void FixtureInitialize()
		{
			base.FixtureInitialize();

			new ARDataBinderTestCase().CreateAndPopulateTables();
		}

		[Test]
		public void SimpleAutoLoad()
		{
			string[] args;
			
			args = new string[] {
				"id=1",
				"name=John",
				"age=99",
			};
			
			DoGet("ARFetchTest/SavePerson.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:99]");
		}
				
		[Test]
		public void ArrayAutoLoad()
		{
			string[] args = new string[] {
				"id=1",
				"id=2",
				"id=16"
			};
			
			DoGet("ARFetchTest/SavePeople.rails", args);
			AssertReplyStartsWith("Length=3\n[1:John:99]");
			AssertSuccess();

// TODO: Check why random results returned by the page
//			AssertReplyContains("[1:Name 1:1]");
//			AssertReplyContains("[2:Name 2:2]");
//			AssertReplyContains("[0::0]");
		}	
	}
}
