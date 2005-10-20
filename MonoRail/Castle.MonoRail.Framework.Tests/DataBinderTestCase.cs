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
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	using NUnit.Framework;
	
	using Castle.MonoRail.Framework;

	[TestFixture]
	public class DataBinderTestCase
	{
		[Test]
		public void SimpleDataBind()
		{
			string name = "John";
			int age = 32;
			NameValueCollection args = new NameValueCollection();

			args.Add("Person.Name", name);
			args.Add("Person.Age", age.ToString());
			DataBinder binder = new DataBinder(null);
			object instance = binder.BindObject(
				typeof (Person),"Person", args, null, null, 3, "");

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
			DataBinder binder = new DataBinder(null);
			object instance = binder.BindObject(
				typeof (Game),"Game",args,null,null,3,"");

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
			DataBinder binder = new DataBinder(null);
			object instance = binder.BindObject(
				typeof (Person[]),"Person",args,null,null,3,"");

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
			DataBinder binder = new DataBinder(null);
			object instance;
			NameValueCollection args;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				args = BuildComplexParamList(useCountAttribute);
				args.Add("Team@ignore", "yes");
				instance = binder.BindObject(
					typeof (Team[]),"Team",args,null,null,3,"");
				Assert.IsNull(instance);

				args.Remove("Team@ignore");
				args.Add("Team[0]@ignore", "yes");
				instance = binder.BindObject(
					typeof (Team[]),"Team",args,null,null,3,"");
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team.Length == 1);

				args.Remove("Team[0]@ignore");
				args.Add("Team[0].Members@ignore", "yes");
				instance = binder.BindObject(
					typeof (Team[]),"Team",args,null,null,3,"");
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsNull(team[0].Members);
			}
		}

		[Test]
		public void NestedArrayDataBind()
		{
			Team[] team = null;
			foreach (bool useCountAttribute in new bool[] {true, false})
			{
				NameValueCollection args = BuildComplexParamList(useCountAttribute);

				DataBinder binder = new DataBinder(null);
				object instance = binder.BindObject(
					typeof (Team[]),"Team",args,null,null,3,"");

				Assert.IsNotNull(instance);
				team = instance as Team[];
				Assert.IsNotNull(team);
				Assert.IsTrue(team.Length == 2);
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
			}
		}

		/// <summary>
		/// Test when count is too big
		/// </summary>
		[Test]
		public void NonNumericIdAttribute()
		{
			string data = @"
				Person[john].Name = John
				Person[james].Name = James
				Person[mary].Age = 32
				Person[james].Age = 10
				Person[mary].Name = Mary
			";

			NameValueCollection args = ParseNameValueString(data);

			DataBinder binder = new DataBinder(null);
			object instance = binder.BindObject(
				typeof (Person[]),"Person",args,null,null,3,"");

			Assert.IsNotNull(instance);
			Person[] people = instance as Person[];
			Assert.IsNotNull(people);
			Assert.IsTrue(people.Length == 3);
		}

		/// <summary>
		/// Test when count is too big
		/// </summary>
		[Test]
		public void CountAttribute()
		{
			string data = @"
				Person@count = 1000
				Person[0].Name = John
				Person[1].Name = John
				Person[1].Age = 32
				Person[3].Name = John
			";

			NameValueCollection args = ParseNameValueString(data);

			DataBinder binder = new DataBinder(null);
			object instance = binder.BindObject(
				typeof (Person[]),"Person",args,null,null,3,"");

			Assert.IsNotNull(instance);
			Person[] people = instance as Person[];
			Assert.IsNotNull(people);
			Assert.IsTrue(people.Length == 5);

			args["Person@count"] = "2";
			binder = new DataBinder(null);
			instance = binder.BindObject(
				typeof (Person[]),"Person",args,null,null,3,"");

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
		/// 	"
		/// 	
		/// and returns a NameValueCollection with these elements
		/// "Person@count"   => "2"
		/// "Person[0].Name" => "Gi   Joe"
		/// "Person[0].Age"  => "32"
		/// "Person[1].Name" => "Mary"
		/// "Person[1].Age"  => "16" 		
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <remarks>
		/// Notice that any that leading and trailing spaces are discarded
		/// </remarks>
		private NameValueCollection ParseNameValueString(string data)
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

		public int[] Scores
		{
			get { return _scores; }
			set { _scores = value; }
		}

		private string[] _opponents;

		public String[] Opponents
		{
			get { return _opponents; }
			set { _opponents = value; }
		}
	}

	class Team
	{
		private Game[] _games;

		public Game[] Games
		{
			get { return _games; }
			set { _games = value; }
		}

		private string _name;

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private Person[] _members;

		public Person[] Members
		{
			get { return _members; }
			set { _members = value; }
		}
	}

	class Person
	{
		private string _name;

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private Int32 _age;

		public Int32 Age
		{
			get { return _age; }
			set { _age = value; }
		}
	}

	#endregion
}