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

	public class DefaultValueExpression : Expression
	{
		private readonly Type type;

		public DefaultValueExpression(Type type)
		{
			this.type = type;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			// TODO: check if this can be simplified by using more of OpCodeUtil and other existing types
			if (IsPrimitiveOrClass(type))
			{
				OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, type);
			}
			else if (type.IsValueType || type.IsGenericParameter)
			{
				// TODO: handle decimal explicitly
				var local = gen.DeclareLocal(type);
				gen.Emit(OpCodes.Ldloca_S, local);
				gen.Emit(OpCodes.Initobj, type);
				gen.Emit(OpCodes.Ldloc, local);
			}
			else if (type.IsByRef)
			{
				EmitByRef(gen);
			}
			else
			{
				throw new ProxyGenerationException("Can't emit default value for type " + type);
			}
		}

		private void EmitByRef(ILGenerator gen)
		{
			var elementType = type.GetElementType();
			if (IsPrimitiveOrClass(elementType))
			{
				OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, elementType);
				OpCodeUtil.EmitStoreIndirectOpCodeForType(gen, elementType);
			}
			else if (elementType.IsGenericParameter || elementType.IsValueType)
			{
				gen.Emit(OpCodes.Initobj, elementType);
			}
			else
			{
				throw new ProxyGenerationException("Can't emit default value for reference of type " + elementType);
			}
		}

		private bool IsPrimitiveOrClass(Type type)
		{
			if ((type.IsPrimitive && type != typeof(IntPtr)))
			{
				return true;
			}
			return ((type.IsClass || type.IsInterface) &&
			        type.IsGenericParameter == false &&
			        type.IsByRef == false);
		}
	}
}