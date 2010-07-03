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

#if SILVERLIGHT
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Castle.Core; PublicKey=024004800094000620002400525341310400101077f5e87030dadccce6902c6adab7a987bd69cb5819991531f560785eacfc89b6fcddf6bb2a0743a7194e454c0273447fc6eec36474ba8e5a3823147d214298e4f9a631b1afee1a51ffeae4672d498f14b0e3d321453cdd8ac64de7e1cf4d222b7e81f54d4fd4672537d702a5b48738cc29d09228f1aa722ae1a9ca2fb")]
// so that using anonymous types does not fail under SL
#endif

namespace Castle.Core.Tests
{
	using System;
	using System.Collections;

	using Castle.Core;

	using NUnit.Framework;

	[TestFixture]
	public class ReflectionBasedDictionaryAdapterTestCase
	{
		public class Customer
		{
			private bool writeOnly;

			public Customer(int id, string name)
				: this(id, name, false)
			{
			}

			public Customer(int id, string name, bool writeOnly)
			{
				this.Id = id;
				this.Name = name;
				this.writeOnly = writeOnly;
			}

			public int Id { get; set; }

			public string Name { get; set; }

			public bool WriteOnly
			{
				set { writeOnly = value; }
			}

			public bool IsWriteOnly
			{
				get { return writeOnly; }
			}
		}

		[Test]
		public void CanAccessExistingPropertiesInACaseInsensitiveFashion()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsTrue(dict.Contains("id"));
			Assert.IsTrue(dict.Contains("ID"));
			Assert.IsTrue(dict.Contains("Id"));
			Assert.IsTrue(dict.Contains("name"));
			Assert.IsTrue(dict.Contains("Name"));
			Assert.IsTrue(dict.Contains("NAME"));
		}

		[Test]
		public void CanAccessPropertiesValues()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.AreEqual(1, dict["id"]);
			Assert.AreEqual("name", dict["name"]);
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void CannotCreateWithNullArgument()
		{
			new ReflectionBasedDictionaryAdapter(null);
		}

		[Test]
		public void EnumeratorIteration()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new {id = 1, name = "jonh", age = 25});

			Assert.AreEqual(3, dict.Count);

			var enumerator = (IDictionaryEnumerator) dict.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Assert.IsNotNull(enumerator.Key);
				Assert.IsNotNull(enumerator.Value);
			}
		}

		[Test]
		public void InexistingPropertiesReturnsNull()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsNull(dict["age"]);
		}

		[Test]
		public void ShouldNotAccessInexistingProperties()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsFalse(dict.Contains("Age"), "Age property found when it should not be");
			Assert.IsFalse(dict.Contains("Address"), "Address property found when it should not be");
		}

		[Test /*(Description = "Test case for patch supplied on the mailing list by Jan Limpens")*/]
		public void ShouldNotAccessWriteOnlyProperties()
		{
			try
			{
				var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name", true));
				Assert.IsTrue((bool) dict["IsWriteOnly"]);
			}
			catch (ArgumentException)
			{
				Assert.Fail("Attempted to read a write-only property");
			}
		}
	}
}