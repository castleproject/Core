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

namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for ConstructorInvocationExpression.
	/// </summary>
	public class ConstructorInvocationExpression : Expression
	{
		private ConstructorInfo _cmethod;
		private Expression[] _args;

		public ConstructorInvocationExpression(ConstructorInfo method, params Expression[] args)
		{
			_cmethod = method;
			_args = args;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
			
			foreach(Expression exp in _args)
			{
				exp.Emit(member, gen);
			}

			gen.Emit(OpCodes.Call, _cmethod);
		}
	}
}
