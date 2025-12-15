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

#nullable enable

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System;
	using System.Reflection.Emit;

	// Locations as defined by ECMA-335 section I.8.3.
	// They are typed and can each hold one value of that type.
	// Managed pointers (&, or "by-refs") may refer to them.
	// For that purpose, their address can be taken.
	//
	// There are four principal types of locations:
	//
	//  1. method arguments (see `ArgumentLocation`)
	//  2. static and instance fields (see `FieldLocation`)
	//  3. local variables (see `LocalLocation`)
	//  4. array elements (see `ArrayElementLocation`)
	//
	// One additional helper location type, `IndirectLocation`,
	// acts as a stand-in for any and all of the above
	// to facilitate code generation involving by-refs.
	internal abstract class Location : IExpression
	{
		protected readonly Type type;

		protected Location(Type type)
		{
			this.type = type;
		}

		public Type Type
		{
			get { return type; }
		}

		public abstract void EmitLoadAddress(ILGenerator gen);

		public abstract void EmitLoad(ILGenerator gen);

		public abstract void EmitStore(IExpression expression, ILGenerator gen);

		public virtual void Generate(ILGenerator gen)
		{
		}

		public void Emit(ILGenerator gen)
		{
			EmitLoad(gen);
		}
	}
}