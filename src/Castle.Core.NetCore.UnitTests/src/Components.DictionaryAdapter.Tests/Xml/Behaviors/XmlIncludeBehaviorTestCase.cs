﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	using System.Xml.Serialization;
	using Xunit;

	public class XmlIncludeBehaviorTestCase
	{
		public abstract class BaseTestCase<TX, TA, TB> : XmlAdapterTestCase
			where TB : TA
		{
			protected abstract TA     GetX(TX obj);
			protected abstract string GetA(TA obj);
			protected abstract string GetB(TB obj);

			[Fact]
			public void Get_NoXsiType()
			{
				var xml = Xml("<Foo> <X> <A>a</A> </X> </Foo>");
				var obj = Create<TX>(xml);

				var x = GetX(obj);
				Assert.That(x,       Is.InstanceOf<TA>() & Is.Not.InstanceOf<TB>());
				Assert.That(GetA(x), Is.EqualTo("a"));
			}

			[Fact]
			public void Get_XsiType_Default()
			{
				var xml = Xml("<Foo $xsi> <X xsi:type='A'> <A>a</A> </X> </Foo>");
				var obj = Create<TX>(xml);

				var x = GetX(obj);
				Assert.That(x,       Is.InstanceOf<TA>() & Is.Not.InstanceOf<TB>());
				Assert.That(GetA(x), Is.EqualTo("a"));
			}

			[Fact]
			public void Get_XsiType_Included()
			{
				var xml = Xml("<Foo $xsi> <X xsi:type='B'> <A>a</A> <B>b</B> </X> </Foo>");
				var obj = Create<TX>(xml);

				var x = GetX(obj);
				Assert.That(x,       Is.InstanceOf<TB>());
				Assert.That(GetA(x), Is.EqualTo("a"));

				var b = (TB) x;
				Assert.That(GetB(b), Is.EqualTo("b"));
			}

			[Fact]
			public void Get_XsiType_Unrecognized()
			{
				var xml = Xml("<Foo $xsi> <X xsi:type='C'> <A>a</A> </X> </Foo>");
				var obj = Create<TX>(xml);

				var x = GetX(obj);
				Assert.That(x,       Is.InstanceOf<TA>()); // virtual
				Assert.That(GetA(x), Is.Null);
			}
		}

		public class IncludedByDeclaringType : BaseTestCase<
			IncludedByDeclaringType.IFoo,
			IncludedByDeclaringType.IA,
			IncludedByDeclaringType.IB>
		{
			[XmlInclude(typeof(IB))]
			public interface IFoo
			{
				IA X { get; set; }
			}

			public interface IA      { string A { get; set; } }
			public interface IB : IA { string B { get; set; } }

			protected override IA     GetX(IFoo obj) { return obj.X; }
			protected override string GetA(IA   obj) { return obj.A; }
			protected override string GetB(IB   obj) { return obj.B; }
		}

		public class IncludedByDeclaredType : BaseTestCase<
			IncludedByDeclaredType.IFoo,
			IncludedByDeclaredType.IA,
			IncludedByDeclaredType.IB>
		{
			public interface IFoo
			{
				IA X { get; set; }
			}

			[XmlInclude(typeof(IB))]
			public interface IA      { string A { get; set; } }
			public interface IB : IA { string B { get; set; } }

			protected override IA     GetX(IFoo obj) { return obj.X; }
			protected override string GetA(IA   obj) { return obj.A; }
			protected override string GetB(IB   obj) { return obj.B; }
		}

		public class UnnecessaryInclude : BaseTestCase<
			UnnecessaryInclude.IFoo,
			UnnecessaryInclude.IA,
			UnnecessaryInclude.IB>
		{
			[XmlInclude(typeof(IB))] // OK
			[XmlInclude(typeof(IB))] // duplicate include
			[XmlInclude(typeof(IA))] // same as declared type
			[XmlInclude(typeof(IC))] // not assignable to declared type
			public interface IFoo
			{
				IA X { get; set; }
			}

			// TODO: Fails if both IA and IB included here
			//[XmlInclude(typeof(IA))] // same as declared type
			[XmlInclude(typeof(IB))] // duplicate include
			[XmlInclude(typeof(IC))] // not assignable to declared type
			public interface IA      { string A { get; set; } }
			public interface IB : IA { string B { get; set; } }
			public interface IC      { string C { get; set; } }

			protected override IA     GetX(IFoo obj) { return obj.X; }
			protected override string GetA(IA   obj) { return obj.A; }
			protected override string GetB(IB   obj) { return obj.B; }
		}
	}
}
#endif
