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
		Equal,
		LessThan,
		GreaterThan,
		GreaterEqual,
		LessEqual,
		NotEqual,
		IsA, 
		Plus,
		Minus,
		And,
		And2,
		Or,
		Or2,
		Xor,
		Mult,
		Div,
		Mod,
	}

	public class BinaryExpression : Expression
	{
		private IExpression left;
		private IExpression right;
		private BinaryOp op;

		public BinaryExpression(IExpression left, IExpression right, BinaryOp op)
		{
			this.left = left;
			this.right = right;
			this.op = op;
		}

		public IExpression Left
		{
			get { return left; }
			set { left = value; }
		}

		public IExpression Right
		{
			get { return right; }
			set { right = value; }
		}

		public BinaryOp Op
		{
			get { return op; }
			set { op = value; }
		}
	}
}
