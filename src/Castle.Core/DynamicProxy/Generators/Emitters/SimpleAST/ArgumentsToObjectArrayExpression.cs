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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Internal;

	internal sealed class ArgumentsToObjectArrayExpression : IExpression
	{
		private readonly Location[] args;

		public ArgumentsToObjectArrayExpression(params Location[] args)
		{
			this.args = args;
		}

		public void Emit(ILGenerator gen)
		{
			var local = gen.DeclareLocal(typeof(object[]));

			gen.Emit(OpCodes.Ldc_I4, args.Length);
			gen.Emit(OpCodes.Newarr, typeof(object));
			gen.Emit(OpCodes.Stloc, local);

			for (var i = 0; i < args.Length; i++)
			{
				gen.Emit(OpCodes.Ldloc, local);
				gen.Emit(OpCodes.Ldc_I4, i);

				var arg = args[i];

#if FEATURE_BYREFLIKE
				if (arg.Type.IsByRefLikeSafe())
				{
					// The by-ref-like argument value cannot be put into the `object[]` array,
					// because it cannot be boxed. We need to replace it with some other value.

					// For now, we just erase it by substituting `null`:
					gen.Emit(OpCodes.Ldnull);
					gen.Emit(OpCodes.Stelem_Ref);

					continue;
				}
#endif

				arg.Emit(gen);

				if (arg.Type.IsByRef)
				{
					throw new NotSupportedException();
				}

				if (arg.Type.IsValueType)
				{
					gen.Emit(OpCodes.Box, arg.Type);
				}
				else if (arg.Type.IsGenericParameter)
				{
					gen.Emit(OpCodes.Box, arg.Type);
				}

				gen.Emit(OpCodes.Stelem_Ref);
			}

			gen.Emit(OpCodes.Ldloc, local);
		}
	}
}