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

namespace Castle.Components.Binder
{
	using System;
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
					if ((input == null) || (((string) input).Length == 0))
					{
						conversionSucceeded = false;
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
			
			if (desiredType == inputType) // Not conversion required
			{
				conversionSucceeded = true;
				return input;
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

				if (desiredType == typeof(String))
				{
					if (conversionSucceeded && ((String) input) == String.Empty)
					{
						return null;
					}
					
					return conversionSucceeded ? input.ToString().Trim(' ') : null;
				}
//				else if (desiredType == typeof(int))
//				{
//					return ConvertInt32(input, ref conversionSucceeded);
//				}
				else if (desiredType.IsArray)
				{
					return conversionSucceeded ? 
						ConvertToArray(desiredType, input, ref conversionSucceeded) : null;
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
				
				ThrowInformativeException(desiredType, input, inner); return null;
			}
		}

		private object ConvertGuid(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeInput(input);
	
			if (value == String.Empty)
			{
				conversionSucceeded = false;
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

			String value = NormalizeInput(input);
	
			if (value == String.Empty)
			{
				conversionSucceeded = false;
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
			
			if (desiredType == typeof(Boolean))
			{
				if (input == null)
				{
					conversionSucceeded = false;
					return null;
				}
				else if (value == String.Empty)
				{
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
			else if (input == null || value == String.Empty)
			{
				conversionSucceeded = false;
						
				return null;
			}
			else
			{
				return System.Convert.ChangeType(input, desiredType);
			}
		}

		private object ConvertEnum(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

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

		private object ConvertDate(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

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
		
		private object ConvertToArray(Type desiredType, object input, ref bool conversionSucceeded)
		{
			Type elemType = desiredType.GetElementType();

			// Fix for mod_mono issue where array values are passed 
			// as a comma seperated String
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
					throw new BindingException("Cannot convert to array type {0} from type {1}", elemType.FullName, input.GetType().FullName);
				}
			}

			Array values = input as Array;
			Array result = Array.CreateInstance(elemType, values.Length);
			bool elementConversionSucceeded;

			for(int i = 0; i < values.Length; i++)
			{
				result.SetValue(Convert(elemType, values.GetValue(i), out elementConversionSucceeded), i);
				
				// if at least one array element get converted 
				// we consider the conversion a success
				if (elementConversionSucceeded && !conversionSucceeded) conversionSucceeded = true;
			}

			return result;
		}
		
		/// <summary>
		/// Support for types that specify a TypeConverter, 
		/// i.e.: NullableTypes
		/// </summary>
		private object ConvertUsingTypeConverter(Type desiredType, object input, ref bool conversionSucceeded)
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
							stArray[i] = itemVal.ToString();
						else
							stArray[i] = String.Empty;
					}
					
					return String.Join(",", stArray);
				}
			}
		}

		private static void ThrowInformativeException(Type desiredType, object input, Exception inner)
		{
			String message = String.Format("Conversion error: " + 
				"Could not convert parameter with value '{0}' to expected type {1}", input, desiredType);
	
			throw new BindingException(message, inner);
		}
	}
}
