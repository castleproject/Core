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

	using NUnit.Framework;

	public class XmlTypeBehaviorTestCase
	{
		[TestFixture]
		public class DerivedComponent : XmlAdapterTestCase
		{
			[XmlType("TRoot", Namespace = "urn:root")]
			[XmlInclude(typeof(IB))]
			public interface IRoot : IDictionaryAdapter
			{
				IA A { get; set; }
			}

			[XmlType("TA", Namespace = "urn:a")]
			public interface IA : IDictionaryAdapter
			{
				string A { get; set; }
			}

			[XmlType("TB", Namespace = "urn:b")]
			public interface IB : IA
			{
				string B { get; set; }
			}

			[Test]
			public void DerivedComponentInDifferentNameSpace()
			{
				var xml = Xml
				(
					"<TRoot xmlns:r='urn:root' xmlns:a='urn:a' xmlns:b='urn:b' $xsi>",
						"<r:A xsi:type='b:TB'>",
							"<a:A>a</a:A>",
							"<b:B>b</b:B>",
						"</r:A>",
					"</TRoot>"
				);
				var foo = Create<IRoot>(xml);

				var baseObj = foo.A;
				Assert.IsNotNull(baseObj);
				Assert.IsInstanceOf<IB>(baseObj);

				var derivedObj = (IB) baseObj;
				Assert.AreEqual("a", derivedObj.A);
				Assert.AreEqual("b", derivedObj.B);
			}
		}
	}
}
#endif
