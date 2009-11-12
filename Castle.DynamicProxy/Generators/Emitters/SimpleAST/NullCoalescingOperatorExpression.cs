// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	public class NullCoalescingOperatorExpression:Expression
	{
		private readonly Reference reference;
		private readonly Reference @default;

		public NullCoalescingOperatorExpression(Reference reference, Reference @default)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}

			if (@default == null)
			{
				throw new ArgumentNullException("default");
			}

			this.reference = reference;
			this.@default = @default;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			reference.LoadReference(gen);
			gen.Emit(OpCodes.Dup);
			var label = gen.DefineLabel();
			gen.Emit(OpCodes.Brtrue_S, label);
			gen.Emit(OpCodes.Pop);
			@default.LoadReference(gen);
			gen.MarkLabel(label);
		}
	}
}