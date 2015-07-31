// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// </summary>
	public class ReferencesToObjectArrayExpression : Expression
	{
		private readonly TypeReference[] args;

		public ReferencesToObjectArrayExpression(params TypeReference[] args)
		{
			this.args = args;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
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

				ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);

				if (reference.Type.GetTypeInfo().IsByRef)
				{
					throw new NotSupportedException();
				}

				if (reference.Type.GetTypeInfo().IsValueType)
#if NETCORE
				{
					gen.Emit(OpCodes.Box, reference.Type); 
				}
#else
				{
					gen.Emit(OpCodes.Box, reference.Type.UnderlyingSystemType);
				}
#endif
				else if (reference.Type.GetTypeInfo().IsGenericParameter)
				{
					gen.Emit(OpCodes.Box, reference.Type);
				}

				gen.Emit(OpCodes.Stelem_Ref);
			}

			gen.Emit(OpCodes.Ldloc, local);
		}
	}
}