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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	[CLSCompliant(false)]
	public class ConstructorInvocationStatement : Statement
	{
		private readonly ConstructorInfo cmethod;
		private readonly Expression[] args;

		public ConstructorInvocationStatement(ConstructorInfo method, params Expression[] args)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (args == null) throw new ArgumentNullException("args");
			
			this.cmethod = method;
			this.args = args;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
			
			foreach(Expression exp in args)
			{
				exp.Emit(member, gen);
			}

			gen.Emit(OpCodes.Call, cmethod);
		}
	}
}
