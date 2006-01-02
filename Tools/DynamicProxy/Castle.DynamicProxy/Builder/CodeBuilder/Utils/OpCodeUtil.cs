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

namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
	using System;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for OpCodeUtil.
	/// </summary>
	abstract class OpCodeUtil
	{
		/// <summary>
		/// Emits a load opcode of the appropriate kind for a constant string or
		/// primitive value.
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="value"></param>
		public static void EmitLoadOpCodeForConstantValue(ILGenerator gen, object value)
		{
			if (value is String)
			{
				gen.Emit(OpCodes.Ldstr, value.ToString());
			}
			else if (value is Int32)
			{
				OpCode code = LdcOpCodesDictionary.Instance[ value.GetType() ];
				gen.Emit(code, (int) value);
			}
			else if (value is bool)
			{
				OpCode code = LdcOpCodesDictionary.Instance[ value.GetType() ];
				gen.Emit(code, Convert.ToInt32(value) );
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Emits a load opcode of the appropriate kind for the constant default value of a
		/// type, such as 0 for value types and null for reference types.
		/// </summary>
		public static void EmitLoadOpCodeForDefaultValueOfType(ILGenerator gen, Type type)
		{
			if (type.IsPrimitive)
			{
				gen.Emit(LdcOpCodesDictionary.Instance[type], 0);
			}
			else
			{
				gen.Emit(OpCodes.Ldnull);
			}
		}

		/// <summary>
		/// Emits a load indirect opcode of the appropriate type for a value or object reference.
		/// Pops a pointer off the evaluation stack, dereferences it and loads
		/// a value of the specified type.
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="type"></param>
		public static void EmitLoadIndirectOpCodeForType(ILGenerator gen, Type type)
		{
			if (type.IsEnum)
			{
				EmitLoadIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
				return;
			}

			if (type.IsByRef)
			{
				throw new NotSupportedException("Cannot load ByRef values");
			}
			else if (type.IsPrimitive)
			{
				OpCode opCode = LdindOpCodesDictionary.Instance[type];

				if (Object.ReferenceEquals(opCode, LdindOpCodesDictionary.EmptyOpCode))
				{
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
				}

				gen.Emit(opCode);
			}
			else if (type.IsValueType)
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				gen.Emit(OpCodes.Ldind_Ref);
			}
		}

		/// <summary>
		/// Emits a store indirectopcode of the appropriate type for a value or object reference.
		/// Pops a value of the specified type and a pointer off the evaluation stack, and
		/// stores the value.
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="type"></param>
		public static void EmitStoreIndirectOpCodeForType(ILGenerator gen, Type type)
		{
			if (type.IsEnum)
			{
				EmitStoreIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
				return;
			}

			if (type.IsByRef)
			{
				throw new NotSupportedException("Cannot store ByRef values");
			}
			else if (type.IsPrimitive)
			{
				OpCode opCode = StindOpCodesDictionary.Instance[type];

				if (Object.ReferenceEquals(opCode, StindOpCodesDictionary.EmptyOpCode))
				{
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
				}

				gen.Emit(opCode);
			}
			else if (type.IsValueType)
			{
				gen.Emit(OpCodes.Stobj, type);
			}
			else
			{
				gen.Emit(OpCodes.Stind_Ref);
			}
		}

		private static Type GetUnderlyingTypeOfEnum(Type enumType)
		{
			Enum baseType = (Enum)Activator.CreateInstance(enumType);
			TypeCode code = baseType.GetTypeCode();

			switch (code)
			{
				case TypeCode.SByte:
					return typeof(SByte);
				case TypeCode.Byte:
					return typeof(Byte);
				case TypeCode.Int16:
					return typeof(Int16);
				case TypeCode.Int32:
					return typeof(Int32);
				case TypeCode.Int64:
					return typeof(Int64);
				default:
					throw new NotSupportedException();
			}
		}
	}
}
