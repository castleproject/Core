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
	using System.Reflection.Emit;

	internal sealed class LoadRefArrayElementExpression : IExpression
	{
		private readonly Reference arrayReference;
		private readonly LiteralIntExpression index;

		public LoadRefArrayElementExpression(int index, Reference arrayReference)
		{
			this.index = new LiteralIntExpression(index);
			this.arrayReference = arrayReference;
		}

		public void Emit(ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(arrayReference, gen);
			index.Emit(gen);
			gen.Emit(OpCodes.Ldelem_Ref);
		}
	}
}