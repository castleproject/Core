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

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for AssignStatement.
	/// </summary>
	public class AssignStatement : Statement
	{
		private Reference m_target;
		private Expression m_expression;

		public AssignStatement( Reference target, Expression expression )
		{
			m_target = target;
			m_expression = expression;
		}

		public override void Emit(IEasyMember member, System.Reflection.Emit.ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference( m_target.OwnerReference, gen );

			m_expression.Emit(member, gen);

			m_target.StoreReference(gen);
		}
	}
}
