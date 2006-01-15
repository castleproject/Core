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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Text;
	using System.Threading;

	using NUnit.Framework;
	
	[TestFixture]
	public class DataBinderTestCase
	{
		private static TimeSpan init;

		[TestFixtureSetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			init = new TimeSpan(DateTime.Now.Ticks);
		}

		[TestFixtureTearDown]
		public void Terminate()
		{
			TimeSpan diff = new TimeSpan(DateTime.Now.Ticks) - init;

			Console.WriteLine(diff.Milliseconds);
		}

		[Test]
		public void DateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 1.ToString());
			args.Add("person.DOBmonth", 12.ToString());
			args.Add("person.DOByear", 2005.ToString());

			DataBinder binder = new DataBinder("person");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person p = (Person) instance;
			Assert.AreEqual(p.DOB, new DateTime(2005, 12, 1));
		}

		[Test]
		public void Ignoring1()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person@ignore", "true");
			args.Add("person.name", "john");

			DataBinder binder = new DataBinder("person");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person person = (Person) instance;
			Assert.IsNull(person.Name);
		}

		[Test]
		public void DayTooBigDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 31.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			DataBinder binder = new DataBinder("person");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person person = (Person) instance;
			Assert.AreEqual(1, binder.Errors.Count);
		}

		[Test]
		public void InvalidDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", 32.ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			DataBinder binder = new DataBinder("person");
			binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));
			Assert.AreEqual(1, binder.Errors.Count);
		}

		[Test]
		public void InvalidDateTime2Bind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("person.DOBday", (-2).ToString());
			args.Add("person.DOBmonth", 2.ToString());
			args.Add("person.DOByear", 2005.ToString());

			DataBinder binder = new DataBinder("person");
			binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));
			Assert.AreEqual(1, binder.Errors.Count);
		}

		[Test]
		public void SimpleDataBind()
		{
			String name = "John";
			int age = 32;
			decimal assets = (decimal) 100000;
			NameValueCollection args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString());
			DataBinder binder = new DataBinder("Person");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void SimpleDataBind2()
		{
			string name = "John";
			int age = 32;
			NameValueCollection args = new NameValueCollection();

			args.Add("abc.Name", name);
			args.Add("abc.Age", age.ToString());
			DataBinder binder = new DataBinder("abc");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
		}

		[Test]
		public void PrimitiveArrayDataBind()
		{
			string scores = "1,2,3,4,5";
			string opponents = "Sao Paulo,Santos,Palmeiras,MogiMirim,Portuguesa";
			NameValueCollection args = new NameValueCollection();
			args.Add("Game.Scores", scores);
			args.Add("Game.Opponents", opponents);
			DataBinder binder = new DataBinder("Game");
			object instance = binder.BindObject(typeof(Game), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Game game = instance as Game;
			Assert.IsNotNull(game);
			Assert.AreEqual(game.Scores.Length, 5);
			Assert.AreEqual(game.Opponents.Length, 5);
			Assert.AreEqual(Join(",", game.Scores), scores);
			Assert.AreEqual(string.Join(",", game.Opponents), opponents);
		}

		[Test]
		public void SimpleArrayDataBind()
		{
			string data = @" 
				Person[0].Name = John
				Person[0].Age  = 32
				Person[1].Name = Mary
				Person[1].Age  = 16
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);
			DataBinder binder = new DataBinder("Person");
			object instance = binder.BindObject(typeof(Person[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person[] sc = instance as Person[];
			Assert.IsNotNull(sc);
			Assert.IsTrue(sc.Length == 2);
			Assert.AreEqual(sc[0].Age, 32);
			Assert.AreEqual(sc[0].Name, "John");
			Assert.AreEqual(sc[1].Age, 16);
			Assert.AreEqual(sc[1].Name, "Mary");
		}

		[Test]
		public void IgnoringArrayDataBind()
		{
			string data = @" 
				Person[0]@ignore = true
				Person[0].Name = John
				Person[0].Age  = 32
				Person[1]@ignore = true
				Person[1].Name = Mary
				Person[1].Age  = 16
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);
			DataBinder binder = new DataBinder("Person");
			object instance = binder.BindObject(typeof(Person[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person[] sc = instance as Person[];
			Assert.IsNotNull(sc);
			Assert.IsTrue(sc.Length == 0);
		}

		[Test]
		public void SimpleArrayDataBindWithPrefix()
		{
			string data = @" 
				abc[0].Name = John
				abc[0].Age  = 32
				abc[1].Name = Mary
				abc[1].Age  = 16
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);
			DataBinder binder = new DataBinder("abc");
			object instance = binder.BindObject(typeof(Person[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person[] sc = instance as Person[];
			Assert.IsNotNull(sc);
			Assert.IsTrue(sc.Length == 2);
			Assert.AreEqual(sc[0].Age, 32);
			Assert.AreEqual(sc[0].Name, "John");
			Assert.AreEqual(sc[1].Age, 16);
			Assert.AreEqual(sc[1].Name, "Mary");
		}

		[Test]
		public void IgnoreAttributeDataBind()
		{
			Team[] team = null;
			
			DataBinder binder = new DataBinder("Team");
			
			object instance;
			
			NameValueCollection args = BuildComplexParamList();

			args.Add("Team[0]@ignore", "yes");
			instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.IsTrue(team.Length == 1);

			args.Remove("Team[0]@ignore");
			args.Add("Team[0].Members@ignore", "yes");
			instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.IsNotNull(team[0].Members);
			Assert.AreEqual(0, team[0].Members.Length);
		}

		[Test]
		public void NestedLevelArrayDataBind()
		{
			Team[] team = null;
			NameValueCollection args = BuildComplexParamList();

			DataBinder binder = new DataBinder("Team");
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[1].Name == "B-Team");
			Assert.IsNotNull(team[0].Games);
			Assert.IsNotNull(team[0].Members);
			Assert.IsNotNull(team[1].Members);
		}

		[Test]
		public void NestedArrayDataBind()
		{
			Team[] team = null;

			NameValueCollection args = BuildComplexParamList();

			DataBinder binder = new DataBinder("Team");
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.AreEqual(2, team[0].Games[0].Scores.Length);
			Assert.AreEqual(2, team[0].Games[0].Opponents.Length);
			Assert.AreEqual(3, team[0].Games[1].Scores.Length);
			Assert.AreEqual(3, team[0].Games[1].Opponents.Length);
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
			Assert.IsTrue(team[0].Members[0].Age == 25);
			Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
			Assert.IsTrue(team[0].Members[1].Age == 15);
			Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
			Assert.IsTrue(team[1].Members[0].Age == 20);
			Assert.IsTrue(team[1].Name == "B-Team");
		}

		[Test]
		public void NestedArrayDataBindWithExcludeList()
		{
			Team[] team = null;

			NameValueCollection args = BuildComplexParamList();

			DataBinder binder = new DataBinder("Team", new Hashtable(), new ArrayList(), "", "Games,Name");
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.AreEqual(2, team.Length);
			Assert.IsNull(team[0].Games);
			Assert.IsNull(team[0].Name);
			Assert.IsTrue(team[0].Members[0].Age == 25);
			Assert.IsNull(team[0].Members[1].Name);
			Assert.IsTrue(team[0].Members[1].Age == 15);
			Assert.IsNull(team[1].Members[0].Name);
			Assert.IsTrue(team[1].Members[0].Age == 20);
			Assert.IsNull(team[1].Name);
		}

		[Test]
		public void NestedArrayDataBindWithAllowList()
		{
			Team[] team = null;
			NameValueCollection args = BuildComplexParamList();

			DataBinder binder = new DataBinder("Team", new Hashtable(), new ArrayList(), "Name,Games,Members", "");
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			team = instance as Team[];
			Assert.IsNotNull(team);
			Assert.IsNull(team[0].Games[0].Scores);
			Assert.IsNull(team[0].Games[0].Opponents);
			Assert.IsNull(team[0].Games[1].Scores);
			Assert.IsNull(team[0].Games[1].Opponents);
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
			Assert.IsTrue(team[0].Members[0].Age == 0);
			Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
			Assert.IsTrue(team[0].Members[1].Age == 0);
			Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
			Assert.IsTrue(team[1].Members[0].Age == 0);
			Assert.IsTrue(team[1].Name == "B-Team");
		}

		[Test]
		public void NestedArrayDataBindWithAllowAndExcludeList()
		{
			Team[] team = null;

			NameValueCollection args = BuildComplexParamList();

			DataBinder binder = new DataBinder("Team", new Hashtable(), new ArrayList(), "Name,Games,Members", "Games");
			
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			team = instance as Team[];
			Assert.IsNotNull(team);
			// Game pass allowlist but should fail on the exclude
			Assert.IsNull(team[0].Games);
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
			Assert.IsTrue(team[0].Members[0].Age == 0);
			Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
			Assert.IsTrue(team[0].Members[1].Age == 0);
			Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
			Assert.IsTrue(team[1].Members[0].Age == 0);
			Assert.IsTrue(team[1].Name == "B-Team");
		}

		[Test]
		public void SimpleDataBindWithErrors()
		{
			// Test when count is too big
			string data = @"
				Person.Name = John
				Person.Age = Thirty Two?
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			DataBinder binder = new DataBinder("person");
			object instance = binder.BindObject(typeof(Person), new NameValueCollectionAdapter(args));

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.AreEqual(1, binder.Errors.Count);
		}

		[Test]
		public void ComplexDataBindWithErrors()
		{
			// Test when count is too big
			string data = @"
				abc[0].Games[0].Scores	   = x,x
				abc[0].Games[0].Opponents = Santos,Cruzeiro
				abc[0].Games[1].Scores    = x,x,x
				abc[0].Games[1].Opponents = Santos,Cruzeiro,Guarani
				abc[0].Name = A-Team
				abc[0].Members[0].Name = Mr. White
				abc[0].Members[0].Age  = xx
				abc[0].Members[1].Name = Mr. Black
				abc[0].Members[1].Age  = xx
				abc[1].Members[0].Name = Mr. B-White
				abc[1].Members[0].Age  = xx				
				abc[1].Name = B-Team
			";

			NameValueCollection args = TestUtils.ParseNameValueString(data);

			DataBinder binder = new DataBinder("abc");
			object instance = binder.BindObject(typeof(Team[]), new NameValueCollectionAdapter(args));
			Assert.IsNotNull(instance);
			Team[] team = instance as Team[];

			Assert.IsNotNull(team);
			Assert.IsNull(team[0].Games[0].Scores);
			Assert.IsNull(team[0].Games[1].Scores);
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[1].Name == "B-Team");
			Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
			Assert.IsTrue(team[0].Members[0].Age == 0);
			Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
			Assert.IsTrue(team[0].Members[1].Age == 0);
			Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
			Assert.IsTrue(team[1].Members[0].Age == 0);
			Assert.AreEqual(5, binder.Errors.Count);
		}

		#region Helpers

		private NameValueCollection BuildComplexParamList()
		{
			String data = "";

			data += @" 
				Team[0].Games[0].Scores	   = 1,2
				Team[0].Games[0].Opponents = Santos,Cruzeiro
				Team[0].Games[1].Scores    = 1,2,4
				Team[0].Games[1].Opponents = Santos,Cruzeiro,Guarani
				Team[0].Name = A-Team
				Team[0].Members[0].Name = Mr. White
				Team[0].Members[0].Age  = 25
				Team[0].Members[1].Name = Mr. Black
				Team[0].Members[1].Age  = 15
				Team[1].Members[0].Name = Mr. B-White
				Team[1].Members[0].Age  = 20				
				Team[1].Name = B-Team
			";

			return TestUtils.ParseNameValueString(data);
		}

		/// <summary>
		/// TODO: move this function to another file
		/// </summary>
		/// <param name="separator"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private string Join(string separator, IList args)
		{
			if (args.Count == 0) return "";

			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < args.Count; i++)
			{
				sb.Append(args[i]).Append(separator);
			}
			return sb.ToString(0, sb.Length - 1);
		}

		#endregion	
	}

	#region Class Helpers

	class Game
	{
		private int[] _scores;
		private string[] _opponents;

		public int[] Scores
		{
			get { return _scores; }
			set { _scores = value; }
		}

		public String[] Opponents
		{
			get { return _opponents; }
			set { _opponents = value; }
		}
	}

	class Team
	{
		private Game[] _games;
		private string _name;
		private Person[] _members;

		public Game[] Games
		{
			get { return _games; }
			set { _games = value; }
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Person[] Members
		{
			get { return _members; }
			set { _members = value; }
		}
	}

	class Person
	{
		private string _name;
		private Int32 _age;
		private Decimal _assets;
		private DateTime _dob;

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Int32 Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public Decimal Assets
		{
			get { return _assets; }
			set { _assets = value; }
		}

		public DateTime DOB
		{
			get { return _dob; }
			set { _dob = value; }
		}
	}

	#endregion
}