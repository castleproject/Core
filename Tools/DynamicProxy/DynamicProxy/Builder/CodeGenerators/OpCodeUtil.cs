// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for OpCodeUtil.
	/// </summary>
	abstract class OpCodeUtil
	{
		/// <summary>
		/// Converts a Value type to a correspondent OpCode
		/// of value type allocation
		/// </summary>
		/// <param name="gen"></param>
		public static void ConvertTypeToOpCode(ILGenerator gen, Type type)
		{
			if (type.IsEnum)
			{
				Enum baseType = (Enum) Activator.CreateInstance(type);
				TypeCode code = baseType.GetTypeCode();

				switch (code)
				{
					case TypeCode.Byte:
						type = typeof (Byte);
						break;
					case TypeCode.Int16:
						type = typeof (Int16);
						break;
					case TypeCode.Int32:
						type = typeof (Int32);
						break;
					case TypeCode.Int64:
						type = typeof (Int64);
						break;
				}

				ConvertTypeToOpCode(gen, type);
				return;
			}

			if (type.IsPrimitive)
			{
				OpCode opCode = OpCodesDictionary.Instance[type];

				if (Object.ReferenceEquals(opCode, OpCodesDictionary.EmptyOpCode))
				{
					throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
				}

				gen.Emit(opCode);
			}
			else
			{
				gen.Emit(OpCodes.Ldobj, type);
			}
		}
	}
}
