// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;

	public class ConflictBehaviorTestCase
	{
		[TestFixture]
		public class AttributeElementConflict : XmlAdapterTestCase
		{
			[XmlDefaults(Qualified = true)]
			[XmlNamespace("urn:a", "a", Root = true)]
			[XmlType(Namespace = "urn:a")]
			public interface IFoo : IDictionaryAdapter
			{
				[Key("IFoo.A")]
				[XmlAttribute("A")]
				string A { get; set; }
			}

			public interface IBar : IDictionaryAdapter
			{
				string A { get; set; }
			}

			[Test]
			public void Set()
			{
				XmlMetadataBehavior.Default.AddReservedNamespaceUri("urn:a");

				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);
				var bar = (IBar) foo.Coerce(typeof(IBar));

				foo.A = "x";
				bar.A = "y";

				Assert.That(xml, XmlEquivalent.To("<Foo xmlns:a='urn:a' a:A='x'> <A>y</A> </Foo>"));
				Assert.That(foo.A, Is.EqualTo("x"), "foo.A");
				Assert.That(bar.A, Is.EqualTo("y"), "bar.A");
			}
		}
	}
}
#endif
