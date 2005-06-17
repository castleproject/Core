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


	public class MethodInvocationExpression : AbstractExpression
	{
		private readonly IExpression target;
		private readonly ExpressionCollection args;

		public MethodInvocationExpression(IExpression target)
		{
			this.target = target;	
			this.target.Parent = this;
			this.args = new ExpressionCollection(this);
		}

		public IExpression Target
		{
			get { return target; }
		}

		public ExpressionCollection Arguments
		{
			get { return args; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitMethodInvocationExpression(this);
		}

		public override IExpression Accept(IExpressionAttrVisitor visitor)
		{
			return visitor.VisitMethodInvocationExpression(this);
		}
	}
}
