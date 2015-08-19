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

#if !SILVERLIGHT // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System.Xml.Serialization;
	using Castle.Components.DictionaryAdapter.Tests;
    using NUnit.Framework;

	public class XmlArrayBehaviorTestCase
	{
        [TestFixture]
        public class ArrayBehavior : XmlAdapterTestCase
        {
            public interface IRoot
            {
                [XmlArray("X")]
                [XmlArrayItem("A", typeof(IDerived1))]
                [XmlArrayItem("B", typeof(IDerived2))]
                IBase[] Items { get; set; }
            }

            public interface IBase { }
            public interface IDerived1 : IBase { int    X { get; set; } }
            public interface IDerived2 : IBase { string X { get; set; } } 

            [Test]
            public void GetProperty_ArrayBehavior_Array_Element()
            {
                var foo = Create<IRoot>("<Root> <X> <A X='1'/> <B X='2'/> </X> </Root>");

				var array = foo.Items;
				Assert.AreEqual(2, array.Length);
				Assert.IsInstanceOf<IDerived1>(array[0]);
				Assert.AreEqual(1, ((IDerived1)array[0]).X);
				Assert.IsInstanceOf<IDerived2>(array[1]);
				Assert.AreEqual("2", ((IDerived2)array[1]).X);
            }

            [Test]
            public void SetProperty_ArrayBehavior_Array()
            {
                var xml = Xml("<Root/>");
                var foo = Create<IRoot>(xml);

				foo.Items = new IBase[]
				{
					Create<IDerived1>("<Derived1 X='1'/>"),
					Create<IDerived2>("<Derived2 X='2'/>")
				};

				CustomAssert.AreXmlEquivalent("<Root> <X> <A><X>1</X></A> <B><X>2</X></B> </X> </Root>", xml);

				var array = foo.Items;
				Assert.AreEqual(2, array.Length);
				Assert.IsInstanceOf<IDerived1>(array[0]);
				Assert.AreEqual(1, ((IDerived1)array[0]).X);
				Assert.IsInstanceOf<IDerived2>(array[1]);
				Assert.AreEqual("2", ((IDerived2)array[1]).X);
            }
        }
	}
}
#endif
