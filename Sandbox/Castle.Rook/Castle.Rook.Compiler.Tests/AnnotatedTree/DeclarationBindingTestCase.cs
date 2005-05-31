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

namespace Castle.Rook.Compiler.Tests.AnnotatedTree
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services.Passes;


	[TestFixture]
	public class DeclarationBindingTestCase : AbstractContainerTestCase
	{
		[Test]
		public void MethodUsingGlobals()
		{
			String contents = 
				"@x:int = 1       \r\n" + 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x += 1        \r\n" + 
				"end              \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void InitExpressionBind()
		{
			String contents = 
				"@x:int, @y:long, @z:int = 1,2       \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(1, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			AssertNoErrorOrWarnings();

			SingleVariableDeclarationStatement decl1 = unit.Statements[0] as SingleVariableDeclarationStatement;
			
			SingleVariableDeclarationStatement decl2 = unit.Statements[1] as SingleVariableDeclarationStatement;
			
			SingleVariableDeclarationStatement decl3 = unit.Statements[2] as SingleVariableDeclarationStatement;

			Assert.IsNotNull(decl1.InitExp);
			Assert.IsNotNull(decl2.InitExp);
			Assert.IsNull(decl3.InitExp);

			Assert.AreEqual( typeof(LiteralReferenceExpression), decl1.InitExp.GetType() );
			Assert.AreEqual( typeof(LiteralReferenceExpression), decl2.InitExp.GetType() );
			Assert.AreEqual( "1", (decl1.InitExp as LiteralReferenceExpression).Content );
			Assert.AreEqual( "2", (decl2.InitExp as LiteralReferenceExpression).Content );
		}

		[Test]
		public void InvalidRedefinition1()
		{
			String contents = 
				"@x:int = 1       \r\n" + 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x:int = 1     \r\n" + 
				"end              \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Sorry but '@x' is already defined.\r\n", message);
		}

		[Test]
		public void InvalidRedefinition2()
		{
			String contents = 
				"@x:int = 1       \r\n" + 
				"@x:int = 2       \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Sorry but '@x' is already defined.\r\n", message);
		}

		[Test]
		public void NestedDefinitionOfInstanceVars()
		{
			String contents = 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x:int = 1     \r\n" + 
				"end              \r\n" + 
				"                 \r\n" + 
				"def some2()      \r\n" + 
				"  @y += 2        \r\n" + 
				"end              \r\n" + 
				"                 \r\n" + 
				"@y:int = 1       \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(3, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			Assert.IsNotNull(unit);
			Assert.AreEqual(4, unit.Statements.Count);

			AssertNoErrorOrWarnings();

			MethodDefinitionStatement m1Stmt = unit.Statements[0] as MethodDefinitionStatement;
			MethodDefinitionStatement m2Stmt = unit.Statements[1] as MethodDefinitionStatement;
			SingleVariableDeclarationStatement varDecl1 = unit.Statements[2] as SingleVariableDeclarationStatement;
			SingleVariableDeclarationStatement varDecl2 = unit.Statements[3] as SingleVariableDeclarationStatement;

			Assert.IsNotNull(m1Stmt);
			Assert.IsNotNull(m2Stmt);
			Assert.IsNotNull(varDecl1);
			Assert.IsNotNull(varDecl2);

			Assert.AreEqual( "some", m1Stmt.FullName );
			ExpressionStatement expStmt = m1Stmt.Statements[0] as ExpressionStatement;
			AssignmentExpression assignExp = expStmt.Expression as AssignmentExpression;
			Assert.AreEqual( "@x", (assignExp.Target as VariableReferenceExpression).Identifier.Name );
			Assert.AreEqual( "1", (assignExp.Value as LiteralReferenceExpression).Content );
		
			Assert.AreEqual( "some2", m2Stmt.FullName );
			expStmt = m2Stmt.Statements[0] as ExpressionStatement;
			AugAssignmentExpression augExp = expStmt.Expression as AugAssignmentExpression;
			Assert.AreEqual( "@y", (augExp.Target as VariableReferenceExpression).Identifier.Name );
			Assert.AreEqual( "2", (augExp.Value as LiteralReferenceExpression).Content );
		}

		[Test]
		public void InvalidRedefinition3()
		{
			String contents = 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x:int = 1     \r\n" + 
				"end              \r\n" + 
				"                 \r\n" + 
				"def some2()      \r\n" + 
				"  @x:int = 2     \r\n" + 
				"end              \r\n" + 
				"                 \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Sorry but '@x' is already defined.\r\n", message);
		}
	}
}
