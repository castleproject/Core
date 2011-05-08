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

	public class ConvertExpression : Expression
	{
		private readonly Expression right;
		private Type fromType;
		private Type target;

		public ConvertExpression(Type targetType, Expression right)
			: this(targetType, typeof(object), right)
		{
		}

		public ConvertExpression(Type targetType, Type fromType, Expression right)
		{
			target = targetType;
			this.fromType = fromType;
			this.right = right;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			right.Emit(member, gen);

			if (fromType == target)
			{
				return;
			}

			if (fromType.IsByRef)
			{
				fromType = fromType.GetElementType();
			}

			if (target.IsByRef)
			{
				target = target.GetElementType();
			}

			if (target.IsValueType)
			{
				if (fromType.IsValueType)
				{
					throw new NotImplementedException("Cannot convert between distinct value types");
				}
				else
				{
					// Unbox conversion
					// Assumes fromType is a boxed value
					// if we can, we emit a box and ldind, otherwise, we will use unbox.any
					if (LdindOpCodesDictionary.Instance[target] != LdindOpCodesDictionary.EmptyOpCode)
					{
						gen.Emit(OpCodes.Unbox, target);
						OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, target);
					}
					else
					{
						gen.Emit(OpCodes.Unbox_Any, target);
					}
				}
			}
			else
			{
				if (fromType.IsValueType)
				{
					// Box conversion
					gen.Emit(OpCodes.Box, fromType);
					EmitCastIfNeeded(typeof(object), target, gen);
				}
				else
				{
					// Possible down-cast
					EmitCastIfNeeded(fromType, target, gen);
				}
			}
		}

		private static void EmitCastIfNeeded(Type from, Type target, ILGenerator gen)
		{
			if (target.IsGenericParameter)
			{
				gen.Emit(OpCodes.Unbox_Any, target);
			}
			else if (from.IsGenericParameter)
			{
				gen.Emit(OpCodes.Box, from);
			}
			else if (target.IsGenericType && target != from)
			{
				gen.Emit(OpCodes.Castclass, target);
			}
			else if (target.IsSubclassOf(from))
			{
				gen.Emit(OpCodes.Castclass, target);
			}
		}
	}
}