// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Web;

	public class DefaultConverter : MarshalByRefObject, IConverter
	{
		public bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			exactMatch = false;

			if (input == null)
			{
				return true;
			}
			else if (inputType == desiredType)
			{
				exactMatch = true;
			}
			else if (!desiredType.IsInstanceOfType(input))
			{
				TypeConverter conv = TypeDescriptor.GetConverter(desiredType);
				return (conv != null && conv.CanConvertFrom(inputType));
			}

			return false;
		}

		public object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			if (inputType == desiredType) // Nothing to convert
			{
				if (inputType == typeof(String))
				{
					if (input == null)
					{
						conversionSucceeded = false;
						return null;
					}
					else if (input.ToString().Length == 0)
					{
						conversionSucceeded = true;
						return null;
					}
					else
					{
						conversionSucceeded = true;
						return input.ToString().Trim(' ');
					}
				}
				else
				{
					conversionSucceeded = true;
					return input;
				}
			}

			return Convert(desiredType, input, out conversionSucceeded);
		}

		/// <summary>
		/// Convert the input param into the desired type
		/// </summary>
		/// <param name="desiredType">Type of the desired</param>
		/// <param name="input">The input</param>
		/// <param name="conversionSucceeded">if <c>false</c> the return value must be ignored</param>
		/// <remarks>
		/// There are 3 possible cases when trying to convert:
		/// 1) Input data for conversion missing (input is null or an empty String)
		///		Returns default conversion value (based on desired type) and set <c>conversionSucceeded = false</c>
		/// 2) Has input data but cannot convert to particular type
		///		Throw exception and set <c>conversionSucceeded = false</c>
		/// 3) Has input data and can convert to particular type
		/// 	 Return input converted to desired type and set <c>conversionSucceeded = true</c>
		/// </remarks>
		public object Convert(Type desiredType, object input, out bool conversionSucceeded)
		{
			try
			{
				conversionSucceeded = (input != null);

				if (desiredType.IsInstanceOfType(input))
				{
					return input;
				}
				else if (desiredType == typeof(String))
				{
					if (conversionSucceeded && input.GetType() == typeof(string) && ((String) input) == String.Empty)
					{
						return null;
					}

					return conversionSucceeded ? input.ToString().Trim(' ') : null;
				}
				else if (desiredType.IsArray)
				{
					return conversionSucceeded
					       	?
					       		ConvertToArray(desiredType, input, ref conversionSucceeded)
					       	: null;
				}
				else if (desiredType.IsEnum)
				{
					return ConvertEnum(desiredType, input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(Decimal))
				{
					return ConvertDecimal(input, ref conversionSucceeded);
				}
				else if (desiredType.IsPrimitive)
				{
					return ConvertPrimitive(desiredType, input, ref conversionSucceeded);
				}
				else if (desiredType.IsGenericType &&
				         desiredType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return ConvertNullable(desiredType, input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(Guid))
				{
					return ConvertGuid(input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(DateTime))
				{
					return ConvertDate(input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(HttpPostedFile))
				{
					return input;
				}
				else if (DataBinder.IsGenericList(desiredType))
				{
					return conversionSucceeded
					       	? ConvertGenericList(desiredType, input, ref conversionSucceeded)
					       	: null;
				}
				else
				{
					return ConvertUsingTypeConverter(desiredType, input, ref conversionSucceeded);
				}
			}
			catch(BindingException)
			{
				throw;
			}
			catch(Exception inner)
			{
				conversionSucceeded = false;

				ThrowInformativeException(desiredType, input, inner);
				return null;
			}
		}

		private object ConvertNullable(Type desiredType, object input, ref bool conversionSucceeded)
		{
			Type underlyingType = Nullable.GetUnderlyingType(desiredType);

			object value = Convert(underlyingType, input, out conversionSucceeded);

			if (conversionSucceeded && value != null)
			{
				Type typeToConstruct = typeof(Nullable<>).MakeGenericType(underlyingType);

				return Activator.CreateInstance(typeToConstruct, value);
			}
			else if (input != null)
			{
				conversionSucceeded = true;
			}
			else
			{
				conversionSucceeded = false;
			}

			return null;
		}

		private object ConvertGenericList(Type desiredType, object input, ref bool conversionSucceeded)
		{
			Type elemType = desiredType.GetGenericArguments()[0];

			input = FixInputForMonoIfNeeded(elemType, input);

			Type listType = typeof(List<>).MakeGenericType(elemType);
			IList result = (IList) Activator.CreateInstance(listType);
			Array values = input as Array;

			bool elementConversionSucceeded;

			for(int i = 0; i < values.Length; i++)
			{
				object val = Convert(elemType, values.GetValue(i), out elementConversionSucceeded);
				if(val!=null)
				{
					result.Add(val);
				}

				// if at least one list element get converted 
				// we consider the conversion a success
				if (elementConversionSucceeded && !conversionSucceeded)
				{
					conversionSucceeded = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Fix for mod_mono issue where array values are passed as a comma seperated String.
		/// </summary>
		/// <param name="elemType"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		private static object FixInputForMonoIfNeeded(Type elemType, object input)
		{
			if (!input.GetType().IsArray)
			{
				if (input.GetType() == typeof(String))
				{
					input = NormalizeInput(input);

					if (input.ToString() == String.Empty)
					{
						input = Array.CreateInstance(elemType, 0);
					}
					else
					{
						input = input.ToString().Split(',');
					}
				}
				else
				{
					throw new BindingException("Cannot convert to collection of {0} from type {1}", elemType.FullName,
					                           input.GetType().FullName);
				}
			}

			return input;
		}

		private object ConvertGuid(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}

			String value = NormalizeInput(input);

			if (value == String.Empty)
			{
				conversionSucceeded = true;
				return null;
			}
			else
			{
				return new Guid(value);
			}
		}

		private object ConvertDecimal(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}

			String value = NormalizeInput(input);

			if (value == String.Empty)
			{
				conversionSucceeded = true;
				return null;
			}
			else
			{
				return System.Convert.ToDecimal(value);
			}
		}

		private object ConvertPrimitive(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeInput(input);

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

				foreach(char c in value.ToCharArray())
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

		private object ConvertToArray(Type desiredType, object input, ref bool conversionSucceeded)
		{
			Type elemType = desiredType.GetElementType();

			input = FixInputForMonoIfNeeded(elemType, input);

			Array values = input as Array;
			Array result = Array.CreateInstance(elemType, values.Length);

			for(int i = 0; i < values.Length; i++)
			{
				bool elementConversionSucceeded;

				result.SetValue(Convert(elemType, values.GetValue(i), out elementConversionSucceeded), i);

				// if at least one array element get converted 
				// we consider the conversion a success
				if (elementConversionSucceeded && !conversionSucceeded)
				{
					conversionSucceeded = true;
				}
			}

			return result;
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

		private static object ConvertEnum(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}

			String value = NormalizeInput(input);

			if (value == String.Empty)
			{
				conversionSucceeded = false;
				return null;
			}
			else
			{
				return Enum.Parse(desiredType, value, true);
			}
		}

		private static object ConvertDate(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (input == null)
			{
				conversionSucceeded = false;
				return null;
			}

			String value = NormalizeInput(input);

			if (value == String.Empty)
			{
				conversionSucceeded = false;

				return null;
			}
			else
			{
				return DateTime.Parse(value);
			}
		}

		/// <summary>
		/// Support for types that specify a TypeConverter, 
		/// i.e.: NullableTypes
		/// </summary>
		private static object ConvertUsingTypeConverter(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			Type sourceType = (input != null ? input.GetType() : typeof(String));
			TypeConverter conv = TypeDescriptor.GetConverter(desiredType);

			if (conv != null && conv.CanConvertFrom(sourceType))
			{
				try
				{
					return conv.ConvertFrom(input);
				}
				catch(Exception ex)
				{
					String message = String.Format("Conversion error: " +
					                               "Could not convert parameter with value '{0}' to {1}", input, desiredType);

					throw new BindingException(message, ex);
				}
			}
			else
			{
				String message = String.Format("Conversion error: " +
				                               "Could not convert parameter with value '{0}' to {1}", input, desiredType);

				throw new BindingException(message);
			}
		}

		private static String NormalizeInput(object input)
		{
			if (input == null)
			{
				return String.Empty;
			}
			else
			{
				if (!input.GetType().IsArray)
				{
					return input.ToString().Trim();
				}
				else
				{
					Array array = (Array) input;

					String[] stArray = new string[array.GetLength(0)];

					for(int i = 0; i < stArray.Length; i++)
					{
						object itemVal = array.GetValue(i);

						if (itemVal != null)
						{
							stArray[i] = itemVal.ToString();
						}
						else
						{
							stArray[i] = String.Empty;
						}
					}

					return String.Join(",", stArray);
				}
			}
		}

		private static void ThrowInformativeException(Type desiredType, object input, Exception inner)
		{
			String message = String.Format("Conversion error: " +
			                               "Could not convert parameter with value '{0}' to expected type {1}", input,
			                               desiredType);

			throw new BindingException(message, inner);
		}
	}
}
