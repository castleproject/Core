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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class DictionarySerializationTestCase
	{
		[Test]
		public void NullReferenceProxyDeserializationTest()
		{
			ProxyGenerator generator = new ProxyGenerator();
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstances =
				new Dictionary<ClassOverridingEqualsAndGetHashCode, string>();
			ClassOverridingEqualsAndGetHashCode c =
				(ClassOverridingEqualsAndGetHashCode)generator.CreateClassProxy(typeof(ClassOverridingEqualsAndGetHashCode));
			c.Id = Guid.NewGuid();
			c.Name = DateTime.Now.ToString("yyyyMMddHHmmss");
			theInstances.Add(c, c.Name);
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstancesBis =
				SerializeAndDeserialize<Dictionary<ClassOverridingEqualsAndGetHashCode, string>>(theInstances);

			Assert.IsNotNull(theInstancesBis);
			Assert.AreEqual(theInstances.Count, theInstancesBis.Count);
		}

		[Test]
		public void DictionaryDeserializationWithoutProxyTest()
		{
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstances =
				new Dictionary<ClassOverridingEqualsAndGetHashCode, string>();

			for (int i = 0; i < 50; i++)
			{
				ClassOverridingEqualsAndGetHashCode c = new ClassOverridingEqualsAndGetHashCode();
				c.Id = Guid.NewGuid();
				c.Name = DateTime.Now.ToString("yyyyMMddHHmmss");
				theInstances.Add(c, c.Name);
			}

#pragma warning disable 219
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstancesBis =
				SerializeAndDeserialize<Dictionary<ClassOverridingEqualsAndGetHashCode, string>>(theInstances);
#pragma warning restore 219
		}

		[Test]
		public void DictionaryDeserializationWithProxyTest()
		{
			ProxyGenerator generator = new ProxyGenerator();
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstances =
				new Dictionary<ClassOverridingEqualsAndGetHashCode, string>();

			for (int i = 0; i < 50; i++)
			{
				ClassOverridingEqualsAndGetHashCode c =
					(ClassOverridingEqualsAndGetHashCode)generator.CreateClassProxy(typeof(ClassOverridingEqualsAndGetHashCode));
				c.Id = Guid.NewGuid();
				c.Name = DateTime.Now.ToString("yyyyMMddHHmmss");
				theInstances.Add(c, c.Name);
			}

#pragma warning disable 219
			Dictionary<ClassOverridingEqualsAndGetHashCode, string> theInstancesBis =
				SerializeAndDeserialize<Dictionary<ClassOverridingEqualsAndGetHashCode, string>>(theInstances);
#pragma warning restore 219
		}

		[Test]
		public void BasicSerializationProxyTest()
		{
			ProxyGenerator generator = new ProxyGenerator();
			ClassOverridingEqualsAndGetHashCode c =
				(ClassOverridingEqualsAndGetHashCode)generator.CreateClassProxy(typeof(ClassOverridingEqualsAndGetHashCode));
			c.Id = Guid.NewGuid();
			c.Name = DateTime.Now.ToString("yyyyMMddHHmmss");

			ClassOverridingEqualsAndGetHashCode c2 = SerializeAndDeserialize<ClassOverridingEqualsAndGetHashCode>(c);
			Assert.IsNotNull(c2);
			Assert.AreEqual(c.Id, c2.Id);
			Assert.AreEqual(c.Name, c2.Name);
		}

		public static T SerializeAndDeserialize<T>(T proxy)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, proxy);
			stream.Position = 0;
			return (T) formatter.Deserialize(stream);
		}
	}
}

#endif