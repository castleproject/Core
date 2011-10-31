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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using NUnit.Framework;
#if DOTNET40
    using System.Threading.Tasks;
#endif

    [TestFixture]
    public class XmlTypeSerializerCacheTestCase
    {
        [Test]
        public void Indexer_ReturnsSerializer()
        {
            var serializer = XmlTypeSerializerCache.Instance[typeof(A)];

            Assert.That(serializer, Is.Not.Null & Is.InstanceOf<XmlTypeSerializer>());
        }

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
#endif
