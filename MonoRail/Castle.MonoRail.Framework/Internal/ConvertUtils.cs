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

		public static object Convert(Type desiredType, String paramName, NameValueCollection paramList, IDictionary files)
		{			
			bool conversionSucceeded;

			return Convert(desiredType, paramName, paramList, files, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, String paramName, NameValueCollection paramList, IDictionary files, out bool conversionSucceeded)
		{	
			object input = GetInput(desiredType, paramName, paramList, files);

			return Convert(desiredType, input, out conversionSucceeded);
		}

		public static object Convert(Type desiredType, object value)
		{			
			bool conversionSucceeded;

			return Convert(desiredType, value, out conversionSucceeded);
		}

		/// <summary>
		/// Convert the input param into the desired type
		/// </summary>
		/// <param name="desiredType">Type of the desired.</param>
		/// <param name="input">The input.</param>
		/// <param name="conversionSucceeded">if set to <c>true</c> [conversion succeeded].</param>
		/// <returns>
		/// There are 3 possible cases when trying to convert:
		/// 1) Input data for conversion missing (input is null or an empty string)
		///		Returns default conversion value (based on desired type) and set conversionSucceeded = false
		/// 2) Has input data but cannot convert to particular type
		///		Throw exception and set conversionSucceeded = false
		/// 3) Has input data and can convert to particular type
		/// 	 Return input converted to desired type and set conversionSucceeded = true
		/// </returns>
		public static object Convert(Type desiredType, object input, out bool conversionSucceeded)
		{			
			try
			{
				conversionSucceeded = (input != null);

				if (desiredType == typeof(String))
				{
					return conversionSucceeded ? input.ToString() : String.Empty;
				}
				else if (desiredType.IsArray)
				{
					return conversionSucceeded ? ConvertToArray(desiredType, input, out conversionSucceeded) : null;
				}
				else if (desiredType.IsEnum)
				{
					string value = NormalizeString(input as string);

					if (value == String.Empty)
					{
						conversionSucceeded = false;
						return null;
					}
					else if (!Regex.IsMatch( value, @"\D", RegexOptions.Compiled)) 
					{
						object enumValue = System.Convert.ChangeType(value, Enum.GetUnderlyingType(desiredType));
						// optional: test if the specified value is valid within the enum
						//if (Enum.IsDefined(desiredType, enumValue))
						//	throw or set enumValue = null
						return enumValue;
					}
					else
					{
						return Enum.Parse(desiredType, value, true);	
					}									
				}
				else if (desiredType.IsPrimitive)
				{
					string value = NormalizeString(input as string);

					if (value == String.Empty)
					{
						conversionSucceeded = false;
						return Activator.CreateInstance(desiredType);
					}
					else if (desiredType == typeof(Boolean))
					{
						return String.Compare( "false", value, true ) != 0;
					}					
					else
					{
						return System.Convert.ChangeType(value, desiredType);	
					}							
				}
				else if (desiredType == typeof(Decimal))
				{
					string value = NormalizeString(input as string);
					
					if( value == String.Empty )
					{
						conversionSucceeded = false;
						return Decimal.Zero;
					}
					else
					{
						return System.Convert.ToDecimal( value );	
					}					
				}			
				else if (desiredType == typeof(Guid))
				{
					string value = NormalizeString(input as string);

					if( value == String.Empty )
					{
						conversionSucceeded = false;
						return Guid.Empty;
					}
					else
					{
						return new Guid(value);	
					}		
				}
				else if (desiredType == typeof(DateTime))
				{
					return ConvertDate(input, out conversionSucceeded);
				}
				else if (desiredType == typeof(HttpPostedFile))
				{
					return input;
				}
				else 
				{
					// support for types that specify a TypeConverter, i.e.: NullableTypes
					Type sourceType = (input != null ? input.GetType() : typeof(String));
					TypeConverter conv = TypeDescriptor.GetConverter(desiredType);

					if (conv != null && conv.CanConvertFrom(sourceType))
					{
						return conv.ConvertFrom(input);
					}
					else
					{
						String message = String.Format("Cannot convert argument '{0}', with value '{1}', "+
							"from {2} to {3}", "input", input, sourceType, desiredType);
	
						throw new ArgumentException(message);						
					}
				}				
			}
			catch(Exception)
			{
				conversionSucceeded = false;
				throw;
			}
		}

		private static object ConvertDate( object input, out bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if( input != null && input.GetType().IsArray )
			{
				Array inputArray = input as Array;

				if( inputArray.Length != 3 || input.GetType().GetElementType() != typeof(string) )
				{
					String message = String.Format( "Convert DateTime expects string array with size 3, input was {0} with size {1}", 
					                                input.GetType().GetElementType().FullName, inputArray.Length );

					throw new ArgumentException( message );					
				}

				try
				{												
					int numYear = System.Convert.ToInt32(inputArray.GetValue(0));
					int numMonth = System.Convert.ToInt32(inputArray.GetValue(1));
					int numDay = System.Convert.ToInt32(inputArray.GetValue(2));
							
					int daysInMonth = DateTime.DaysInMonth( numYear, numMonth);
							
					if( numDay > 31 )
					{
						throw new ArgumentException( String.Format( "Convert DateTime day {1} is too big", numDay) );
					}
							
					if( numDay > daysInMonth ) numDay = daysInMonth;

					return new DateTime( numYear, numMonth, numDay );
				}
				catch(Exception inner)
				{
					String message = String.Format( "Convert DateTime invalid date (day {2} month {1} year {0})", 
					                                (object[]) inputArray  );

					throw new ArgumentException(message, inner);
				}					
			}
			else
			{
				string value = NormalizeString(input as string);
					
				if(value == String.Empty)
				{
					conversionSucceeded = false;
					return DateTime.Now;
				}
				else
				{
					return DateTime.Parse(value);
				}
			}
		}

		private static object ConvertToArray(Type desiredType, object input, out bool conversionSucceeded)
		{
			Type elemType = desiredType.GetElementType();

			// Fix for mod_mono issue where array values are passed 
			// as a comma seperated String
			if( !input.GetType().IsArray )
			{
				if( input.GetType() == typeof(string) )
				{
					input = NormalizeString(input.ToString());

					if( input.ToString() == String.Empty )
					{
						input = Array.CreateInstance(elemType, 0);
					}
					else
					{
						input = input.ToString().Split( ',' );		
					}										
				}
				else
				{
					throw new RailsException("Cannot convert to array type {0} from type {1}", elemType.FullName, input.GetType().FullName );
				}				
			}			

			Array values = input as Array;
			Array result = Array.CreateInstance(elemType, values.Length);
			bool elementConversionSucceeded = conversionSucceeded = false;

			for(int i=0; i < values.Length; i++)
			{
				result.SetValue( Convert(elemType, values.GetValue(i), out elementConversionSucceeded), i);
				// if at least one array element got converted we consider the conversion a success
				if( elementConversionSucceeded ) conversionSucceeded = true;
			}
	
			return result;
		}

		private static string NormalizeString( string input )
		{
			return (input == null) ? String.Empty : input.Trim();
		}

		public static bool HasConvertInput(Type desiredType, string paramName, NameValueCollection paramList, IDictionary fileList)
		{
			return GetInput(desiredType, paramName, paramList, fileList) != null;
		}

		private static object GetInput( Type type, string paramName, NameValueCollection paramList, IDictionary fileList )
		{
			if( type == typeof(HttpPostedFile) )
			{
				return fileList[paramName];
			}
			else
			{
				object input = paramList[paramName];

				// When type == DateTime we should look for params of type paramNameyear,month,day
				if( input == null && type == typeof(DateTime) &&
					paramList[paramName + "day"]   != null &&
					paramList[paramName + "month"] != null &&
					paramList[paramName + "year"]  != null )
				{
					return new string[]{ paramList[paramName + "year"], paramList[paramName + "month"], paramList[paramName + "day"] };
				}
				
				return input;				
			}
		}		
	}
}
