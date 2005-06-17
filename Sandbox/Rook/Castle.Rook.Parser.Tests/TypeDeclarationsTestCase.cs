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
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
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
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(1, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual( "MyBaseType", (ClassDefinitionStatement.BaseTypes[0] as Identifier).Name );
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
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(3, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual( "MyBaseType", (ClassDefinitionStatement.BaseTypes[0] as Identifier).Name );
			Assert.AreEqual( "IList", (ClassDefinitionStatement.BaseTypes[1] as Identifier).Name );
			Assert.AreEqual( "Collections.IBindable", (ClassDefinitionStatement.BaseTypes[2] as Identifier).Name );
		}

		[Test]
		public void StaticFields()
		{
			String contents = 
				"class MyClass \r\n" + 
				"  @@myfield = 1 " + 
				"  @@otherfield = 22 " + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(2, ClassDefinitionStatement.Statements.Count);

			FieldDeclarationStatement stmt = ClassDefinitionStatement.Statements[0] as FieldDeclarationStatement;
			Assert.IsNotNull(stmt);
			Assert.AreEqual("@@myfield", (stmt.Target as StaticFieldReferenceExpression).Name);
			Assert.AreEqual("1", (stmt.Value as LiteralExpression).Value);

			stmt = ClassDefinitionStatement.Statements[1] as FieldDeclarationStatement;
			Assert.IsNotNull(stmt);
			Assert.AreEqual("@@otherfield", (stmt.Target as StaticFieldReferenceExpression).Name);
			Assert.AreEqual("22", (stmt.Value as LiteralExpression).Value);
		}

		[Test]
		public void ExplicitFieldsDeclaration()
		{
			String contents = 
				"class MyClass \r\n" + 
				"  @myfield:int " + 
				"  @otherfield:string " + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(2, ClassDefinitionStatement.Statements.Count);

//			FieldDeclarationStatement stmt = ClassDefinitionStatement.Statements[0] as FieldDeclarationStatement;
//			Assert.IsNotNull(stmt);
//			Assert.AreEqual("@@myfield", (stmt.Target as StaticFieldReferenceExpression).Name);
//			Assert.AreEqual("1", (stmt.Value as LiteralExpression).Value);
//
//			stmt = ClassDefinitionStatement.Statements[1] as FieldDeclarationStatement;
//			Assert.IsNotNull(stmt);
//			Assert.AreEqual("@@otherfield", (stmt.Target as StaticFieldReferenceExpression).Name);
//			Assert.AreEqual("22", (stmt.Value as LiteralExpression).Value);
		}

		[Test]
		public void ExplicitFieldsDeclarationWithInitialization()
		{
			String contents = 
				"class MyClass \r\n" + 
				"  @myfield:int = 1" + 
				"  @otherfield:string = \"hammett\" " + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(2, ClassDefinitionStatement.Statements.Count);

//			FieldDeclarationStatement stmt = ClassDefinitionStatement.Statements[0] as FieldDeclarationStatement;
//			Assert.IsNotNull(stmt);
//			Assert.AreEqual("@@myfield", (stmt.Target as StaticFieldReferenceExpression).Name);
//			Assert.AreEqual("1", (stmt.Value as LiteralExpression).Value);
//
//			stmt = ClassDefinitionStatement.Statements[1] as FieldDeclarationStatement;
//			Assert.IsNotNull(stmt);
//			Assert.AreEqual("@@otherfield", (stmt.Target as StaticFieldReferenceExpression).Name);
//			Assert.AreEqual("22", (stmt.Value as LiteralExpression).Value);
		}

		[Test]
		[Ignore("Currently not supported")]
		public void MethodInvocationsOnClassBody()
		{
			String contents = 
				"class MyClass \r\n" + 
				"  attr_accessor (:name) \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(1, ClassDefinitionStatement.Statements.Count);

			ExpressionStatement stmt = ClassDefinitionStatement.Statements[0] as ExpressionStatement;
			Assert.IsNotNull(stmt);
			Assert.IsNotNull(stmt.Expression);

			MethodInvokeExpression minv = stmt.Expression as MethodInvokeExpression;
			Assert.IsNotNull(minv);
			Assert.IsNotNull(minv.Target);
			Assert.IsNotNull(minv.Arguments);
			Assert.AreEqual(1, minv.Arguments.Count);
			Assert.AreEqual("attr_accessor", (minv.Target as IdentifierReferenceExpression).Name);
			
			SymbolExpression se = minv.Arguments[0] as SymbolExpression;
			Assert.IsNotNull(se);
			Assert.AreEqual(":name", se.Name);
		}

		[Test]
		public void AssignmentsAndAccessLevels()
		{
			String contents = 
				"class MyClass \r\n" + 
				"private \r\n" +
				"  @@myfield = 1 \r\n" + 
				"public \r\n" +
				"  @@otherfield = 2 \r\n" + 
				"  @@someother = 3 \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(3, ClassDefinitionStatement.Statements.Count);

			FieldDeclarationStatement stmt = ClassDefinitionStatement.Statements[0] as FieldDeclarationStatement;
			Assert.IsNotNull(stmt);
			Assert.AreEqual(AccessLevel.Private, stmt.ScopeAccessLevel);
			Assert.AreEqual("@@myfield", (stmt.Target as StaticFieldReferenceExpression).Name);
			Assert.AreEqual("1", (stmt.Value as LiteralExpression).Value);

			stmt = ClassDefinitionStatement.Statements[1] as FieldDeclarationStatement;
			Assert.IsNotNull(stmt);
			Assert.AreEqual(AccessLevel.Public, stmt.ScopeAccessLevel);
			Assert.AreEqual("@@otherfield", (stmt.Target as StaticFieldReferenceExpression).Name);
			Assert.AreEqual("2", (stmt.Value as LiteralExpression).Value);

			stmt = ClassDefinitionStatement.Statements[2] as FieldDeclarationStatement;
			Assert.IsNotNull(stmt);
			Assert.AreEqual(AccessLevel.Public, stmt.ScopeAccessLevel);
			Assert.AreEqual("@@someother", (stmt.Target as StaticFieldReferenceExpression).Name);
			Assert.AreEqual("3", (stmt.Value as LiteralExpression).Value);
		}

		[Test]
		public void AssignmentsAndMethods()
		{
			String contents = 
				"class MyClass \r\n" + 
				"private \r\n" +
				"  @@myfield = 1 \r\n" + 
				"   \r\n" + 
				"  def method1() \r\n" + 
				"  end \r\n" + 
				"   \r\n" + 
				"  def self.method2() \r\n" + 
				"  end \r\n" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(3, ClassDefinitionStatement.Statements.Count);

			FieldDeclarationStatement assignStmt = ClassDefinitionStatement.Statements[0] as FieldDeclarationStatement;
			MethodDefinitionStatement method1Stmt = ClassDefinitionStatement.Statements[1] as MethodDefinitionStatement;
			MethodDefinitionStatement method2Stmt = ClassDefinitionStatement.Statements[2] as MethodDefinitionStatement;

			Assert.IsNotNull(assignStmt);
			Assert.IsNotNull(method1Stmt);
			Assert.IsNotNull(method2Stmt);

			Assert.AreEqual("method1", method1Stmt.Name);
			Assert.AreEqual("method2", method2Stmt.Name);
			Assert.AreEqual("self", method2Stmt.BoundTo);
		}
	}
}