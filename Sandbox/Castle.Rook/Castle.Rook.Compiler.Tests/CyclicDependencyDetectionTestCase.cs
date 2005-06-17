using Castle.Rook.Compiler.AST.Util;
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

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;

	[TestFixture]
	public class CyclicDependencyDetectionTestCase
	{
		[Test]
		[Ignore("Need to implement that")]
		public void SimplestCase()
		{
			// @x = @y + 1
			// @y = @x + 1

			Identifier xident = new Identifier(IdentifierType.InstanceField, "x", null );
			Identifier yident = new Identifier(IdentifierType.InstanceField, "y", null );

			SingleVariableDeclarationStatement xdecl = new SingleVariableDeclarationStatement( xident );
			SingleVariableDeclarationStatement ydecl = new SingleVariableDeclarationStatement( yident );

			VariableReferenceExpression xref = new VariableReferenceExpression(xident);
			VariableReferenceExpression yref = new VariableReferenceExpression(yident);

			BinaryExpression exp1 = 
				new BinaryExpression(yref, 
				new LiteralReferenceExpression("1", LiteralReferenceType.IntLiteral), BinaryOp.Plus);

			xdecl.DependencyExpression = exp1;

			BinaryExpression exp2 = 
				new BinaryExpression(xref, 
				new LiteralReferenceExpression("1", LiteralReferenceType.IntLiteral), BinaryOp.Plus);

			ydecl.DependencyExpression = exp2;

			Assert.IsTrue( ASTUtils.IsCycled(xdecl) );
		}
	}
}
