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
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for LoadRefArrayElementExpression.
	/// </summary>
	public class LoadRefArrayElementExpression : Expression
	{
		private FixedReference m_index; 
		private Reference m_arrayReference;

		public LoadRefArrayElementExpression( int index, Reference arrayReference ) : 
			this( new FixedReference(index), arrayReference )
		{
		}

		public LoadRefArrayElementExpression( FixedReference index, Reference arrayReference )
		{
			m_index = index;
			m_arrayReference = arrayReference;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(m_arrayReference, gen);
			ArgumentsUtil.EmitLoadOwnerAndReference(m_index, gen);
			gen.Emit(OpCodes.Ldelem_Ref);
		}
	}
}
