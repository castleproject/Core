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
	using System.Collections.Specialized;
	using System.Configuration;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.Components.Binder;
	using Castle.MonoRail.Framework;

	using NUnit.Framework;
	
	using TestScaffolding.Model;

	[TestFixture]
	public class ARDataBinderTestCase
	{
		private ARDataBinder binder = new ARDataBinder();
		private object instance;
		private SimplePerson person;

		private Category cat1;
		private Category cat2;
		private Category cat3;
	
		[TestFixtureSetUp]
		public void Init()
		{
			CreateAndPopulateTables();
		}

		[SetUp]
		public void TestInit()
		{
			binder.AutoLoad = AutoLoadBehavior.Always;
		}
			
		[Test]
		public void AutoloadOnSuccessScenario()
		{			
			NameValueCollection args = new NameValueCollection();

			args.Add("SimplePerson@autoload", "yes");
			args.Add("SimplePerson.Id", "15");
			args.Add("SimplePerson.Age", "200");

			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
			
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
			NameValueCollection args = new NameValueCollection();

			args.Add("SimplePerson@autoload", "no");
			args.Add("SimplePerson.Id", "15");
			args.Add("SimplePerson.Age", "200");

			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
			
			Assert.IsNotNull(instance);	
			person = instance as SimplePerson;
			Assert.IsNull( person.Name );
			Assert.IsTrue( person.Age == 200);	
		}
		
		[Test, ExpectedException(typeof(RailsException))]
		public void AutoloadOnMissingPK()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("SimplePerson.Age", "200");

			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
		}
		
		[Test, ExpectedException(typeof(Castle.ActiveRecord.NotFoundException))]
		public void LoadingWithANonExistentId()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("SimplePerson.Id", "5000");
			args.Add("SimplePerson.Age", "200");

			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
		}
		
		[Test]
		public void NoLoadingWhenIdMatchNoAutoLoadWhenPrimaryKeyIs()
		{
			NameValueCollection args = new NameValueCollection();

			binder.AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey;

			args.Add("SimplePerson.Id", "0");
			args.Add("SimplePerson.Age", "200");

			instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
			
			person = instance as SimplePerson;
			
			Assert.AreEqual(0, person.Id);
			Assert.AreEqual(200, person.Age);
		}

        [Test]
        [ExpectedException(typeof(RailsException), "Could not find primary key 'Id' for 'TestScaffolding.Model.SimplePerson'")]
        public void ErrorWhenAutoLoadIsTrueButNoIdSpecified()
        {
            NameValueCollection args = new NameValueCollection();

            binder.AutoLoad = AutoLoadBehavior.Always;

            args.Add("SimplePerson.Id", "");

            instance = binder.BindObject(typeof(SimplePerson), "SimplePerson", new NameValueCollectionAdapter(args));
        }
		
		[Test]
		public void AutoloadAndArray()
		{			
			NameValueCollection args = new NameValueCollection();

			args.Add("SimplePerson[0]@autoload", "no");
			args.Add("SimplePerson[0].Id", "1");
			args.Add("SimplePerson[0].Name", "Custom Name");
			args.Add("SimplePerson[1]@autoload", "yes");
			args.Add("SimplePerson[1].Id", "2");
			args.Add("SimplePerson[1].Age", "20");
			args.Add("SimplePerson[2]@autoload", "no");
			args.Add("SimplePerson[2].Id", "3");
			args.Add("SimplePerson[2].Age", "200");
			args.Add("SimplePerson[3]@autoload", "yes");
			args.Add("SimplePerson[3].Id", "15");
			args.Add("SimplePerson[3].Name", "Name overriden");

			instance = binder.BindObject( typeof(SimplePerson[]), "SimplePerson", new NameValueCollectionAdapter(args) );	

			Assert.IsNotNull(instance);
			SimplePerson[] people = instance as SimplePerson[];
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
			Assert.IsTrue(people[3].Name == "Name overriden");
			Assert.IsTrue(people[3].Age == 15);															
		}

		[Test]
		[Ignore("Create schema does not create the correct tables")]
		public void PopulatingContainers()
		{
			binder.AutoLoad = AutoLoadBehavior.Never;

			NameValueCollection args = new NameValueCollection();

			args.Add("blog.name", "my blog");
			args.Add("blog.author", "hammett");
			args.Add("blog.categories.id", cat1.Id.ToString());
			args.Add("blog.categories.id", cat2.Id.ToString());

			Blog instance = (Blog) binder.BindObject(
				typeof(Blog), "blog", new NameValueCollectionAdapter(args));
			
			Assert.IsNotNull(instance);	
			Assert.AreEqual( "my blog", instance.Name );
			Assert.AreEqual( "hammett", instance.Author );
			Assert.IsNotNull(instance.Categories);
			Assert.AreEqual(2, instance.Categories.Count);
		}

		[Test]
		[Ignore("Create schema does not create the correct tables")]
		public void PopulatingContainersWithArray()
		{
			binder.AutoLoad = AutoLoadBehavior.Never;

			NameValueCollection args = new NameValueCollection();

			args.Add("blog.name", "my blog");
			args.Add("blog.author", "hammett");
			args.Add("blog.categories[0].id", cat1.Id.ToString());
			args.Add("blog.categories[1].id", cat2.Id.ToString());

			Blog instance = (Blog) binder.BindObject(
				typeof(Blog), "blog", new NameValueCollectionAdapter(args));
			
			Assert.IsNotNull(instance);	
			Assert.AreEqual( "my blog", instance.Name );
			Assert.AreEqual( "hammett", instance.Author );
			Assert.IsNotNull(instance.Categories);
			Assert.AreEqual(2, instance.Categories.Count);
		}

		public void CreateAndPopulateTables()
		{
			ActiveRecordStarter.Initialize( 
				ConfigurationSettings.GetConfig( "activerecord" ) as IConfigurationSource, 
				typeof(SimplePerson), typeof(Blog), typeof(Category) /*, typeof(BlogCategory)*/ );

			// try { ActiveRecordStarter.DropSchema(); } catch(Exception) {}

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

				cat1 = new Category("Technical");
				cat2 = new Category("Political");
				cat3 = new Category("General");

				cat1.Save();
				cat2.Save();
				cat3.Save();
			}
		}
	}
}
