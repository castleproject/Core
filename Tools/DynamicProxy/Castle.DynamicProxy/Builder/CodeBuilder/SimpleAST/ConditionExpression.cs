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

namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection.Emit;
	using System.Collections;

	/// <summary>
	/// Summary description for ConditionExpression.
	/// </summary>
	[CLSCompliant(false)]
	public class ConditionExpression : Expression
	{
		private OpCode _operation = OpCodes.Brfalse_S;
		private ArrayList _trueStmts;
		private ArrayList _falseStmts;
		private Expression _left;
		private Expression _right;

		public ConditionExpression( Expression left ) : this(OpCodes.Brfalse_S, left)
		{
		}

		public ConditionExpression( OpCode operation, Expression left ) : this(operation, left, null)
		{
		}

		public ConditionExpression( OpCode operation, Expression left, Expression right )
		{
			_trueStmts = new ArrayList();
			_falseStmts = new ArrayList();

			_operation = operation;
			_left = left;
			_right = right;
		}

		public void AddTrueStatement( Statement stmt )
		{
			_trueStmts.Add( stmt );
		}

		public void AddFalseStatement( Statement stmt )
		{
			_falseStmts.Add( stmt );
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			if (OpCodes.Brfalse.Equals(_operation) || 
				OpCodes.Brfalse_S.Equals(_operation) || 
				OpCodes.Brtrue.Equals(_operation) ||
				OpCodes.Brtrue_S.Equals(_operation) )
			{
				// Unary operators
				_left.Emit(member, gen);
			}
			else
			{
				// Binary operators
				_left.Emit(member, gen);
				_right.Emit(member, gen);
			}

			Label truePart = gen.DefineLabel(); 
			Label exitPart = gen.DefineLabel(); 
			
			gen.Emit(_operation, truePart);
			
			if (_falseStmts.Count != 0)
			{
				foreach(Statement stmt in _falseStmts)
				{
					stmt.Emit(member, gen);
				}
			}
			
			gen.Emit(OpCodes.Br_S, exitPart);

			gen.MarkLabel(truePart);
			if (_trueStmts.Count != 0)
			{
				foreach(Statement stmt in _trueStmts)
				{
					stmt.Emit(member, gen);
				}
			}

			gen.MarkLabel(exitPart);
		}
	}
}
