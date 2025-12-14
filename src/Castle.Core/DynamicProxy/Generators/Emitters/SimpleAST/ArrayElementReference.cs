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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System.Diagnostics;
	using System.Reflection.Emit;

	internal sealed class ArrayElementReference : Reference
	{
		private readonly Reference array;
		private readonly int index;

		public ArrayElementReference(Reference array, int index)
			: base(array.Type.GetElementType())
		{
			Debug.Assert(array.Type.IsArray);
			Debug.Assert(array.Type.GetElementType().IsClass || array.Type.GetElementType().IsInterface);

			this.array = array;
			this.index = index;
		}

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			array.Emit(gen);
			LiteralIntExpression.Emit(index, gen);
			gen.Emit(OpCodes.Ldelema);
		}

		public override void LoadReference(ILGenerator gen)
		{
			array.Emit(gen);
			LiteralIntExpression.Emit(index, gen);
			gen.Emit(OpCodes.Ldelem_Ref);

			// NOTE: The opcode isedabove is only valid for reference types.
			// Here's the opcode mapping for all array element types:
			//
			// | element type                                 | opcode
			// |----------------------------------------------|-------------------
			// | signed primitive types and enums             | ldelem.i{1,2,4,8}
			// | unsigned primitive types (including `bool`)  | ldelem.u{1,2,4}
			// | `nint` and `nuint`                           | ldelem.i
			// | floating point primitive types               | ldelem.r{4,8}
			// | generic type param/args and structs          | ldelem
			// | classes and interfaces                       | ldelem.ref
		}

		public override void StoreReference(IExpression expression, ILGenerator gen)
		{
			array.Emit(gen);
			LiteralIntExpression.Emit(index, gen);
			expression.Emit(gen);
			gen.Emit(OpCodes.Stelem_Ref);

			// NOTE: The opcode used above is only valid for reference types.
			// Here's the opcode mapping for all array element types:
			//
			// | element type                                 | opcode
			// |----------------------------------------------|-------------------
			// | signed primitive types and enums             | stelem.i{1,2,4,8}
			// | unsigned primitive types (including `bool`)  | stelem.u{1,2,4}
			// | `nint` and `nuint`                           | stelem.i
			// | floating point primitive types               | stelem.r{4,8}
			// | generic type param/args and structs          | stelem
			// | classes and interfaces                       | stelem.ref
		}
	}
}
