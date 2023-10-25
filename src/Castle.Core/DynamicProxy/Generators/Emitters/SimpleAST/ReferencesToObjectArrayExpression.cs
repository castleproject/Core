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

	using Castle.DynamicProxy.Tokens;

	internal class ReferencesToObjectArrayExpression : IExpression
	{
		private readonly TypeReference[] args;

#if FEATURE_BYREFLIKE
		private readonly MethodInfo proxiedMethod;
		private readonly IByRefLikeConverterSelector byRefLikeConverterSelector;

		public ReferencesToObjectArrayExpression(MethodInfo proxiedMethod, IByRefLikeConverterSelector byRefLikeConverterSelector, params TypeReference[] args)
		{
			this.proxiedMethod = proxiedMethod;
			this.byRefLikeConverterSelector = byRefLikeConverterSelector;
			this.args = args;
		}
#else
		public ReferencesToObjectArrayExpression(params TypeReference[] args)
		{
			this.args = args;
		}
#endif

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

				var reference = args[i];

#if FEATURE_BYREFLIKE
				if (reference.Type.IsByRefLike)
				{
					var converterType = byRefLikeConverterSelector?.SelectConverterType(proxiedMethod, i, reference.Type);
					if (converterType != null)
					{
						// instantiate the by-ref-like value converter:
						gen.Emit(OpCodes.Ldtoken, converterType);
						gen.Emit(OpCodes.Call, TypeMethods.GetTypeFromHandle);
						gen.EmitCall(OpCodes.Call, ActivatorMethods.CreateInstance, null);

						// invoke it:
						var boxMethodOnConverter = converterType.GetMethod("Box");
						// (TODO: isn't there a nicer way to figure out whether or not the argument was passed by-ref,
						// and then ensure that we end up with the argument's address on the evaluation stack?)
						var argumentReference = (ArgumentReference)(reference is IndirectReference ? reference.OwnerReference : reference);
						gen.Emit(argumentReference.Type.IsByRef ? OpCodes.Ldarg_S : OpCodes.Ldarga_S, argumentReference.Position);
						gen.EmitCall(OpCodes.Callvirt, boxMethodOnConverter, null);
					}
					else
					{
						// no by-ref-like value converter is available, fall back to substituting the argument value with `null`
						// because the logic further down would attempt to box it (which isn't allowed for by-ref-like values):
						gen.Emit(OpCodes.Ldnull);
					}

					gen.Emit(OpCodes.Stelem_Ref);
					continue;
				}
#endif

				ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);

				if (reference.Type.IsByRef)
				{
					throw new NotSupportedException();
				}

				if (reference.Type.IsValueType)
				{
					gen.Emit(OpCodes.Box, reference.Type);
				}
				else if (reference.Type.IsGenericParameter)
				{
					gen.Emit(OpCodes.Box, reference.Type);
				}

				gen.Emit(OpCodes.Stelem_Ref);
			}

			gen.Emit(OpCodes.Ldloc, local);
		}
	}
}