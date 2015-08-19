// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
#if !SILVERLIGHT
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Xml;
	using Castle.Components.DictionaryAdapter.Xml;
	using NUnit.Framework;

	[TestFixture]
	public class MemberwiseEqualityHashCodeStrategyTestCase
	{
		private DictionaryAdapterFactory factory;

#if FEATURE_XUNITNET
		public MemberwiseEqualityHashCodeStrategyTestCase()
#else
		[SetUp]
		public void SetUp()
#endif
		{
			factory = new DictionaryAdapterFactory();
			GetAdapter<IPhone>();
			GetAdapter<IAddress>();
		}

		[Test]
		public void Will_Get_Same_HashCode_For_Default()
		{
			var person1 = GetAdapter<IPerson>();
			var person2 = GetXmlAdapter<IPerson>();
			Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
		}

		[Test]
		public void Will_Get_Same_HashCode_For_Same_Members()
		{
			var person1 = GetAdapter<IPerson>();
			PopulatePerson(person1);
			var person2 = GetXmlAdapter<IPerson>();
			PopulatePerson(person2);
			Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
		}

		[Test]
		public void Will_Get_Different_HashCode_For_Different_Members()
		{
			var person1 = GetAdapter<IPerson>();
			PopulatePerson(person1);
			var person2 = GetXmlAdapter<IPerson>();
			PopulatePerson(person2);
			person2.Age = 40;
			Assert.AreNotEqual(person1.GetHashCode(), person2.GetHashCode());
		}

		[Test]
		public void Will_Be_Equal_When_Same_Reference()
		{
			var person = GetAdapter<IPerson>();
			Assert.AreEqual(person, person);
		}

		[Test]
		public void Will_Not_Be_Equal_When_Types_Differ()
		{
			var person = GetAdapter<IPerson>();
			Assert.AreNotEqual(person, person.HomeAddress);
		}

		[Test]
		public void Will_Be_Equal_When_Two_Defaults_Compared()
		{
			var person1 = GetAdapter<IPerson>();
			var person2 = GetXmlAdapter<IPerson>();
			Assert.AreEqual(person1, person2);
		}

		[Test]
		public void Will_Be_Equal_When_Equal_Members()
		{
			var person1 = GetAdapter<IPerson>();
			PopulatePerson(person1);
			var person2 = GetXmlAdapter<IPerson>();
			PopulatePerson(person2);
			Assert.AreEqual(person1, person2);
		}

		[Test]
		public void Will_Not_Be_Equal_When_Equal_Members()
		{
			var person1 = GetAdapter<IPerson>();
			PopulatePerson(person1);
			var person2 = GetXmlAdapter<IPerson>();
			PopulatePerson(person2);
			person2.Age = 40;
			Assert.AreNotEqual(person1, person2);
		}

		[Test]
		public void Will_Handle_Circularities()
		{
			var person1 = GetAdapter<IPerson>();
			PopulatePerson(person1);
			person1.Friends.Add(person1);
			var person2 = GetAdapter<IPerson>();
			PopulatePerson(person2);
			person2.Friends.Add(person2);
			Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
		}

		private void PopulatePerson(IPerson person)
		{
			person.Name = "Robin";
			person.Age = 27;
			person.DOB = new DateTime(1983, 3, 12, 3, 30, 0);
			person.Friends = new List<IPerson>();
			person.First_Name = "Rob";
			person.HomeAddress = GetAdapter<IAddress>();
			person.WorkAddress = GetAdapter<IAddress>();
			person.BillingAddress = GetAdapter<IAddress>();
		}

		private T GetAdapter<T>() where T : class
		{
			return (T)factory.GetAdapter(typeof(T), new Hashtable(), new PropertyDescriptor()
				.AddBehaviors(XmlMetadataBehavior.Default, new MemberwiseEqualityHashCodeStrategy()));
		}

		private T GetXmlAdapter<T>() where T : class
		{
			var xpath = new XmlAdapter(new XmlDocument());
			return (T)factory.GetAdapter(typeof(T), new Hashtable(), new PropertyDescriptor()
				.AddBehaviors(XmlMetadataBehavior.Default, xpath, new MemberwiseEqualityHashCodeStrategy()));
		}
	}
#endif
}
