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


namespace Castle.Components.Binder
{
	using System;

	internal class PrimitiveConverter:TypeConverterBase
	{
		public override object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = ConverterUtil.NormalizeInput(input);

			if (IsBool(desiredType))
			{
				return SpecialBoolConversion(value, input, ref conversionSucceeded);
			}
			else if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}
			else if (value == String.Empty)
			{
				conversionSucceeded = true;
				return null;
			}
			else
			{
				return System.Convert.ChangeType(input, desiredType);
			}
		}
		private static bool IsBool(Type desiredType)
		{
			bool isBool = desiredType == typeof(Boolean);

			if (!isBool)
			{
				isBool = desiredType == typeof(bool?);
			}

			return isBool;
		}
		private static object SpecialBoolConversion(string value, object input, ref bool conversionSucceeded)
		{
			if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}
			else if (value == String.Empty)
			{
				conversionSucceeded = false;
				return false;
			}
			else
			{
				if (value.IndexOf(',') != -1)
				{
					value = value.Substring(0, value.IndexOf(','));
				}

				bool performNumericConversion = false;

				foreach (char c in value.ToCharArray())
				{
					if (Char.IsNumber(c))
					{
						performNumericConversion = true;
						break;
					}
				}

				if (performNumericConversion)
				{
					return System.Convert.ToBoolean(System.Convert.ToInt32(value));
				}
				else
				{
					return !(String.Compare("false", value, true) == 0);
				}
			}
		}

		public override bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			return IsTypeConvertible(desiredType, inputType, input, out exactMatch) && desiredType.IsPrimitive;
		}
	}
}
