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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;
	using System.Diagnostics;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	internal class ConvertArgumentFromObjectExpression : IExpression
	{
		private readonly IExpression obj;
		private Type dereferencedArgumentType;

		public ConvertArgumentFromObjectExpression(IExpression obj, Type dereferencedArgumentType)
		{
			Debug.Assert(dereferencedArgumentType.IsByRef == false);

			this.obj = obj;
			this.dereferencedArgumentType = dereferencedArgumentType;
		}

		public void Emit(ILGenerator gen)
		{
			obj.Emit(gen);

			if (dereferencedArgumentType == typeof(object))
			{
				return;
			}

			if (dereferencedArgumentType.IsValueType)
			{
#if FEATURE_BYREFLIKE
				if (dereferencedArgumentType.IsByRefLikeSafe())
				{
					gen.Emit(OpCodes.Ldtoken, dereferencedArgumentType);
					gen.Emit(OpCodes.Call, TypeMethods.GetTypeFromHandle);
					gen.Emit(OpCodes.Call, ByRefLikeReferenceMethods.GetPtr);
					gen.Emit(OpCodes.Ldobj, dereferencedArgumentType);
				}
				else
#endif
				{
					// Unbox conversion
					// Assumes fromType is a boxed value
					// if we can, we emit a box and ldind, otherwise, we will use unbox.any
					if (LdindOpCodesDictionary.Instance[dereferencedArgumentType] != LdindOpCodesDictionary.EmptyOpCode)
					{
						gen.Emit(OpCodes.Unbox, dereferencedArgumentType);
						OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, dereferencedArgumentType);
					}
					else
					{
						gen.Emit(OpCodes.Unbox_Any, dereferencedArgumentType);
					}
				}
			}
			else
			{
				// Possible down-cast
				if (dereferencedArgumentType.IsGenericParameter)
				{
					gen.Emit(OpCodes.Unbox_Any, dereferencedArgumentType);
				}
				else if (dereferencedArgumentType.IsGenericType)
				{
					gen.Emit(OpCodes.Castclass, dereferencedArgumentType);
				}
				else if (dereferencedArgumentType.IsSubclassOf(typeof(object)))
				{
					gen.Emit(OpCodes.Castclass, dereferencedArgumentType);
				}
			}
		}
	}
}
