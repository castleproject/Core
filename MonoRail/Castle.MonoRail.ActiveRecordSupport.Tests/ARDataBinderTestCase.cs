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
	using System;
	using System.Collections.Specialized;
	using System.Configuration;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Tests;
	using Castle.MonoRail.ActiveRecordSupport.Tests.Model;
	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;
	
	using TestScaffolding.Model;

	[TestFixture]
	public class ARDataBinderTestCase
	{
		private ARDataBinder binder = new ARDataBinder();
		private NameValueCollection args;
		private object instance;
		private SimplePerson person;
		private SimplePerson[] people;
			
		[SetUp]
		public void Init()
		{
			CreateAndPopulatePeopleTable();
		}
			
		[Test]
		public void AutoloadOnSuccessScenario()
		{			
			String data = @"
				SimplePerson@autoload = yes
				SimplePerson.Id = 15
				SimplePerson.Age = 200
			";
			
			args = DataBinderTestCase.ParseNameValueString(data);
			
			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", args);
			
			Assert.IsNotNull(instance);	
			person = instance as SimplePerson;
			// since we are doing auto load the name property should
			// had being populated already
			Assert.IsTrue( person.Name == "Name 15", "AutoLoad failed to load db property");
			//  (Age should had being overwritten)
			Assert.IsTrue( person.Age == 200, "AutoLoad Failed to overwrite db property");
		}
		
		[Test]
		public void AutoloadOffSuccessScenario()
		{
			String data = @"
				SimplePerson@autoload = no
				SimplePerson.Id = 15
				SimplePerson.Age = 200
			";
			
			args = DataBinderTestCase.ParseNameValueString(@data);
			
			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", args);
			
			Assert.IsNotNull(instance);	
			person = instance as SimplePerson;
			Assert.IsNull( person.Name );
			Assert.IsTrue( person.Age == 200);	
		}
		
		[Test]
		public void AutoloadOnMissingPK()
		{
			String data = @"
				SimplePerson@autoload = yes
				SimplePerson.Age = 200
			";
			
			args = DataBinderTestCase.ParseNameValueString(@data);
			
			try
			{
				instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", args);	

				Assert.Fail("Autoload should had thrown an exception, cause pk was missing");
			}
			catch(RailsException)
			{
				// Expected
			}
		}
		
		[Test]
		public void LoadingWithANonExistentId()
		{
			String data = @"
				SimplePerson@autoload = yes
				SimplePerson.Id = 5000
				SimplePerson.Age = 200
			";
			
			args = DataBinderTestCase.ParseNameValueString(@data);
			
			try
			{
				instance = binder.BindObject( typeof(SimplePerson), "SimplePerson", args);	
				
				Assert.Fail("Autoload should had thrown an exception, cause pk value was invalid");
			}
			catch(Castle.ActiveRecord.NotFoundException)
			{
				// Expected
			}
		}
		
		[Test]
		public void AutoloadWithArray()
		{
			String data = @"DisconnectedPerson@autoload = yes";
			args = DataBinderTestCase.ParseNameValueString(@data);
			
			try
			{
				instance = binder.BindObject( typeof(DisconnectedPerson), "DisconnectedPerson", args );	

				Assert.Fail("Autoload should had thrown an exception, DisconnectedPerson is not an active record class");
			}
			catch(RailsException)
			{
				// Expected
			}
		}
			
		[Test]
		public void AutoloadWithNonARClass()
		{			
			String data = @"
				SimplePerson[0]@autoload = no
				SimplePerson[0].Id = 1
				SimplePerson[0].Name = Custom Name

				SimplePerson[1]@autoload = yes
				SimplePerson[1].Id = 2
				SimplePerson[1].Age = 20
				
				SimplePerson[2]@autoload = no
				SimplePerson[2].Id = 3
				SimplePerson[2].Age = 200			
				
				SimplePerson[3]@autoload = yes
				SimplePerson[3].Id = 15
				SimplePerson[3].Name = Name Overwrite
			";
			
			args = DataBinderTestCase.ParseNameValueString(@data);
			
			instance = binder.BindObject( typeof(SimplePerson[]), "SimplePerson", args );	

			Assert.IsNotNull(instance);
			people = instance as SimplePerson[];
			Assert.IsNotNull(people);
			Assert.IsTrue( people.Length == 4 );
			
			Assert.IsTrue(people[0].Id == 1);
			Assert.IsTrue(people[0].Name == "Custom Name");
			Assert.IsTrue(people[0].Age == 0);

			Assert.IsTrue(people[1].Id == 2);
			Assert.IsTrue(people[1].Name == "Name 2");
			Assert.IsTrue(people[1].Age == 20);
			
			Assert.IsTrue(people[2].Id == 3);
			Assert.IsTrue(people[2].Name == null);
			Assert.IsTrue(people[2].Age == 200);
			
			Assert.IsTrue(people[3].Id == 15);
			Assert.IsTrue(people[3].Name == "Name Overwrite");
			Assert.IsTrue(people[3].Age == 15);															
		}
		
		public static void CreateAndPopulatePeopleTable()
		{
			ActiveRecordStarter.Initialize( 
				ConfigurationSettings.GetConfig( "activerecord" ) as IConfigurationSource, 
				typeof(SimplePerson) );

			ActiveRecordStarter.CreateSchema();

			using(new SessionScope())
			{
				for(int i=1; i <= 15; i++)
				{
					SimplePerson person = new SimplePerson();
					person.Name = "Name " + i.ToString();
					person.Age = i;
					person.Save();
				}
			}
		}		
	}
}
