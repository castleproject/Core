// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Collections;

	using Castle.Rook.Compiler.Visitors;


	public class ForStatement : AbstractStatement
	{
		private StatementCollection statements;
		private IList varRefs = new ArrayList();
		private IExpression evalExp;

		public ForStatement()
		{
			statements = new StatementCollection(this);
		}

		public void AddVarRef(VariableReferenceExpression vre)
		{
			varRefs.Add(vre);
		}

		public StatementCollection Statements
		{
			get { return statements; }
		}

		public IExpression EvalExp
		{
			get { return evalExp; }
			set { evalExp = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitForStatement(this);
		}
	}
}
