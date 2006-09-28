// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	[CLSCompliant(false)]
	public class ConvertExpression : Expression
	{
		private readonly Type target;
		private readonly Type fromType;
		private readonly Expression right;

		public ConvertExpression(Type targetType, Expression right)
			: this(targetType, typeof(object), right)
		{
		}

		public ConvertExpression(Type targetType, Type fromType, Expression right)
		{
			this.target = targetType;
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
				throw new NotSupportedException("Cannot convert from ByRef types");
			}

			if (target.IsByRef)
			{
				throw new NotSupportedException("Cannot convert to ByRef types");
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
					gen.Emit(OpCodes.Unbox, target);
					OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, target);
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
