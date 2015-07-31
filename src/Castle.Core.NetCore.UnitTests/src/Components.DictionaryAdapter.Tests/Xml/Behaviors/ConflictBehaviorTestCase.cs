﻿// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT && !MONO && !NETCORE // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
	using Castle.Components.DictionaryAdapter.Tests;
	using Xunit;

	public class ConflictBehaviorTestCase
	{
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

			[Fact]
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

		[TestFixture]
		public class AssignCollectionWithPriorItem : XmlAdapterTestCase
		{
			[Reference]
			public interface IHasItemsArray
			{
				IItem[] Array { get; set; }
			}

			[Reference]
			public interface IHasItemsList
			{
				IList<IItem> List { get; set; }
			}

			[Reference]
			public interface IItem
			{
				string Value { get; set; }
			}

			[Fact]
			public void AssignArray()
			{
				var xml     = Xml("<HasItemsArray/>");
				var foo     = Create<IHasItemsArray>(xml);
				var itemA   = Create<IItem>();
				var itemB   = Create<IItem>();

				foo.Array = new[] {        itemB };
				foo.Array = new[] { itemA, itemB };

				var expectedXml = Xml("<Root> <Array> <Item/> <Item/> </Array> </Root>");
				Assert.That(xml, XmlEquivalent.To(expectedXml));
			}

			[Fact]
			public void AssignList()
			{
				var xml   = Xml("<HasItemsList/>");
				var foo   = Create<IHasItemsList>(xml);
				var itemA = Create<IItem>();
				var itemB = Create<IItem>();

				foo.List = new[] {        itemB };
				foo.List = new[] { itemA, itemB };

				var expectedXml = Xml("<Root> <List> <Item/> <Item/> </List> </Root>");
				Assert.That(xml, XmlEquivalent.To(expectedXml));
			}
		}

		[TestFixture]
		public class ReferenceBehaviorAndMultipleCopy : XmlAdapterTestCase
		{
			[Reference]
			public interface IFoo : IDictionaryAdapter
			{
				IFoo        One  { get; set; }
				IList<IFoo> List { get; set; }
			}

			[Fact]
			public void NestedReference_Strange()
			{
				var xml = Xml("<Foo/>");
				var a = Create<IFoo>(xml);
				var b = Create<IFoo>();
				var c = Create<IFoo>();

				b.List.Add(c);
				a.One = b;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One>",
							"<List> <Foo/> </List>",
						"</One>",
					"</Foo>"
				)));
			}

			protected override T Create<T>(XmlNode storage, Action<PropertyDescriptor> config)
			{
				return base.Create<T>(storage, d => d.AddBehavior(new MultipleCopyAttribute()));
			}

			public class MultipleCopyAttribute : DictionaryBehaviorAttribute,
				IDictionaryInitializer,
				IDictionaryCopyStrategy
			{
				private bool copying;

				public void Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
				{
					dictionaryAdapter.This.AddCopyStrategy(this);
				}

				public bool Copy(IDictionaryAdapter source, IDictionaryAdapter target,
					ref Func<DictionaryAdapter.PropertyDescriptor, bool> selector)
				{
					if (copying)
						return false;

					try
					{
						copying = true;

						// An unwise idea, but it shouldn't cause DA to implode.
						source.CopyTo(target);
						source.CopyTo(target);
					}
					finally
					{
						copying = false;
					}

					return true;
				}
			}
		}
	}
}
#endif
