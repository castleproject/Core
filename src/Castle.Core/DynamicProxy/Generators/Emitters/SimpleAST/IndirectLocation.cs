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
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	///   Wraps a reference that is passed 
	///   ByRef and provides indirect load/store support.
	/// </summary>
	[DebuggerDisplay("*{owner}")]
	internal class IndirectLocation : Location
	{
		private readonly Location owner;

		public IndirectLocation(Location byRefReference) :
			base(byRefReference.Type.GetElementType())
		{
			if (!byRefReference.Type.IsByRef)
			{
				throw new ArgumentException("Expected an IsByRef reference", nameof(byRefReference));
			}

			owner = byRefReference;
		}

		public override void EmitLoadAddress(ILGenerator gen)
		{
			// Load of owner reference takes care of this.
		}

		// TODO: Better name

		public override void EmitLoad(ILGenerator gen)
		{
			owner.Emit(gen);
			OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, Type);
		}

		public override void EmitStore(IExpression expression, ILGenerator gen)
		{
			owner.Emit(gen);
			expression.Emit(gen);
			OpCodeUtil.EmitStoreIndirectOpCodeForType(gen, Type);
		}

		public static Location WrapIfByRef(Location reference)
		{
			return reference.Type.IsByRef ? new IndirectLocation(reference) : reference;
		}

		// TODO: Better name
		public static Location[] WrapIfByRef(Location[] references)
		{
			var result = new Location[references.Length];

			for (var i = 0; i < references.Length; i++)
			{
				result[i] = WrapIfByRef(references[i]);
			}

			return result;
		}
	}
}