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
	using System.Reflection.Emit;

	/// <summary>
	///   Summary description for NewArrayExpression.
	/// </summary>
	public class NewArrayExpression : Expression
	{
		private readonly Type arrayType;
		private readonly int size;

		public NewArrayExpression(int size, Type arrayType)
		{
			this.size = size;
			this.arrayType = arrayType;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldc_I4, size);
			gen.Emit(OpCodes.Newarr, arrayType);
		}
	}
}