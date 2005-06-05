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

namespace Castle.Rook.Compiler.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;

	[TestFixture]
	public class BreadthFirstVisitorTestCase : AbstractContainerTestCase
	{
		[Test]
		public void BreadthFirstTraversal()
		{
			String contents = 
				"gx1:int = 1		\r\n" + 
				"gx2:int			\r\n" + 
				"					\r\n" + 
				"class Something    \r\n" + 
				"  @inst1:int       \r\n" + 
				"					\r\n" + 
				"  def method1()    \r\n" + 
				"    @inst2 += 1    \r\n" + 
				"    gx1 += 1		\r\n" + 
				"  end				\r\n" + 
				"					\r\n" + 
				"  @inst2:int       \r\n" + 
				"					\r\n" + 
				"  def method1()    \r\n" + 
				"    @inst3:int     \r\n" + 
				"    @inst1 += 1    \r\n" + 
				"  end				\r\n" + 
				"					\r\n" + 
				"end				\r\n" + 
				"";

			SourceUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(3, unit.Statements.Count);

			MyBreadthFirstVisitor visitor = new MyBreadthFirstVisitor();
			visitor.VisitNode( unit );
			Assert.AreEqual( 8, visitor.VisitedNodes.Count );
			Assert.AreEqual( typeof(MultipleVariableDeclarationStatement), visitor.VisitedNodes[0].GetType() );
			Assert.AreEqual( typeof(MultipleVariableDeclarationStatement), visitor.VisitedNodes[1].GetType() );
			Assert.AreEqual( typeof(TypeDefinitionStatement), visitor.VisitedNodes[2].GetType() );
			Assert.AreEqual( typeof(MultipleVariableDeclarationStatement), visitor.VisitedNodes[3].GetType() );
			Assert.AreEqual( typeof(MethodDefinitionStatement), visitor.VisitedNodes[4].GetType() );
			Assert.AreEqual( typeof(MultipleVariableDeclarationStatement), visitor.VisitedNodes[5].GetType() );
			Assert.AreEqual( typeof(MethodDefinitionStatement), visitor.VisitedNodes[6].GetType() );
			Assert.AreEqual( typeof(MultipleVariableDeclarationStatement), visitor.VisitedNodes[7].GetType() );

			Identifier tde = null;
			tde = (visitor.VisitedNodes[0] as MultipleVariableDeclarationStatement).Identifiers[0] as Identifier;
			Assert.AreEqual("gx1", tde.Name);

			tde = (visitor.VisitedNodes[1] as MultipleVariableDeclarationStatement).Identifiers[0] as Identifier;
			Assert.AreEqual("gx2", tde.Name);

			tde = (visitor.VisitedNodes[3] as MultipleVariableDeclarationStatement).Identifiers[0] as Identifier;
			Assert.AreEqual("@inst1", tde.Name);

			tde = (visitor.VisitedNodes[5] as MultipleVariableDeclarationStatement).Identifiers[0] as Identifier;
			Assert.AreEqual("@inst2", tde.Name);

			tde = (visitor.VisitedNodes[7] as MultipleVariableDeclarationStatement).Identifiers[0] as Identifier;
			Assert.AreEqual("@inst3", tde.Name);
		}	
	}


	public class MyBreadthFirstVisitor : BreadthFirstVisitor
	{
		private ArrayList visitedNodes = new ArrayList();

		public IList VisitedNodes
		{
			get { return visitedNodes; }
		}

		public override bool VisitNamespace(NamespaceDeclaration ns)
		{
			visitedNodes.Add(ns);

			return base.VisitNamespace(ns);
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			visitedNodes.Add(typeDef);

			return base.VisitTypeDefinitionStatement(typeDef);
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			visitedNodes.Add(methodDef);

			return base.VisitMethodDefinitionStatement(methodDef);
		}

		public override bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
		{
			visitedNodes.Add(varDecl);

			return base.VisitMultipleVariableDeclarationStatement(varDecl);
		}
	}
}
