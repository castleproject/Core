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
	public class TypeDeclarationsTestCase
	{
		[Test]
		public void SimpleDeclaration()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(0, classNode.BaseTypes.Count);
		}

		[Test]
		public void SimpleDeclarationWithExtends()
		{
			String contents = 
				"class MyClass < MyBaseType \r\n" + 
				"" + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(1, classNode.BaseTypes.Count);
			Assert.AreEqual( "MyBaseType", (classNode.BaseTypes[0] as QualifiedIdentifier).Name );
		}

		[Test]
		public void SimpleDeclarationWithExtends2()
		{
			String contents = 
				"class MyClass < MyBaseType, IList, Collections.IBindable \r\n" + 
				"" + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(3, classNode.BaseTypes.Count);
			Assert.AreEqual( "MyBaseType", (classNode.BaseTypes[0] as QualifiedIdentifier).Name );
			Assert.AreEqual( "IList", (classNode.BaseTypes[1] as QualifiedIdentifier).Name );
			Assert.AreEqual( "Collections.IBindable", (classNode.BaseTypes[2] as QualifiedIdentifier).Name );
		}

		[Test]
		public void StaticFields()
		{
			String contents = 
				"class MyClass \r\n" + 
				"  @@myfield " + 
				"  @@otherfield " + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(0, classNode.BaseTypes.Count);
			Assert.AreEqual(2, classNode.StaticFields.Count);
			Assert.AreEqual(0, classNode.InstanceFields.Count);
		}

		[Test]
		public void StaticFieldsAndAccessLevels()
		{
			String contents = 
				"class MyClass \r\n" + 
				"private \r\n" +
				"  @@myfield \r\n" + 
				"public \r\n" +
				"  @@otherfield \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(0, classNode.BaseTypes.Count);
			Assert.AreEqual(2, classNode.StaticFields.Count);
			Assert.AreEqual(0, classNode.InstanceFields.Count);
		}
	}
}
