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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection.Emit;

	internal abstract class OpCodeUtil
	{
		/// <summary>
		///   Emits a load indirect opcode of the appropriate type for a value or object reference.
		///   Pops a pointer off the evaluation stack, dereferences it and loads
		///   a value of the specified type.
		/// </summary>
		/// <param name = "gen"></param>
		/// <param name = "type"></param>
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
			else if (type.IsPrimitive && type != typeof(IntPtr))
			{
				var opCode = LdindOpCodesDictionary.Instance[type];

				if (opCode == LdindOpCodesDictionary.EmptyOpCode)
				{
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
				}

				gen.Emit(opCode);
			}
			else if (type.IsValueType)
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
			else if (type.IsGenericParameter)
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				gen.Emit(OpCodes.Ldind_Ref);
			}
		}

		/// <summary>
		///   Emits a load opcode of the appropriate kind for a constant string or
		///   primitive value.
		/// </summary>
		/// <param name = "gen"></param>
		/// <param name = "value"></param>
		public static void EmitLoadOpCodeForConstantValue(ILGenerator gen, object value)
		{
			if (value is String)
			{
				gen.Emit(OpCodes.Ldstr, value.ToString());
			}
			else if (value is Int32)
			{
				var code = LdcOpCodesDictionary.Instance[value.GetType()];
				gen.Emit(code, (int)value);
			}
			else if (value is bool)
			{
				var code = LdcOpCodesDictionary.Instance[value.GetType()];
				gen.Emit(code, Convert.ToInt32(value));
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		///   Emits a load opcode of the appropriate kind for the constant default value of a
		///   type, such as 0 for value types and null for reference types.
		/// </summary>
		public static void EmitLoadOpCodeForDefaultValueOfType(ILGenerator gen, Type type)
		{
			if (type.IsPrimitive)
			{
				var opCode = LdcOpCodesDictionary.Instance[type];
				switch (opCode.StackBehaviourPush)
				{
					case StackBehaviour.Pushi:
						gen.Emit(opCode, 0);
						if (Is64BitTypeLoadedAsInt32(type))
						{
							// we load Int32, and have to convert it to 64bit type
							gen.Emit(OpCodes.Conv_I8);
						}
						break;
					case StackBehaviour.Pushr8:
						gen.Emit(opCode, 0D);
						break;
					case StackBehaviour.Pushi8:
						gen.Emit(opCode, 0L);
						break;
					case StackBehaviour.Pushr4:
						gen.Emit(opCode, 0F);
						break;
					default:
						throw new NotSupportedException();
				}
			}
			else
			{
				gen.Emit(OpCodes.Ldnull);
			}
		}

		/// <summary>
		///   Emits a store indirectopcode of the appropriate type for a value or object reference.
		///   Pops a value of the specified type and a pointer off the evaluation stack, and
		///   stores the value.
		/// </summary>
		/// <param name = "gen"></param>
		/// <param name = "type"></param>
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
			else if (type.IsPrimitive && type != typeof(IntPtr))
			{
				var opCode = StindOpCodesDictionary.Instance[type];

				if (Equals(opCode, StindOpCodesDictionary.EmptyOpCode))
				{
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
				}

				gen.Emit(opCode);
			}
			else if (type.IsValueType)
			{
				gen.Emit(OpCodes.Stobj, type);
			}
			else if (type.IsGenericParameter)
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
			var baseType = (Enum)Activator.CreateInstance(enumType);
			var code = baseType.GetTypeCode();

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
				case TypeCode.UInt16:
					return typeof(UInt16);
				case TypeCode.UInt32:
					return typeof(UInt32);
				case TypeCode.UInt64:
					return typeof(UInt64);
				default:
					throw new NotSupportedException();
			}
		}

		private static bool Is64BitTypeLoadedAsInt32(Type type)
		{
			return type == typeof(long) || type == typeof(ulong);
		}
	}
}