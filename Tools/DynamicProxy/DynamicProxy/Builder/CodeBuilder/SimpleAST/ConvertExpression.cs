using System.Reflection.Emit;
using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
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

	/// <summary>
	/// Summary description for ConvertExpression.
	/// </summary>
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

			if (_target.IsValueType && !_fromType.IsValueType)
			{
				gen.Emit(OpCodes.Unbox, _target);
				OpCodeUtil.ConvertTypeToOpCode(gen, _target);
				return;
			}
			else if (!_target.IsValueType && _fromType.IsValueType)
			{
				gen.Emit(OpCodes.Box, _fromType);
			}
			
			if (_target != typeof(Object))
			{
				gen.Emit(OpCodes.Castclass, _target);
			}
		}
	}
}
