// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class DictionaryAdapterFactoryTestCase
	{
		private IDictionary dictionary;
		private DictionaryAdapterFactory factory;

		[SetUp]
		public void SetUp()
		{
			dictionary = new Hashtable();
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void CreateAdapter_NoPrefixPropertiesOnly_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test, ExpectedException(typeof(TypeLoadException))]
		public void CreateAdapter_NoPrefixWithMethod_ThrowsException()
		{
			factory.GetAdapter<IPersonWithMethod>(dictionary);
		}

		[Test]
		public void CreateAdapter_PrefixPropertiesOnly_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test]
		public void UpdateAdapter_NoPrefix_UpdatesDictionary()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>(); // I need some friends

			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>) dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapter_Prefix_UpdatesDictionary()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();
			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>) dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapterAndRead_NoPrefix_Matches()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_Prefix_Matches()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_PrefixOverride_Matches()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefixOverride>(dictionary);
			person.Name = "Craig";

			Assert.AreEqual("Craig", dictionary["Name"]);
		}

		[Test]
		public void ReadAdapter_NoPrefixUnitialized_ReturnsDefaults()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void ReadAdapter_PrefixUnitialized_ReturnsDefaults()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void UpdateAdapterAndRead_WithSeveralDifferentOverridesWithDifferentPrefixes_DictionaryKeysHaveCorrectPrefixes()
		{
			IPersonWithDeniedInheritancePrefix person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);

			string name = "Ming The Merciless";
			int numberOfFeet = 2;
			string hairColour = "Muddy Golden Labrador";
			string eyeColour = "The Colour Of Broken Dreams";
			int numberOfHeads = 1;
			int numberOfFingers = 3;

			person.Name = name;
			person.NumberOfFeet = numberOfFeet;
			person.HairColour = hairColour;
			person.EyeColour = eyeColour;
			person.NumberOfHeads = numberOfHeads;
			person.NumberOfFingers = numberOfFingers;

			string[] keys = new string[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(keys, 0);

			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Name"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "NumberOfFeet"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person_HairColour"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person2_EyeColour"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person2_NumberOfHeads"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "NumberOfFingers"; }));

			Assert.AreEqual(name, person.Name);
			Assert.AreEqual(numberOfFeet, person.NumberOfFeet);
			Assert.AreEqual(hairColour, person.HairColour);
			Assert.AreEqual(eyeColour, person.EyeColour);
			Assert.AreEqual(numberOfHeads, person.NumberOfHeads);
			Assert.AreEqual(numberOfFingers, person.NumberOfFingers);
		}
	}

	public interface IPerson
	{
		string Name { get; set; }

		int Age { get; set; }

		DateTime DOB { get; set; }

		IList<IPerson> Friends { get; set; }
	}

	public interface IPersonWithoutPrefix : IPerson
	{
		int NumberOfFeet { get; set; }
	}

	[DictionaryAdapterKeyPrefix("Person_")]
	public interface IPersonWithPrefix : IPersonWithoutPrefix
	{
		string HairColour { get; set; }
	}

	[DictionaryAdapterKeyPrefix("Person2_")]
	public interface IPersonWithPrefixOverride : IPersonWithPrefix
	{
		string EyeColour { get; set; }
	}

	public interface IPersonWithPrefixOverrideFurtherOverride : IPersonWithPrefixOverride
	{
		int NumberOfHeads { get; set; }
	}

	[DictionaryAdapterKeyPrefix("")]
	public interface IPersonWithDeniedInheritancePrefix : IPersonWithPrefixOverrideFurtherOverride
	{
		int NumberOfFingers { get; set; }
	}

	public interface IPersonWithMethod : IPerson
	{
		void Run();
	}
}
