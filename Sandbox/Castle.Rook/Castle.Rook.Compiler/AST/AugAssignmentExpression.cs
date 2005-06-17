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

	public enum AugType
	{
		Undefined,
		PlusAssign,
		MinusAssign,
		MultAssign,
		DivAssign,
		ModAssign,
		BitwiseAndAssign,
		BitwiseOrAssign,
		BitwiseXorAssign
	}

	public class AugAssignmentExpression : AbstractExpression
	{
		private readonly AugType augType;
		private IExpression target;
		private IExpression value;

		public AugAssignmentExpression(IExpression target, IExpression value, AugType augType)
		{
			this.augType = augType;
			this.target = target;	target.Parent = this;
			this.value = value;		value.Parent = this;
		}

		public IExpression Target
		{
			get { return target; }
			set { target = value; }
		}

		public IExpression Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public AugType AugType
		{
			get { return augType; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitAugAssignmentExpression(this);
		}

		public override IExpression Accept(IExpressionAttrVisitor visitor)
		{
			return visitor.VisitAugAssignmentExpression(this);
		}
	}
}
