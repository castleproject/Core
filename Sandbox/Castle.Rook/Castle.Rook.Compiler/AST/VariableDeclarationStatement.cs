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


	public class VariableDeclarationStatement : AbstractStatement
	{
		private readonly AccessLevel accessLevel;
		private ExpressionCollection decls;
		private ExpressionCollection initExps;

		public VariableDeclarationStatement(AccessLevel accessLevel)
		{
			decls = new ExpressionCollection(this);
			initExps = new ExpressionCollection(this);
			this.accessLevel = accessLevel;
		}

		public void Add(TypeDeclarationExpression tdstmt)
		{
			decls.Add(tdstmt);
		}

		public void AddInitExp(IExpression exp)
		{
			initExps.Add(exp);
		}

		public ExpressionCollection Declarations
		{
			get { return decls; }
		}

		public ExpressionCollection InitExpressions
		{
			get { return initExps; }
		}

		public AccessLevel AccessLevel
		{
			get { return accessLevel; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitVariableDeclarationStatement(this);
		}
	}
}
