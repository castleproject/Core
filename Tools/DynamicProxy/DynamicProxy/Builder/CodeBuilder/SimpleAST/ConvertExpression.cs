using System.Reflection.Emit;
using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
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

	/// <summary>
	/// Summary description for ConvertExpression.
	/// </summary>
	public class ConvertExpression : Expression
	{
		private Type m_target;
		private Type m_fromType;
		private Expression m_right;

		public ConvertExpression( Type targetType, Expression right ) : this(targetType, typeof(object), right)
		{
		}

		public ConvertExpression( Type targetType, Type fromType, Expression right )
		{
			m_target = targetType;
			m_fromType = fromType;
			m_right = right;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			m_right.Emit(member, gen);

			if (m_fromType == m_target)
			{
				return;
			}

			if (m_target.IsValueType && !m_fromType.IsValueType)
			{
				gen.Emit(OpCodes.Unbox, m_target);
				OpCodeUtil.ConvertTypeToOpCode(gen, m_target);
				return;
			}
			else if (!m_target.IsValueType && m_fromType.IsValueType)
			{
				gen.Emit(OpCodes.Box, m_fromType);
			}
			
			if (m_target != typeof(Object))
			{
				gen.Emit(OpCodes.Castclass, m_target);
			}
		}
	}
}
