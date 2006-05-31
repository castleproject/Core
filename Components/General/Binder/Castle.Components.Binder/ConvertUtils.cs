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
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Web;

	/// <summary>
	/// Handles only simple conversion of well-known types (primitives, DateTime)
	/// as well as type that have a <see cref="TypeConverter"/> associated
	/// </summary>
	public sealed class ConvertUtils
	{
		private ConvertUtils()
		{
		}

		public static object Convert(Type desiredType, String paramName, 
			NameValueCollection paramList, IDictionary files)
		{
			bool conversionSucceeded;

			return Convert(desiredType, paramName, paramList, files, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, String paramName, 
			IDictionary paramList, IDictionary files)
		{
			bool conversionSucceeded;

			return Convert(desiredType, paramName, paramList, files, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, String paramName, 
			NameValueCollection paramList, IDictionary files, out bool conversionSucceeded)
		{
			object input = GetInput(desiredType, paramName, paramList, files);

			return Convert(desiredType, paramName, input, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, String paramName, 
			IDictionary paramList, IDictionary files, out bool conversionSucceeded)
		{
			object input = GetInput(desiredType, paramName, paramList, files);

			return Convert(desiredType, paramName, input, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, object value)
		{
			bool conversionSucceeded;

			return Convert(desiredType, "unspecified param", value, out conversionSucceeded);
		}

		/// <summary>
		/// Convert the input param into the desired type
		/// </summary>
		/// <param name="desiredType">Type of the desired</param>
		/// <param name="input">The input</param>
		/// <param name="conversionSucceeded">if <c>false</c> the return value must be ignored</param>
        /// <param name="paramName">Parameter name</param>
		/// <remarks>
		/// There are 3 possible cases when trying to convert:
		/// 1) Input data for conversion missing (input is null or an empty String)
		///		Returns default conversion value (based on desired type) and set <c>conversionSucceeded = false</c>
		/// 2) Has input data but cannot convert to particular type
		///		Throw exception and set <c>conversionSucceeded = false</c>
		/// 3) Has input data and can convert to particular type
		/// 	 Return input converted to desired type and set <c>conversionSucceeded = true</c>
		/// </remarks>
		public static object Convert(Type desiredType, String paramName, object input, out bool conversionSucceeded)
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
				else if (desiredType == typeof(int))
				{
					return ConvertInt32(input, ref conversionSucceeded);
				}
				else if (desiredType.IsArray)
				{
					return conversionSucceeded ? 
						ConvertToArray(desiredType, paramName, input, ref conversionSucceeded) : null;
				}
				else if (desiredType.IsEnum)
				{
					return ConvertEnum(desiredType, input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(Decimal))
				{
					return ConvertDecimal(input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(Guid))
				{
					return ConvertGuid(input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(DateTime))
				{
					return ConvertDate(paramName, input, ref conversionSucceeded);
				}
				else if (desiredType == typeof(HttpPostedFile))
				{
					return input;
				}
				else if (desiredType.IsPrimitive)
				{
					return ConvertPrimitive(desiredType, input, ref conversionSucceeded);
				}
				else
				{
					return ConvertUsingTypeConverter(desiredType, paramName, input, ref conversionSucceeded);
				}
			}
			catch(BindingException)
			{
				conversionSucceeded = false;

				throw;
			}
			catch(Exception inner)
			{
				conversionSucceeded = false;
				
				ThrowInformativeException(desiredType, paramName, input, inner); return null;
			}
		}

		private static object ConvertInt32(object input, ref bool conversionSucceeded)
		{
			if (conversionSucceeded && ((String) input) != String.Empty)
			{
				return System.Convert.ToInt32(input);
			}

			conversionSucceeded = false;

			return null;
		}

		private static object ConvertGuid(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeString(input as String);
	
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

		private static object ConvertDecimal(object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeString(input as String);
	
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

		private static object ConvertPrimitive(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeString(input as String);
	
			if (desiredType == typeof(Boolean))
			{
				if (value == String.Empty)
				{
					return false;
				}
				else
				{
					return !(String.Compare("false", value, true) == 0);
				}
			}
			else if (value == String.Empty)
			{
				conversionSucceeded = false;
						
				return null;
			}
			else
			{
				return System.Convert.ChangeType(value, desiredType);
			}
		}

		private static object ConvertEnum(Type desiredType, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			String value = NormalizeString(input as String);
	
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

		private static object ConvertDate(string paramName, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (input != null && input.GetType().IsArray)
			{
				Array inputArray = input as Array;

				if (inputArray.Length != 3 || input.GetType().GetElementType() != typeof(String))
				{
					String message = String.Format("Convert DateTime expects String array with size 3, input was {0} with size {1}",
					                               input.GetType().GetElementType().FullName, inputArray.Length);

					throw new BindingException(message);
				}

				int numYear = System.Convert.ToInt32(inputArray.GetValue(0));
				int numMonth = System.Convert.ToInt32(inputArray.GetValue(1));
				int numDay = System.Convert.ToInt32(inputArray.GetValue(2));

				try
				{
					return new DateTime(numYear, numMonth, numDay);
				}
				catch(Exception inner)
				{
					String message = String.Format(
						"Convert DateTime: invalid date for " + 
						"parameter {0} (day: {1} month: {2} year: {3})", paramName, numDay, numMonth, numYear);

					throw new BindingException(message, inner);
				}
			}
			else
			{
				String value = NormalizeString(input as String);

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
		}

		private static object ConvertToArray(Type desiredType, string paramName, object input, ref bool conversionSucceeded)
		{
			Type elemType = desiredType.GetElementType();

			// Fix for mod_mono issue where array values are passed 
			// as a comma seperated String
			if (!input.GetType().IsArray)
			{
				if (input.GetType() == typeof(String))
				{
					input = NormalizeString(input.ToString());

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
			bool elementConversionSucceeded = conversionSucceeded = false;

			for(int i = 0; i < values.Length; i++)
			{
				result.SetValue(Convert(elemType, paramName, values.GetValue(i), out elementConversionSucceeded), i);
				
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
		private static object ConvertUsingTypeConverter(Type desiredType, string paramName, object input, ref bool conversionSucceeded)
		{
			conversionSucceeded = true;

			Type sourceType = (input != null ? input.GetType() : typeof(String));
			TypeConverter conv = TypeDescriptor.GetConverter(desiredType);

			if (input != null && conv != null && conv.CanConvertFrom(sourceType))
			{
				return conv.ConvertFrom(input);
			}
			else
			{
				conversionSucceeded = false;

				String message = String.Format("Conversion error: " + 
					"Could not convert parameter '{0}' with value '{1}' to {2}", paramName, input, desiredType);

				throw new BindingException(message);
			}
		}

		private static String NormalizeString(String input)
		{
			return (input == null) ? String.Empty : input.Trim();
		}

		private static object GetInput(Type type, String paramName, NameValueCollection paramList, IDictionary fileList)
		{
			if (type == typeof(HttpPostedFile))
			{
				return fileList[paramName];
			}
			else
			{
				object input = paramList[paramName];

				// When type == DateTime we should also look 
				// up for paramNameyear, paramNamemonth, paramNameday
				if (input == null && type == typeof(DateTime) &&
					paramList[paramName + "day"] != null &&
					paramList[paramName + "month"] != null &&
					paramList[paramName + "year"] != null)
				{
					return new String[] {   paramList[paramName + "year"], 
											paramList[paramName + "month"], 
											paramList[paramName + "day"] };
				}

				return input;
			}
		}

		private static object GetInput(Type type, String paramName, IDictionary paramList, IDictionary fileList)
		{
			if (type == typeof(HttpPostedFile))
			{
				return fileList[paramName];
			}
			else
			{
				object input = paramList[paramName];

				// When type == DateTime we should also look 
				// up for paramNameyear, paramNamemonth, paramNameday
				if (input == null && type == typeof(DateTime) &&
					paramList[paramName + "day"] != null &&
					paramList[paramName + "month"] != null &&
					paramList[paramName + "year"] != null)
				{
					return new String[] {  
										(String) paramList[paramName + "year"], 
										(String) paramList[paramName + "month"], 
										(String) paramList[paramName + "day"] };
				}

				return input;
			}
		}

		private static void ThrowInformativeException(Type desiredType, string paramName, object input, Exception inner)
		{
			String message = String.Format("Conversion error: " + 
				"Could not convert parameter '{0}' with value '{1}' to {2}", paramName, input, desiredType);
	
			throw new BindingException(message, inner);
		}
	}
}