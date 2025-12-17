// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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
	using System.Diagnostics;
	using System.Reflection.Emit;

	internal sealed class ArrayElementReference : Reference
	{
		private readonly Reference array;
		private readonly int index;

		public ArrayElementReference(Reference array, int index)
			: base(array.Type.GetElementType()!)
		{
			Debug.Assert(array.Type.IsArray);

			// The below methods have the `ldelem.ref` and `stelem.ref` opcodes hardcoded,
			// which is only correct for reference types. Once we start using this class
			// for arrays of primitive types, enums, value types, or generic parameters,
			// we will need to revisit this.
			Debug.Assert(array.Type.GetElementType()!.IsClass || array.Type.GetElementType()!.IsInterface);

			this.array = array;
			this.index = index;
		}

		public override void Emit(ILGenerator gen)
		{
			array.Emit(gen);
			gen.Emit(OpCodes.Ldc_I4, index);
			gen.Emit(OpCodes.Ldelem_Ref);
		}

		public override void EmitAddress(ILGenerator gen)
		{
			array.Emit(gen);
			gen.Emit(OpCodes.Ldc_I4, index);
			gen.Emit(OpCodes.Ldelema);
		}

		public override void EmitStore(IExpression value, ILGenerator gen)
		{
			array.Emit(gen);
			gen.Emit(OpCodes.Ldc_I4, index);
			value.Emit(gen);
			gen.Emit(OpCodes.Stelem_Ref);
		}
	}
}
