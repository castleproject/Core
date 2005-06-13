using Castle.Rook.Compiler.Visitors;
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

	public enum BinaryOp
	{
		Undefined,
		And,
		And2,
		Or,
		Or2,
		Xor,
		LessThan,
		GreaterThan,
		Equal,
		GreaterEqual,
		LessEqual,
		NotEqual,
		Mult,
		Div,
		Mod,
		Plus, 
		Minus
	}

	public class BinaryExpression : AbstractExpression
	{
		private readonly BinaryOp op;
		private readonly IExpression lhs;
		private readonly IExpression rhs;

		public BinaryExpression(IExpression lhs, IExpression rhs, BinaryOp op)
		{
			this.op = op;
			this.lhs = lhs;
			this.rhs = rhs;
		}

		public BinaryOp Operation
		{
			get { return op; }
		}

		public IExpression LeftHandSide
		{
			get { return lhs; }
		}

		public IExpression RightHandSide
		{
			get { return rhs; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitBinaryExpression(this);
		}

		public override IExpression Accept(IExpressionAttrVisitor visitor)
		{
			return visitor.VisitBinaryExpression(this);
		}
	}
}
