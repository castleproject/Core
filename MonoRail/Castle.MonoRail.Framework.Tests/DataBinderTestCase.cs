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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text;

	using NUnit.Framework;
	
	using Castle.MonoRail.Framework;

	[TestFixture]
	public class DataBinderTestCase
	{
	
		[Test]
		public void DateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("DOBday", 1.ToString());
			args.Add("DOBmonth", 12.ToString());
			args.Add("DOByear", 2005.ToString() );
			
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person), "", args);

			Assert.IsNotNull(instance);
			Person p = (Person) instance ;
			Assert.IsNotNull(instance);
			Assert.AreEqual( p.DOB, new DateTime(2005,12,1) );
		}

		[Test]
		public void DayTooBigDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("DOBday", 31.ToString());
			args.Add("DOBmonth", 2.ToString());
			args.Add("DOByear", 2005.ToString() );
			
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person), "", args);

			Assert.IsNotNull(instance);
			Person p = (Person) instance ;
			Assert.IsNotNull(instance);
			Assert.AreEqual( p.DOB, new DateTime(2005,2,28) );
		}

		[Test]
		public void InvalidDateTimeBind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("DOBday", 32.ToString());
			args.Add("DOBmonth", 2.ToString());
			args.Add("DOByear", 2005.ToString() );
			
			try
			{
				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Person), "", args);
				Assert.Fail("InvalidDateTimeBind must throw an ArgumentException");
			}
			catch(ArgumentException){}
		}

		[Test]
		public void InvalidDateTime2Bind()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("DOBday", (-2).ToString());
			args.Add("DOBmonth", 2.ToString());
			args.Add("DOByear", 2005.ToString() );
			
			try
			{
				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Person), "", args);
				Assert.Fail("InvalidDateTime2Bind must throw an ArgumentException");
			}
			catch(ArgumentException){}
		}
								
		[Test]
		public void SimpleDataBind()
		{
			String name = "John";
			int age = 32;
			decimal assets = (decimal)100000;
			NameValueCollection args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			args.Add("Person.Assets", assets.ToString() );
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person), "Person", args);

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.AreEqual(person.Age, age);
			Assert.AreEqual(person.Name, name);
			Assert.AreEqual(person.Assets, assets);
		}

		[Test]
		public void SimpleDataBindNoPrefix()
		{
			string name = "John";
			int age = 32;
			NameValueCollection args = new NameValueCollection();

			args.Add("Name", name);
			args.Add("Age", age.ToString());
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof(Person), "", args);

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
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof(Game), "Game", args);

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
				Person@count   = 2
				Person[0].Name = John
				Person[0].Age  = 32
				Person[1].Name = Mary
				Person[1].Age  = 16
			";

			NameValueCollection args = ParseNameValueString(data);
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof(Person[]), "Person", args);

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
		public void SimpleArrayDataBindNoPrefix()
		{
			string data = @" 
				[0].Name = John
				[0].Age  = 32
				[1].Name = Mary
				[1].Age  = 16
			";

			NameValueCollection args = ParseNameValueString(data);
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person[]), "", args);

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
			DataBinder binder = new DataBinder();
			object instance;
			NameValueCollection args;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				args = BuildComplexParamList(useCountAttribute);
				args.Add("Team@ignore", "yes");
				instance = binder.BindObject(typeof (Team[]), "Team", args);
				Assert.IsNull(instance);

				args.Remove("Team@ignore");
				args.Add("Team[0]@ignore", "yes");
				instance = binder.BindObject(typeof(Team[]), "Team", args);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team.Length == 1);

				args.Remove("Team[0]@ignore");
				args.Add("Team[0].Members@ignore", "yes");
				instance = binder.BindObject(typeof (Team[]), "Team", args);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsNull(team[0].Members);
			}
		}

		[Test]
		public void NestedLevelArrayDataBind()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Team[]),"Team",args,null,null,1,null);

				Assert.IsNotNull(instance);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team[0].Name == "A-Team");
				Assert.IsTrue(team[1].Name == "B-Team");
				Assert.IsNull(team[0].Games);
				Assert.IsNull(team[0].Members);
				Assert.IsNull(team[1].Members);
			}
		}
		
		[Test]
		public void NestedArrayDataBind()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Team[]),"Team",args);

				Assert.IsNotNull(instance);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team[0].Games[0].Scores.Length == 2);
				Assert.IsTrue(team[0].Games[0].Opponents.Length == 2);
				Assert.IsTrue(team[0].Games[1].Scores.Length == 3);
				Assert.IsTrue(team[0].Games[1].Opponents.Length == 3);
				Assert.IsTrue(team[0].Name == "A-Team");
				Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
				Assert.IsTrue(team[0].Members[0].Age == 25);
				Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
				Assert.IsTrue(team[0].Members[1].Age == 15);
				Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
				Assert.IsTrue(team[1].Members[0].Age == 20);
				Assert.IsTrue(team[1].Name == "B-Team");
			}
		}

		[Test]
		public void NestedArrayDataBindWithExcludeList()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Team[]),"Team",args,null,null,3,"Games,Name");

				Assert.IsNotNull(instance);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team.Length == 2);
				Assert.IsNull(team[0].Games);
				Assert.IsNull(team[0].Name);
				Assert.IsTrue(team[0].Members[0].Age == 25);
				Assert.IsNull(team[0].Members[1].Name);
				Assert.IsTrue(team[0].Members[1].Age == 15);
				Assert.IsNull(team[1].Members[0].Name);
				Assert.IsTrue(team[1].Members[0].Age == 20);
				Assert.IsNull(team[1].Name);
			}
		}
		
		[Test]
		public void NestedArrayDataBindWithAllowList()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Team[]),"Team",args,null,null,3,null,"Name,Games,Members");

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
		}
		
		[Test]
		public void NestedArrayDataBindWithAllowAndExcludeList()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder();
				object instance = binder.BindObject(typeof (Team[]),"Team",args,null,null,3,"Games","Name,Games,Members");

				
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
		}
		
		[Test]
		public void NonNumericIdAttribute()
		{
			// Test when count is too big
			string data = @"
				Person[john].Name = John
				Person[james].Name = James
				Person[mary].Age = 32
				Person[james].Age = 10
				Person[mary].Name = Mary
			";

			NameValueCollection args = ParseNameValueString(data);

			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person[]), "Person", args);

			Assert.IsNotNull(instance);
			Person[] people = instance as Person[];
			Assert.IsNotNull(people);
			Assert.IsTrue(people.Length == 3);
		}

		[Test]
		public void SimpleDataBindWithErrors()
		{
			// Test when count is too big
			string data = @"
				Person.Name = John
				Person.Age = Thirty Two?
			";

			NameValueCollection args = ParseNameValueString(data);
			ArrayList errorList = new ArrayList();
			
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person), "Person", args, null, errorList, 3, null);

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.IsTrue(errorList.Count == 1);
		}

		[Test]
		public void SimpleDataBindWithErrorsNoPrefix()
		{
			// Test when count is too big
			string data = @"
				Name = John
				Age = Thirty Two?
			";

			NameValueCollection args = ParseNameValueString(data);
			ArrayList errorList = new ArrayList();
			
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person), null, args, null, errorList, 3, null);

			Assert.IsNotNull(instance);
			Person person = instance as Person;
			Assert.IsNotNull(person);
			Assert.IsTrue(person.Name == "John");
			Assert.IsTrue(person.Age == 0);
			Assert.IsTrue(errorList.Count == 1);
			Assert.IsTrue( (errorList[0] as DataBindError).Key.IndexOf("Person") == 0 );
		}
		
		[Test]
		public void ComplexDataBindWithErrors()
		{
			// Test when count is too big
			string data = @"
				[0].Games[0].Scores	   = x,x
				[0].Games[0].Opponents = Santos,Cruzeiro
				[0].Games[1].Scores    = x,x,x
				[0].Games[1].Opponents = Santos,Cruzeiro,Guarani
				[0].Name = A-Team
				[0].Members[0].Name = Mr. White
				[0].Members[0].Age  = xx
				[0].Members[1].Name = Mr. Black
				[0].Members[1].Age  = xx
				[1].Members[0].Name = Mr. B-White
				[1].Members[0].Age  = xx				
				[1].Name = B-Team
			";

			NameValueCollection args = ParseNameValueString(data);
			ArrayList errorList = new ArrayList();
			
			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Team[]), null, args, null, errorList, 3, null);
			Assert.IsNotNull(instance);
			Team[] team = instance as Team[];
			
			Assert.IsNotNull(team);
			Assert.IsNull(team[0].Games[0].Scores );
			Assert.IsNull(team[0].Games[1].Scores );
			Assert.IsTrue(team[0].Name == "A-Team");
			Assert.IsTrue(team[1].Name == "B-Team");
			Assert.IsTrue(team[0].Members[0].Name == "Mr. White");
			Assert.IsTrue(team[0].Members[0].Age == 0);
			Assert.IsTrue(team[0].Members[1].Name == "Mr. Black");
			Assert.IsTrue(team[0].Members[1].Age == 0);
			Assert.IsTrue(team[1].Members[0].Name == "Mr. B-White");
			Assert.IsTrue(team[1].Members[0].Age == 0);
			Assert.IsTrue(errorList.Count == 5);
			Assert.IsTrue( (errorList[0] as DataBindError).Key.IndexOf("Team") == 0 );			
		}
				
		[Test]
		public void CountAttribute()
		{
			// Test when count is too big
			string data = @"
				Person@count = 1000
				Person[0].Name = John
				Person[1].Name = John
				Person[1].Age = 32
				Person[3].Name = John
			";

			NameValueCollection args = ParseNameValueString(data);

			DataBinder binder = new DataBinder();
			object instance = binder.BindObject(typeof (Person[]), "Person", args );

			Assert.IsNotNull(instance);
			Person[] people = instance as Person[];
			Assert.IsNotNull(people);
			Assert.IsTrue(people.Length == 5);

			args["Person@count"] = "2";
			binder = new DataBinder();
			instance = binder.BindObject(typeof (Person[]), "Person", args );

			Assert.IsNotNull(instance);
			people = instance as Person[];
			Assert.IsNotNull(people);
			Assert.IsTrue(people.Length == 2);
		}

		#region Helpers

		private NameValueCollection BuildComplexParamList(bool useCountAttribute)
		{
			String data = "";

			if (useCountAttribute)
			{
				data += @"
					Team@count = 2
					Team[0].Games@count   = 2
					Team[0].Members@count = 2
					Team[1].Games@count   = 0
					Team[1].Members@count = 1
				";
			}

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

			return ParseNameValueString(data);
		}

		/// <summary>
		/// Parse a string in this format:
		/// @" 
		/// 		Person@count   = 2
		/// 		Person[0].Name = Gi   Joe
		/// 		Person[0].Age  = 32
		/// 		Person[1].Name = Mary
		/// 		Person[1].Age  = 16
		/// 	";
		/// and return a NameValueCollection with these elements
		/// 												"Person@count"   => "2"
		/// "Person[0].Name" => "Gi   Joe"
		/// "Person[0].Age"  => "32"
		/// "Person[1].Name" => "Mary"
		/// "Person[1].Age"  => "16" 		
		/// Notice that any that leading and trailing spaces are discarded
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static NameValueCollection ParseNameValueString(string data)
		{
			NameValueCollection args = new NameValueCollection();
			data = data.Trim();
			foreach (string nameValue in data.Split('\n'))
			{
				if (nameValue.Trim() == "") continue;

				string[] pair = nameValue.Split('=');
				args.Add(pair[0].Trim(), pair[1].Trim());
			}
			return args;
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
			for (int i = 0; i < args.Count; i++)
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
