// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	public class ConditionExpression : Expression
	{
		private OpCode m_operation = OpCodes.Brfalse_S;
		private ArrayList m_trueStmts;
		private ArrayList m_falseStmts;
		private Expression m_left;
		private Expression m_right;

		public ConditionExpression( Expression left ) : this(OpCodes.Brfalse_S, left)
		{
		}

		public ConditionExpression( OpCode operation, Expression left ) : this(operation, left, null)
		{
		}

		public ConditionExpression( OpCode operation, Expression left, Expression right )
		{
			m_trueStmts = new ArrayList();
			m_falseStmts = new ArrayList();

			m_operation = operation;
			m_left = left;
			m_right = right;
		}

		public void AddTrueStatement( Statement stmt )
		{
			m_trueStmts.Add( stmt );
		}

		public void AddFalseStatement( Statement stmt )
		{
			m_falseStmts.Add( stmt );
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			if (OpCodes.Brfalse.Equals(m_operation) || 
				OpCodes.Brfalse_S.Equals(m_operation) || 
				OpCodes.Brtrue.Equals(m_operation) ||
				OpCodes.Brtrue_S.Equals(m_operation) )
			{
				// Unary operators
				m_left.Emit(member, gen);
			}
			else
			{
				// Binary operators
				m_left.Emit(member, gen);
				m_right.Emit(member, gen);
			}

			Label truePart = gen.DefineLabel(); 
			Label exitPart = gen.DefineLabel(); 
			
			gen.Emit(m_operation, truePart);
			
			if (m_falseStmts.Count != 0)
			{
				foreach(Statement stmt in m_falseStmts)
				{
					stmt.Emit(member, gen);
				}
			}
			
			gen.Emit(OpCodes.Br_S, exitPart);

			gen.MarkLabel(truePart);
			if (m_trueStmts.Count != 0)
			{
				foreach(Statement stmt in m_trueStmts)
				{
					stmt.Emit(member, gen);
				}
			}

			gen.MarkLabel(exitPart);
		}
	}
}
