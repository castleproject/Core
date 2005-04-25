// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Parse.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.AST;

	[TestFixture]
	public class NamespacesTestCase
	{
		[Test]
		public void SimpleDeclaration()
		{
			String contents = "namespace MyNamespace \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(1, unit.Namespaces.Count);
			
			NamespaceNode ns = unit.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("MyNamespace", ns.Identifier.Name);
		}

		[Test]
		public void SimpleDeclaration2()
		{
			String contents = "namespace My.qualified.name \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(1, unit.Namespaces.Count);
			
			NamespaceNode ns = unit.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("My.qualified.name", ns.Identifier.Name);
		}

		[Test]
		public void NestedDeclarations()
		{
			String contents = 
				"namespace My \r\n" + 
				"  namespace Nested \r\n" + 
				"    namespace Declaration \r\n" + 
				"    end \r\n" + 
				"  end \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(1, unit.Namespaces.Count);
			
			NamespaceNode ns = unit.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("My", ns.Identifier.Name);
			Assert.AreEqual(1, ns.Namespaces.Count);

			ns = ns.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("Nested", ns.Identifier.Name);
			Assert.AreEqual(1, ns.Namespaces.Count);

			ns = ns.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("Declaration", ns.Identifier.Name);
			Assert.AreEqual(0, ns.Namespaces.Count);
		}

		[Test]
		public void MixinTypes()
		{
			String contents = 
				"namespace My \r\n" + 
				"  mixin MyMixin \r\n" + 
				"  end \r\n" + 
				"  class MyClass \r\n" + 
				"  end \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(1, unit.Namespaces.Count);
			
			NamespaceNode ns = unit.Namespaces[0] as NamespaceNode;
			Assert.IsNotNull(ns);
			Assert.AreEqual("My", ns.Identifier.Name);
			Assert.AreEqual(0, ns.Namespaces.Count);
			Assert.AreEqual(1, ns.MixinTypes.Count);
			Assert.AreEqual(1, ns.ClassesTypes.Count);
		}
	}
}
