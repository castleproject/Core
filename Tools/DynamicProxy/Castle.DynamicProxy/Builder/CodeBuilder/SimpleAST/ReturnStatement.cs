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

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for ReturnStatement.
	/// </summary>
	[CLSCompliant(false)]
	public class ReturnStatement : Statement
	{
		private Reference _reference;
		private Expression _expression;

		public ReturnStatement( )
		{
		}

		public ReturnStatement( Reference reference )
		{
			_reference = reference;
		}

		public ReturnStatement( Expression expression )
		{
			_expression = expression;
		}

		public override void Emit(IEasyMember member, System.Reflection.Emit.ILGenerator gen)
		{
			if (_reference != null)
			{
				ArgumentsUtil.EmitLoadOwnerAndReference(_reference, gen);
			}
			else if (_expression != null)
			{
				_expression.Emit(member, gen);
			}
			else
			{
				if (member.ReturnType != typeof(void))
				{
					OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, member.ReturnType);
				}
			}
			gen.Emit(OpCodes.Ret);
		}
	}
}
