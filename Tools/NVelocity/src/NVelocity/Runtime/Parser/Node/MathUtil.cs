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

namespace NVelocity.Runtime.Parser.Node
{
	using System;

	public class MathUtil
	{
		public static object Add(Type maxType, object left, object right)
		{
			if (maxType == typeof(Double))
			{
				return Convert.ToDouble(left) + Convert.ToDouble(right);
			}
			else if (maxType == typeof(Single))
			{
				return Convert.ToSingle(left) + Convert.ToSingle(right);
			}
			else if (maxType == typeof(Decimal))
			{
				return Convert.ToDecimal(left) + Convert.ToDecimal(right);
			}
			else if (maxType == typeof(Int64))
			{
				return Convert.ToInt64(left) + Convert.ToInt64(right);
			}
			else if (maxType == typeof(Int32))
			{
				return Convert.ToInt32(left) + Convert.ToInt32(right);
			}
			else if (maxType == typeof(Int16))
			{
				return Convert.ToInt16(left) + Convert.ToInt16(right);
			}
			else if (maxType == typeof(SByte))
			{
				return Convert.ToSByte(left) + Convert.ToSByte(right);
			}
			else if (maxType == typeof(Byte))
			{
				return Convert.ToByte(left) + Convert.ToByte(right);
			}

			return 0;
		}

		public static object Mult(Type maxType, object left, object right)
		{
			if (maxType == typeof(Double))
			{
				return Convert.ToDouble(left) * Convert.ToDouble(right);
			}
			else if (maxType == typeof(Single))
			{
				return Convert.ToSingle(left) * Convert.ToSingle(right);
			}
			else if (maxType == typeof(Decimal))
			{
				return Convert.ToDecimal(left) * Convert.ToDecimal(right);
			}
			else if (maxType == typeof(Int64))
			{
				return Convert.ToInt64(left) * Convert.ToInt64(right);
			}
			else if (maxType == typeof(Int32))
			{
				return Convert.ToInt32(left) * Convert.ToInt32(right);
			}
			else if (maxType == typeof(Int16))
			{
				return Convert.ToInt16(left) * Convert.ToInt16(right);
			}
			else if (maxType == typeof(SByte))
			{
				return Convert.ToSByte(left) * Convert.ToSByte(right);
			}
			else if (maxType == typeof(Byte))
			{
				return Convert.ToByte(left) * Convert.ToByte(right);
			}

			return 0;
		}

		public static object Div(Type maxType, object left, object right)
		{
			if (maxType == typeof(Double))
			{
				return Convert.ToDouble(left) / Convert.ToDouble(right);
			}
			else if (maxType == typeof(Single))
			{
				return Convert.ToSingle(left) / Convert.ToSingle(right);
			}
			else if (maxType == typeof(Decimal))
			{
				return Convert.ToDecimal(left) / Convert.ToDecimal(right);
			}
			else if (maxType == typeof(Int64))
			{
				return Convert.ToInt64(left) / Convert.ToInt64(right);
			}
			else if (maxType == typeof(Int32))
			{
				return Convert.ToInt32(left) / Convert.ToInt32(right);
			}
			else if (maxType == typeof(Int16))
			{
				return Convert.ToInt16(left) / Convert.ToInt16(right);
			}
			else if (maxType == typeof(SByte))
			{
				return Convert.ToSByte(left) / Convert.ToSByte(right);
			}
			else if (maxType == typeof(Byte))
			{
				return Convert.ToByte(left) / Convert.ToByte(right);
			}

			return 0;
		}

		public static object Mod(Type maxType, object left, object right)
		{
			if (maxType == typeof(Double))
			{
				return Convert.ToDouble(left) % Convert.ToDouble(right);
			}
			else if (maxType == typeof(Single))
			{
				return Convert.ToSingle(left) % Convert.ToSingle(right);
			}
			else if (maxType == typeof(Decimal))
			{
				return Convert.ToDecimal(left) % Convert.ToDecimal(right);
			}
			else if (maxType == typeof(Int64))
			{
				return Convert.ToInt64(left) % Convert.ToInt64(right);
			}
			else if (maxType == typeof(Int32))
			{
				return Convert.ToInt32(left) % Convert.ToInt32(right);
			}
			else if (maxType == typeof(Int16))
			{
				return Convert.ToInt16(left) % Convert.ToInt16(right);
			}
			else if (maxType == typeof(SByte))
			{
				return Convert.ToSByte(left) % Convert.ToSByte(right);
			}
			else if (maxType == typeof(Byte))
			{
				return Convert.ToByte(left) % Convert.ToByte(right);
			}

			return 0;
		}

		public static object Sub(Type maxType, object left, object right)
		{
			if (maxType == typeof(Double))
			{
				return Convert.ToDouble(left) - Convert.ToDouble(right);
			}
			else if (maxType == typeof(Single))
			{
				return Convert.ToSingle(left) - Convert.ToSingle(right);
			}
			else if (maxType == typeof(Decimal))
			{
				return Convert.ToDecimal(left) - Convert.ToDecimal(right);
			}
			else if (maxType == typeof(Int64))
			{
				return Convert.ToInt64(left) - Convert.ToInt64(right);
			}
			else if (maxType == typeof(Int32))
			{
				return Convert.ToInt32(left) - Convert.ToInt32(right);
			}
			else if (maxType == typeof(Int16))
			{
				return Convert.ToInt16(left) - Convert.ToInt16(right);
			}
			else if (maxType == typeof(SByte))
			{
				return Convert.ToSByte(left) - Convert.ToSByte(right);
			}
			else if (maxType == typeof(Byte))
			{
				return Convert.ToByte(left) - Convert.ToByte(right);
			}

			return 0;
		}

		public static Type ToMaxType(Type leftType, Type rightType)
		{
			if (leftType == rightType) return leftType;

			if (leftType == typeof(Double) || rightType == typeof(Double))
			{
				return typeof(Double);
			}
			else if (leftType == typeof(Single) || rightType == typeof(Single))
			{
				return typeof(Single);
			}
			else if (leftType == typeof(Decimal) || rightType == typeof(Decimal))
			{
				return typeof(Decimal);
			}
			else if (leftType == typeof(Int64) || rightType == typeof(Int64))
			{
				return typeof(Int64);
			}
			else if (leftType == typeof(Int32) || rightType == typeof(Int32))
			{
				return typeof(Int32);
			}
			else if (leftType == typeof(Int16) || rightType == typeof(Int16))
			{
				return typeof(Int16);
			}
			else if (leftType == typeof(SByte) || rightType == typeof(SByte))
			{
				return typeof(SByte);
			}
			else if (leftType == typeof(Byte) || rightType == typeof(Byte))
			{
				return typeof(Byte);
			}

			return null;
		}
	}
}
