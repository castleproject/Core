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

	public class LiteralIntExpression : Expression
	{
		private readonly int value;

		public LiteralIntExpression(int value)
		{
			this.value = value;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			switch (value)
			{
				case -1:
					gen.Emit(OpCodes.Ldc_I4_M1);
					break;
				case 0:
					gen.Emit(OpCodes.Ldc_I4_0);
					break;
				case 1:
					gen.Emit(OpCodes.Ldc_I4_1);
					break;
				case 2:
					gen.Emit(OpCodes.Ldc_I4_2);
					break;
				case 3:
					gen.Emit(OpCodes.Ldc_I4_3);
					break;
				case 4:
					gen.Emit(OpCodes.Ldc_I4_4);
					break;
				case 5:
					gen.Emit(OpCodes.Ldc_I4_5);
					break;
				case 6:
					gen.Emit(OpCodes.Ldc_I4_6);
					break;
				case 7:
					gen.Emit(OpCodes.Ldc_I4_7);
					break;
				case 8:
					gen.Emit(OpCodes.Ldc_I4_8);
					break;
				default:
					gen.Emit(OpCodes.Ldc_I4, value);
					break;
			}
		}
	}
}