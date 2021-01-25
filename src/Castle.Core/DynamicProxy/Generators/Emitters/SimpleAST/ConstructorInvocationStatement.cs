// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	internal class ConstructorInvocationStatement : IStatement
	{
		private readonly IExpression[] args;
		private readonly ConstructorInfo cmethod;

		public ConstructorInvocationStatement(Type baseType)
			: this(GetDefaultConstructor(baseType))
		{
		}

		public ConstructorInvocationStatement(ConstructorInfo method, params IExpression[] args)
		{
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			cmethod = method;
			this.args = args;
		}

		public void Emit(IMemberEmitter member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);

			foreach (var exp in args)
			{
				exp.Emit(member, gen);
			}

			gen.Emit(OpCodes.Call, cmethod);
		}

		private static ConstructorInfo GetDefaultConstructor(Type baseType)
		{
			var type = baseType;
			if (type.ContainsGenericParameters)
			{
				type = type.GetGenericTypeDefinition();
				// need to get generic type definition, otherwise the GetConstructor method might throw NotSupportedException
			}

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return type.GetConstructor(flags, null, Type.EmptyTypes, null);
		}
	}
}