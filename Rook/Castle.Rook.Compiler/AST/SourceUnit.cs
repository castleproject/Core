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

namespace Castle.Rook.Compiler.AST
{
	using System;

	using Castle.Rook.Compiler.Visitors;


	public class SourceUnit : ASTNode
	{
		private readonly CompilationUnit compilationUnit;
		private readonly string filename;

		private ASTNodeCollection statements;
		private ASTNodeCollection namespaces;

		public SourceUnit(CompilationUnit compilationUnit, String filename)
		{
			statements = new ASTNodeCollection(this);
			namespaces = new ASTNodeCollection(this);
			this.compilationUnit = compilationUnit;
			this.filename = filename;
		}

		public CompilationUnit CompilationUnit
		{
			get { return compilationUnit; }
		}

		public ASTNodeCollection Statements
		{
			get { return statements; }
		}

		public ASTNodeCollection Namespaces
		{
			get { return namespaces; }
		}

		public string Filename
		{
			get { return filename; }
		}

		public override void RemoveChild(IASTNode node)
		{
			statements.Remove( node );
			namespaces.Remove( node );
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitSourceUnit(this);
			return true;
		}
	}
}
