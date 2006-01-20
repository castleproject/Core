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
	public class ARSmartDispatchControllerTestCase : AbstractMRTestCase
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
				"SimplePerson.Id=1",
				"SimplePerson.Name=John"
			};
			
			DoGet("ARDataBinderTest/SavePerson.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:1]");

			new ARDataBinderTestCase().CreateAndPopulateTables();
			
			args = new string[] {
						"SimplePerson@autoload=no",
						"SimplePerson.Id=1",
						"SimplePerson.Name=John"
		    };
			
			DoGet("ARDataBinderTest/SavePerson.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:0]");
		}
				
		[Test]
		public void ArrayAutoLoad()
		{
			string[] args = new string[] {
				"SimplePerson[0]@autoload=yes",
				"SimplePerson[0].Id=1",
				"SimplePerson[0].Name=John",
				
				"SimplePerson[1]@autoload=yes",
				"SimplePerson[1].Id=2",
				"SimplePerson[1].Age=20",
				
				"SimplePerson[2]@autoload=yes",
				"SimplePerson[2].Id=3",

				"SimplePerson[3]@autoload=no",
				"SimplePerson[3].Id=16",
				"SimplePerson[3].Name=Julio",
				"SimplePerson[3].Age=16"
			};
			
			DoGet("ARDataBinderTest/SavePeople.rails", args);
			AssertSuccess();
			AssertReplyContains("[1:John:1]");
			AssertReplyContains("[2:Name 2:20]");
			AssertReplyContains("[3:Name 3:3]");
			AssertReplyContains("[16:Julio:16]");			
		}	
	}
}
