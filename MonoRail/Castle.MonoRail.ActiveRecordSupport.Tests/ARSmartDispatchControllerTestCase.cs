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

namespace Castle.MonoRail.ActiveRecordSupport.Tests
{
	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;

	[TestFixture]
	public class ARSmartDispatchControllerTestCase : AbstractMRTestCase
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();

			ARDataBinderTestCase.CreateAndPopulatePeopleTable();
		}

		[Test]
		public void SimpleAutoLoad()
		{
			string[] args;
			
			args = new string[] {
				"SimplePerson@autoload=yes",
				"SimplePerson.Id=1",
				"SimplePerson.Name=John"
			};
			
			DoGet("ARDataBinderTest/SavePerson.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:1]");

			ARDataBinderTestCase.CreateAndPopulatePeopleTable();
			
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
		public void SimpleNoPrefixAutoLoad()
		{
			string[] args;

			args = new string[] {
									"@autoload=yes",
									"Id=1",
									"Name=John"
								};
			
			DoGet("ARDataBinderTest/SavePersonNoPrefix.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:1]");

			ARDataBinderTestCase.CreateAndPopulatePeopleTable();
			
			args = new string[] {
									"@autoload=no",
									"Id=1",
									"Name=John"
								};
			
			DoGet("ARDataBinderTest/SavePersonNoPrefix.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:0]");
		}
		
		[Test]
		public void SimpleAutoLoadWithValidate()
		{
			string[] args;
			
			args = new string[] {
									"SimplePerson@autoload=yes",
									"SimplePerson.Id=1",
									"SimplePerson.Name=John"
								};
			
			DoGet("ARDataBinderTest/SavePersonWithValidate.rails", args);
			AssertSuccess();
			AssertReplyStartsWith("[1:John:1]");
		}				

		[Test]
		public void SimpleAutoLoadWithValidateBadData()
		{
			string[] args;
			
			args = new string[] {
									"SimplePerson@autoload=no",
									"SimplePerson.Id=1",
									"SimplePerson.Name=John??"
								};
			
			DoGet("ARDataBinderTest/SavePersonWithValidate.rails", args);
			AssertReplyContains("Error Validating Attribute");
		}				

		[Test]
		public void ArrayAutoLoadNoPrefix()
		{
			string[] args;
			
			args = new string[] {
									"[0]@autoload=yes",
									"[0].Id=1",
									"[0].Name=John",
									
									"[1]@autoload=yes",
									"[1].Id=2",
									"[1].Age=20",
									
									"[2]@autoload=yes",
									"[2].Id=3",

									"[3]@autoload=no",
									"[3].Id=16",
									"[3].Name=Julio",
									"[3].Age=16"
								};
			
			DoGet("ARDataBinderTest/SavePeopleNoPrefix.rails", args);
			AssertSuccess();
			AssertReplyContains("[1:John:1]");
			AssertReplyContains("[2:Name 2:20]");
			AssertReplyContains("[3:Name 3:3]");
			AssertReplyContains("[16:Julio:16]");			
		}	
				
		[Test]
		public void ArrayAutoLoad()
		{
			string[] args;
			
			args = new string[] {
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
		
		[Test]
		public void ArrayAutoLoadWithValidate()
		{
			string[] args;
			
			args = new string[] {
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
			
			DoGet("ARDataBinderTest/SavePeopleWithValidate.rails", args);
			AssertSuccess();
			AssertReplyContains("[1:John:1]");
			AssertReplyContains("[2:Name 2:20]");
			AssertReplyContains("[3:Name 3:3]");
			AssertReplyContains("[16:Julio:16]");			
		}				

		[Test]
		public void ArrayAutoLoadWithValidateBadData()
		{
			string[] args;
			
			args = new string[] {
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
									"SimplePerson[3].Name=Julio??",
									"SimplePerson[3].Age=16"
								};
			
			DoGet("ARDataBinderTest/SavePeopleWithValidate.rails", args);
			AssertReplyContains("Error Validating Attribute");
		}				
	}
}
