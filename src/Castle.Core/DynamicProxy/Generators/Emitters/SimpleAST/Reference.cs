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
	using System.Reflection.Emit;

	internal abstract class Reference : IExpression
	{
		protected Reference? owner;

		protected Reference(Reference? owner)
		{
			this.owner = owner;
		}

		public Reference? OwnerReference
		{
			get { return owner; }
			set { owner = value; }
		}

		public abstract void LoadAddressOfReference(ILGenerator gen);

		public abstract void LoadReference(ILGenerator gen);

		public abstract void StoreReference(IExpression value, ILGenerator gen);

		public virtual void Generate(ILGenerator gen)
		{
		}

		public void Emit(ILGenerator gen)
		{
			OwnerReference?.Emit(gen);
			LoadReference(gen);
		}
	}
}