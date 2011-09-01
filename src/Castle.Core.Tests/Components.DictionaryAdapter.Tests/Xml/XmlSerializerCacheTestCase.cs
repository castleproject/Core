// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;
#if DOTNET40
	using System.Threading.Tasks;
#endif

	[TestFixture]
	public class XmlSerializerCacheTestCase
	{
		[Test]
		public void Indexer_ReturnsXmlSerializer()
		{
			var serializer = XmlTypeSerializerCache.Instance[typeof(A)];

			Assert.That(serializer, Is.Not.Null);
		}

		//[Test]
		//public void Indexer_ReturnsXmlSerializer_WhichUsesKnownRootElement()
		//{
		//    var serializer = XmlTypeSerializerCache.Instance[typeof(A)];

		//    var settings   = new XmlWriterSettings { OmitXmlDeclaration = true };
		//    var namespaces = new XmlSerializerNamespaces();
		//    namespaces.Add(string.Empty, string.Empty);

		//    var xml = new StringBuilder();
		//    using (var writer = XmlWriter.Create(xml, settings))
		//        serializer.Serialize(writer, new A { X = 1 }, namespaces);

		//    Assert.That(xml.ToString(), XmlEquivalent.To("<Root> <X>1</X> </Root>"));
		//}

		[Test]
		public void Indexer_ForSameType_ReturnsSameInstance()
		{
			var a = XmlTypeSerializerCache.Instance[typeof(A)];
			var b = XmlTypeSerializerCache.Instance[typeof(A)];

			Assert.That(a, Is.SameAs(b));
		}

		[Test]
		public void Indexer_ForDifferentType_ReturnsDifferentInstance()
		{
			var a = XmlTypeSerializerCache.Instance[typeof(A)];
			var b = XmlTypeSerializerCache.Instance[typeof(B)];

			Assert.That(a, Is.Not.SameAs(b));
		}

#if DOTNET40
		[Test]
		public void Indexer_ForSameType_ReturnsSameInstance_Concurrently()
		{
			var count = 16;
			var serializers = new XmlTypeSerializer[count];

			Parallel.For(0, count,
				i => serializers[i] = XmlTypeSerializerCache.Instance[typeof(A)]);

			for (var i = 1; i < count; i++)
				Assert.That(serializers[i], Is.Not.Null & Is.SameAs(serializers[0]));
		}
#endif

		public class A { public int    X { get; set; } }
		public class B { public string X { get; set; } }
	}
}
