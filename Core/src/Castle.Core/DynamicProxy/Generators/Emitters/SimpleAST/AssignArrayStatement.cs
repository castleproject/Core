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
	using System.Reflection.Emit;

	public class AssignArrayStatement : Statement
	{
		private readonly Reference targetArray;
		private readonly int targetPosition;
		private readonly Expression value;

		public AssignArrayStatement(Reference targetArray, int targetPosition, Expression value)
		{
			this.targetArray = targetArray;
			this.targetPosition = targetPosition;
			this.value = value;
		}

		public override void Emit(IMemberEmitter member, ILGenerator il)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(targetArray, il);

			il.Emit(OpCodes.Ldc_I4, targetPosition);

			value.Emit(member, il);

			il.Emit(OpCodes.Stelem_Ref);
		}
	}
}