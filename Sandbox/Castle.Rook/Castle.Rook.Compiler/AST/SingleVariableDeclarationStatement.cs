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


	public class SingleVariableDeclarationStatement : AbstractStatement
	{
		private readonly Identifier ident;
		private IExpression initExp;
		private IExpression depExp;

		public SingleVariableDeclarationStatement(Identifier ident)
		{
			this.ident = ident;
		}

		public Identifier Identifier
		{
			get { return ident; }
		}

		public IExpression InitExp
		{
			get { return initExp; }
			set
			{
				initExp = value; 
				if (value != null) value.Parent = this;
			}
		}

		/// <summary>
		/// Used to resolve the type of the target variable/field
		/// </summary>
		public IExpression DependencyExpression
		{
			get { return depExp; }
			set
			{
				depExp = value; 
				if (value != null) value.Parent = this;
			}
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitSingleVariableDeclarationStatement(this);
		}

		public void ConvertInitExpressionToDependency()
		{
			depExp = initExp;
			initExp = null;
		}
	}
}
