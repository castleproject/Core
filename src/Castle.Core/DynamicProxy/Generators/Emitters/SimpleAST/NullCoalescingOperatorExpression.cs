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
	using System.Reflection.Emit;

	internal class NullCoalescingOperatorExpression : IExpression
	{
		private readonly IExpression @default;
		private readonly IExpression expression;

		public NullCoalescingOperatorExpression(IExpression expression, IExpression @default)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			if (@default == null)
			{
				throw new ArgumentNullException(nameof(@default));
			}

			this.expression = expression;
			this.@default = @default;
		}

		public void Emit(ILGenerator gen)
		{
			expression.Emit(gen);
			gen.Emit(OpCodes.Dup);
			var label = gen.DefineLabel();
			gen.Emit(OpCodes.Brtrue_S, label);
			gen.Emit(OpCodes.Pop);
			@default.Emit(gen);
			gen.MarkLabel(label);
		}
	}
}