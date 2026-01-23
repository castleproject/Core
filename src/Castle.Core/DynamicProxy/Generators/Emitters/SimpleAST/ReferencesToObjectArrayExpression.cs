// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

#nullable enable

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Internal;

	internal class ReferencesToObjectArrayExpression : IExpression
	{
		private readonly ArgumentReference[] arguments;

		public ReferencesToObjectArrayExpression(ArgumentReference[] arguments)
		{
			this.arguments = arguments;
		}

		public void Emit(ILGenerator gen)
		{
			var argumentsArray = gen.DeclareLocal(typeof(object?[]));

			gen.Emit(OpCodes.Ldc_I4, arguments.Length);
			gen.Emit(OpCodes.Newarr, typeof(object));
			gen.Emit(OpCodes.Stloc, argumentsArray);

			for (var i = 0; i < arguments.Length; i++)
			{
				gen.Emit(OpCodes.Ldloc, argumentsArray);
				gen.Emit(OpCodes.Ldc_I4, i);

				var argument = arguments[i];
				Reference dereferencedArgument = argument.Type.IsByRef ? new IndirectReference(argument) : argument;
				var dereferencedArgumentType = dereferencedArgument.Type;

#if FEATURE_BYREFLIKE
				if (dereferencedArgumentType.IsByRefLikeSafe())
				{
					// The by-ref-like argument value cannot be put into the `object[]` array,
					// because it cannot be boxed. We need to replace it with some other value.

					// For now, we just erase it by substituting `null`:
					gen.Emit(OpCodes.Ldnull);
					gen.Emit(OpCodes.Stelem_Ref);

					continue;
				}
#endif

				dereferencedArgument.Emit(gen);

				if (dereferencedArgumentType.IsValueType)
				{
					gen.Emit(OpCodes.Box, dereferencedArgumentType);
				}
				else if (dereferencedArgumentType.IsGenericParameter)
				{
					gen.Emit(OpCodes.Box, dereferencedArgumentType);
				}

				gen.Emit(OpCodes.Stelem_Ref);
			}

			gen.Emit(OpCodes.Ldloc, argumentsArray);
		}
	}
}