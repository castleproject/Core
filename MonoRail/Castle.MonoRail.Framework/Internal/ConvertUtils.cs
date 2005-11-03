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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Text.RegularExpressions;
	using System.Web;

	public sealed class ConvertUtils
	{
		private ConvertUtils()
		{
		}
				
		public static object Convert(Type desiredType, String value, String paramName, IDictionary files, NameValueCollection paramList)
		{
			return Convert(desiredType, new String[] { value }, paramName, files, paramList);
		}

		public static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, NameValueCollection paramList)
		{
			bool conversionSucceeded; 
			return Convert(desiredType, values, paramName, files, paramList, out conversionSucceeded);
		}
		
		public static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, NameValueCollection paramList, out bool conversionSucceeded)
		{
			String value = null;

			if (values != null && values.Length > 0) 
			{
				value = values[0];
				conversionSucceeded = true;
			}
			else
			{
				conversionSucceeded = false;
			}

			if (desiredType == typeof(String))
			{
				return value;
			}
			else if (desiredType.IsArray)
			{
				return values != null ? ConvertToArray(desiredType, values, paramName, files, paramList) : null;
			}
			else if (desiredType.IsEnum)
			{
				if (value == String.Empty || value == null) return null;
				
				if (!Regex.IsMatch(value.ToString(), @"\D", RegexOptions.Compiled)) 
				{
					object enumValue = System.Convert.ChangeType(value, Enum.GetUnderlyingType(desiredType));
					// optional: test if the specified value is valid within the enum
					//if (Enum.IsDefined(desiredType, enumValue))
					//	throw or set enumValue = null
					return enumValue;
				}
				
				return Enum.Parse(desiredType, value, true);
			}
			else if (desiredType.IsPrimitive)
			{
				if (desiredType == typeof(Boolean))
					return (value != null && String.Compare("false", value, true) != 0);
				
				if (value == String.Empty || value == null) 
					return null;
				
				return System.Convert.ChangeType(value, desiredType);
			}
			else if (desiredType == typeof(Decimal))
			{
				if (value == null) return null;
				return System.Convert.ToDecimal(value.ToString());
			}			
			else if (desiredType == typeof(Guid))
			{
				if (value == null) return Guid.Empty;
				return new Guid(value.ToString());
			}
			else if (desiredType == typeof(DateTime))
			{
				if (value == null)
				{
					String day = paramList[paramName + "day"];
					String month = paramList[paramName + "month"];
					String year = paramList[paramName + "year"];

					if (day != null && month != null && year != null)
					{
						try
						{
							// we have found a composite date so we 
							// consider the convertion successful
							conversionSucceeded = true; 

							return new DateTime(
								System.Convert.ToInt32(year), 
								System.Convert.ToInt32(month), 
								System.Convert.ToInt32(day));
						}
						catch(Exception inner)
						{
							String message = String.Format("Invalid date (day {0} month {1} year {2}) for {3} ", 
								day, month, year, paramName);

							throw new ArgumentException(message, inner);
						}
					}
				}

				if (value == null || value == String.Empty)
				{
					return null;
				}
				else
				{
					return DateTime.Parse(value);
				}
			}
			else if (desiredType == typeof(HttpPostedFile))
			{
				conversionSucceeded = true; // if we get some files we don't care about the values being null
				return files[paramName];
			}
			else 
			{
				// support for types that specify a TypeConverter, i.e.: NullableTypes
				TypeConverter conv = TypeDescriptor.GetConverter(desiredType);
				if (conv != null && value != null && conv.CanConvertFrom(value.GetType()))
					return conv.ConvertFrom(value);
				
				String message = String.Format("Cannot convert argument '{0}' with value '{1}' " + 
					"as we don't know how to convert from this value to its type. " +
					"desired type = {2}", paramName, value, desiredType);
	
				throw new ArgumentException(message);
			}
		}

		private static object ConvertToArray(Type desiredType, String[] values, String paramName, IDictionary files, NameValueCollection paramList)
		{
			Type elemType	= desiredType.GetElementType();

			// Fix for mod_mono issue where array values are passed 
			// as a comma seperated String
			if(values.Length == 1 && (values[0].IndexOf(',') > -1))
			{
				values = values[0].Split(',');
			}

			Array newArray	= Array.CreateInstance(elemType, values.Length);
	
			for(int i=0; i < values.Length; i++)
			{
				newArray.SetValue(Convert(elemType, new String[] { values[i] }, paramName, files, paramList), i);
			}
	
			return newArray;
		}		
	}
}
