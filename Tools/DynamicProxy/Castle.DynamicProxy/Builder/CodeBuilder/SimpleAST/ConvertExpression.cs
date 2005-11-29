// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for ConvertExpression.
	/// </summary>
	[CLSCompliant(false)]
	public class ConvertExpression : Expression
	{
		private Type _target;
		private Type _fromType;
		private Expression _right;

		public ConvertExpression( Type targetType, Expression right ) : this(targetType, typeof(object), right)
		{
		}

		public ConvertExpression( Type targetType, Type fromType, Expression right )
		{
			_target = targetType;
			_fromType = fromType;
			_right = right;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			_right.Emit(member, gen);

			if (_fromType == _target)
			{
				return;
			}

			if (_fromType.IsByRef)
			{
				throw new NotSupportedException("Cannot convert from ByRef types");
			}

			if (_target.IsByRef)
			{
				throw new NotSupportedException("Cannot convert to ByRef types");
			}

			if (_target.IsValueType)
			{
				if (_fromType.IsValueType)
				{
					throw new NotImplementedException("Cannot convert between distinct value types at the moment");
				}
				else
				{
					// Unbox conversion
					// Assumes fromType is a boxed value
					gen.Emit(OpCodes.Unbox, _target);
					OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, _target);
				}
			}
			else
			{
				if (_fromType.IsValueType)
				{
					// Box conversion
					gen.Emit(OpCodes.Box, _fromType);
					EmitCastIfNeeded(typeof(object), _target, gen);
				}
				else
				{
					// Possible down-cast
					EmitCastIfNeeded(_fromType, _target, gen);
				}
			}
		}

		private void EmitCastIfNeeded(Type from, Type target, ILGenerator gen)
		{
			if (target.IsSubclassOf(from))
			{
				gen.Emit(OpCodes.Castclass, target);
			}
		}
	}
}
