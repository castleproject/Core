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
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	///   Represents the storage location <c>X</c> referenced by a <see cref="Reference"/>
	///   holding an unmanaged pointer <c>&amp;X</c> to it.
	///   It essentially has the same function as the pointer indirection / dereferencing operator <c>*</c>.
	/// </summary>
	[DebuggerDisplay("*{ptr}")]
	internal class PointerReference : Reference
	{
		private readonly IExpression ptr;

		public PointerReference(IExpression ptr, Type elementType)
			: base(elementType)
		{
			this.ptr = ptr;
		}

		public override void EmitAddress(ILGenerator gen)
		{
			ptr.Emit(gen);
		}

		public override void Emit(ILGenerator gen)
		{
			ptr.Emit(gen);
			OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, Type);
		}

		public override void EmitStore(IExpression value, ILGenerator gen)
		{
			ptr.Emit(gen);
			value.Emit(gen);
			OpCodeUtil.EmitStoreIndirectOpCodeForType(gen, Type);
		}
	}
}