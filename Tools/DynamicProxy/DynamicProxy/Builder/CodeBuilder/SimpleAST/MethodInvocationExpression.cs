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
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for MethodInvocationExpression.
	/// </summary>
	public class MethodInvocationExpression : Expression
	{
		protected MethodInfo _method;
		protected Expression[] _args;
		protected Reference _owner;

		public MethodInvocationExpression(MethodInfo method, params Expression[] args) : 
			this(SelfReference.Self, method, args)
		{
		}

		public MethodInvocationExpression(EasyMethod method, params Expression[] args) : 
			this(SelfReference.Self, method.MethodBuilder, args)
		{
		}

		public MethodInvocationExpression(Reference owner, EasyMethod method, params Expression[] args) : 
			this(owner, method.MethodBuilder, args)
		{
		}

		public MethodInvocationExpression(Reference owner, MethodInfo method, params Expression[] args)
		{
			_owner = owner;
			_method = method;
			_args = args;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(_owner, gen);

			foreach(Expression exp in _args)
			{
				exp.Emit(member, gen);
			}

			gen.Emit(OpCodes.Call, _method);
		}
	}
}
