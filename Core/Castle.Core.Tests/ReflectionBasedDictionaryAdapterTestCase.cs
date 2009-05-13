// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Tests
{
	using System;
	using Castle.Core;
	using NUnit.Framework;

	[TestFixture]
	public class ReflectionBasedDictionaryAdapterTestCase
	{
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CannotCreateWithNullArgument()
		{
			new ReflectionBasedDictionaryAdapter(null);
		}

		[Test]
		public void CanAccessExistingPropertiesInACaseInsensitiveFashion()
		{
			ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));
			
			Assert.IsTrue(dict.Contains("id"));
			Assert.IsTrue(dict.Contains("ID"));
			Assert.IsTrue(dict.Contains("Id"));
			Assert.IsTrue(dict.Contains("name"));
			Assert.IsTrue(dict.Contains("Name"));
			Assert.IsTrue(dict.Contains("NAME"));
		}

		[Test]
		public void ShouldNotAccessInexistingProperties()
		{
			ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsFalse(dict.Contains("Age"), "Age property found when it should not be");
            Assert.IsFalse(dict.Contains("Address"), "Address property found when it should not be");
		}

		[Test]
		public void CanAccessPropertiesValues()
		{
			ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.AreEqual(1, dict["id"]);
			Assert.AreEqual("name", dict["name"]);
		}

		[Test]
		public void InexistingPropertiesReturnsNull()
		{
			ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsNull(dict["age"]);
		}

		[Test/*(Description = "Test case for patch supplied on the mailing list by Jan Limpens")*/]
		public void ShouldNotAccessWriteOnlyProperties()
		{
			try
			{
				ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name", true));
				Assert.IsTrue((bool) dict["IsWriteOnly"]);
			}
			catch(ArgumentException)
			{
				Assert.Fail("Attempted to read a write-only property");
			}
		}

#if DOTNET35
		[Test]
		public void EnumeratorIteration()
		{
			ReflectionBasedDictionaryAdapter dict = new ReflectionBasedDictionaryAdapter(new {id = 1, name = "jonh", age = 25});

			Assert.AreEqual(3, dict.Count);

			System.Collections.IDictionaryEnumerator enumerator = (System.Collections.IDictionaryEnumerator)dict.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Assert.IsNotNull(enumerator.Key);
				Assert.IsNotNull(enumerator.Value);
			}
		}
#endif


		public class Customer
		{
			private int id;
			private string name;
			private bool writeOnly;

			public Customer(int id, string name)
				: this(id, name, false)
			{
			}

			public Customer(int id, string name, bool writeOnly)
			{
				this.id = id;
				this.name = name;
				this.writeOnly = writeOnly;
			}

			public int Id
			{
				get { return id; }
				set { id = value; }
			}

			public string Name
			{
				get { return name; }
				set { name = value; }
			}

			public bool WriteOnly
			{
				set { writeOnly = value; }
			}

			public bool IsWriteOnly
			{
				get { return writeOnly; }
			}
		}
	}
}
