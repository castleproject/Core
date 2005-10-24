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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;
	using System.Reflection;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text.RegularExpressions;

	/// <summary>
	/// A DataBinder can be used to map properties from 
	/// a NameValueCollection to one or more instance types.
	/// </summary>
	/// <remarks>
	/// This code is messy. We need more test cases so we can 
	/// refactor it mercyless
	/// </remarks>
	public class DataBinder
	{
		protected internal static readonly String MetadataIdentifier = "@";
		protected internal static readonly String IgnoreAttribute = MetadataIdentifier + "ignore";
		protected internal static readonly String CountAttribute = MetadataIdentifier + "count";
		protected internal static readonly String Yes = "yes";
		protected internal static readonly int DefaultNestedLevelsLeft = 3;
		protected internal static readonly BindingFlags PropertiesBindingFlags = 
			BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private String root = null;
		private IRailsEngineContext context;
		
		#region Constructors
		
		public DataBinder(IRailsEngineContext context)
		{
			this.context = context;
		}

		#endregion

		#region BindObject family

		public object BindObject(Type instanceType)
		{
			return BindObject(instanceType, String.Empty, context.Params, context.Request.Files, null, DefaultNestedLevelsLeft, String.Empty);
		}

		public object BindObject(Type instanceType, String paramPrefix, IList errorList, int nestedLevel, String excludedProperties)
		{
			return BindObject(instanceType, paramPrefix, context.Params, context.Request.Files, errorList, nestedLevel, excludedProperties);
		}

		public object BindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel, String excludedProperties)
		{
			if (instanceType.IsAbstract || instanceType.IsInterface) return null;
			if (root == null) root = instanceType.Name;

			if (ShouldIgnoreElement( paramList, paramPrefix )) return null;

			if (instanceType.IsArray)
			{
				return BindObjectArrayInstance(instanceType, paramPrefix, paramList, 
					files, errorList, nestedLevel, excludedProperties);
			}
			else
			{
				object instance = CreateInstance(instanceType, paramPrefix, paramList);
				
				return BindObjectInstance(instance, paramPrefix, paramList, files, 
					errorList, nestedLevel, excludedProperties);
			}			
		}
		
		#endregion
			
		#region CreateInstance 
		 
		protected virtual object CreateInstance( Type instanceType, string paramPrefix, NameValueCollection paramsList )
		{
			return Activator.CreateInstance(instanceType);
		}
		
		protected virtual object CreateArrayElementInstance( Type instanceType, string paramPrefix, NameValueCollection paramsList )
		{
			return CreateInstance( instanceType.GetElementType(), paramPrefix, paramsList );
		}
		
		#endregion
		
		#region BindObjectInstance
		
		public object[] BindObjectArrayInstance(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevelsLeft, String excludedProperties)
		{
			if(ShouldIgnoreElement( paramList, paramPrefix )) return null;
			
			ArrayList bindArray = new ArrayList();
	
			// check if a count attribute is present, if so we assume the
			// params are in order i.e.
			// param[0], param[1], ... param[count-1]
			// otherwise we have to find all uniques id for that identifier
			// which is probably slower but is more flexible
			string countBeforeCast = paramList[paramPrefix + CountAttribute ];	
		
			if( countBeforeCast != null )
			{
				Int32 count = System.Convert.ToInt32( countBeforeCast );					
				
				// if count > paramList.Count that means that there is a problem
				// in the count variable
				if( count > paramList.Count ) count = paramList.Count;
					
				for(int i=0; i < count; i++)
				{
					String arrayParamPrefix = paramPrefix + "[" + i + "]";

					AddArrayInstance(bindArray, instanceType, arrayParamPrefix, 
						paramList, files, errorList, nestedLevelsLeft, excludedProperties );											
				}				
			}
			else
			{
				String[] uniquePrefixes = Grep( paramList.AllKeys, "^" + Regex.Escape( paramPrefix ) + @"\[(.*?)]", 1 );

				foreach( string prefix in uniquePrefixes )
				{
					String arrayParamPrefix = paramPrefix + "[" + prefix + "]";
				
					AddArrayInstance(bindArray, instanceType, arrayParamPrefix, 
						paramList, files, errorList, nestedLevelsLeft, excludedProperties );
				}				
			}
						
			return (object[]) bindArray.ToArray( instanceType.GetElementType() );		
		}

		#endregion
				
		#region BindObjectInstance

		private void AddArrayInstance( ArrayList bindArray, Type instanceType, string arrayParamPrefix, NameValueCollection paramList, IDictionary files, IList errorList, int nestedLevelsLeft, string excludedProperties )
		{
			if( !ShouldIgnoreElement( paramList, arrayParamPrefix ) )
			{
				object instance = CreateArrayElementInstance(
					instanceType, arrayParamPrefix, paramList);
				
				BindObjectInstance( instance, 
					arrayParamPrefix, 
					paramList, 
					files, 
					errorList, 
					nestedLevelsLeft, 
					excludedProperties );

				bindArray.Add( instance );
			}
		}

		public object BindObjectInstance(object instance, String paramPrefix)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance(instance,  paramPrefix, context.Params, context.Request.Files, 
				null, DefaultNestedLevelsLeft, string.Empty);
		}

		public object BindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance( instance, paramPrefix, paramList, files, errorList, DefaultNestedLevelsLeft, string.Empty );
		}

		public object BindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel, String excludedProperties)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			return InternalRecursiveBindObjectInstance( instance, paramPrefix, paramList, files, errorList, nestedLevel, excludedProperties );
		}

		#endregion
		
		#region InternalBindObject
		
		private object InternalBindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevelsLeft, string excludedProperties)
		{
			if (instanceType.IsAbstract || instanceType.IsInterface) return null;
			if (root == null) root = instanceType.Name;

			object instance = CreateInstance(instanceType, paramPrefix, paramList);

			return InternalRecursiveBindObjectInstance(instance, paramPrefix, paramList, files, 
				errorList, nestedLevelsLeft, excludedProperties);
		}
		
		private object InternalRecursiveBindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevelsLeft, string excludedProperties)
		{
			if (--nestedLevelsLeft < 0)
			{
				return instance;
			}
			
			if( ShouldIgnoreElement( paramList, paramPrefix ) ) return null;

			PropertyInfo[] props = instance.GetType().GetProperties(PropertiesBindingFlags);

			String[] excludeList = CreateExcludeList(excludedProperties);

			foreach (PropertyInfo prop in props)
			{
				if (!prop.CanWrite || Array.IndexOf(excludeList, prop.Name) != -1)
				{
					continue;
				}

				Type propType = prop.PropertyType;
								
				try
				{
					if ( !IsSimpleProperty(propType) )
					{
						// if the property is an object, we look if it is already instanciated
						object value = prop.GetValue(instance, null);
						
						String propName = BuildParamName(paramPrefix, prop.Name);

						if( propType.IsArray )
						{
							value = BindObjectArrayInstance(
										propType, propName, paramList, files, 										
										errorList, nestedLevelsLeft, excludedProperties);
							prop.SetValue(instance, value, null);
						}
						else if (value == null) // if it's not there, we create it
						{
							value = InternalBindObject(prop.PropertyType, propName, paramList, files, 
								errorList, nestedLevelsLeft, excludedProperties);
							
							prop.SetValue(instance, value, null);
						}
						else // if the object already instanciated, then we use it 
						{
							InternalRecursiveBindObjectInstance(value, propName, paramList, files, 
								errorList, nestedLevelsLeft, excludedProperties);
						}
					}
					else
					{
						String paramName = BuildParamName(paramPrefix, prop.Name); 

						bool conversionSucceeded;

						String[] values = paramList.GetValues(paramName);
						
						object value = Convert(prop.PropertyType, values, paramName, files, context, out conversionSucceeded);
						
						// we don't want to set the value if the form param was missing
						// to avoid loosing existing values in the object instance
						if (conversionSucceeded && value != null)
						{
							prop.SetValue(instance, value, null);
						}
					}						
				}
				catch (Exception ex)
				{
					if (errorList != null)
					{
						errorList.Add(new DataBindError(root, BuildParamName(paramPrefix, prop.Name), ex));
					}
					else
					{
						throw;
					}
				}
			}

			return instance;
		}

		#endregion
		
		#region Helpers

		private bool IsSimpleProperty(Type propType)
		{
			// When dealing with arrays or lists we want to check
			// the type of the array element type
			if( propType.IsArray || typeof(IList).IsAssignableFrom(propType) )
				propType = propType.GetElementType();
				
			return propType.IsPrimitive || propType.IsEnum ||
				   propType == typeof(String) ||
				   propType == typeof(Guid) || 
				   propType == typeof(DateTime);
		}
		
		private bool ShouldIgnoreElement( NameValueCollection paramList, string paramPrefix )
		{		
			return Yes.Equals( paramList.Get(paramPrefix + IgnoreAttribute) ); 
		}

		private String[] CreateExcludeList(String excludedProperties)
		{
			String[] excludeList = excludedProperties.Split(',');
	
			if (excludedProperties != null)
			{
				excludeList = excludedProperties.Split(',');
				NormalizeExcludeList(excludeList);
			}
			else
			{
				excludeList = new String[] { String.Empty };
			}

			return excludeList;
		}

		private void NormalizeExcludeList(String[] excludeList)
		{
			for(int i=0; i < excludeList.Length; i++)
			{
				excludeList[i] = excludeList[i].Trim();
			}
		}
				
		private static String NormalizeParamPrefix(String paramPrefix)
		{
			return (paramPrefix != null && paramPrefix != String.Empty) ? 
				paramPrefix.ToLower(CultureInfo.InvariantCulture) : String.Empty;
		}

		private static String BuildParamName(String prefix, String name)
		{
			if (prefix != String.Empty)
			{
				return String.Format("{0}.{1}", prefix, name);
			}
			else
			{
				return name;
			}
		}
		/// <summary>
		/// Similar to the grep in perl but with the option
		/// of specifying if you want to capture the value to be returned
		/// i.e. 
		/// <code>
		/// list = param[0], param[1], param[2]
		/// Grep( list, "^param", 0 ) => [ "param[0]", "param[1]", "param[2]" ]
		/// Grep( list, "^param\[(.*?)]", 1 ) => [ "0", "1", "2" ]
		/// </code>
		/// Note that it only returns distinct values
		/// </summary>
		public static string[] Grep( string[] values, string pattern, int captureNumber )
		{
			NameValueCollection results = new NameValueCollection();
			Regex re = new Regex( pattern, RegexOptions.IgnoreCase );
			
			foreach(string value in values)
			{
				// Note: I have this check here cause when using testsupport
				// the paramList attribute passed to the databinder had a mixture
				// of servervariables and post/query string params, and one of them
				// was a null value, which cause an exception, I need to verify if 
				// this is also true when running through IIS
				if( value == null ) continue;
				
				Match match = re.Match(value);

				if( match.Success )
				{
					if( match.Groups.Count >= captureNumber )
					{
						results[ match.Groups[captureNumber].Value ] = String.Empty;
					}
				}
			}

			return results.AllKeys;
		}
				
		#endregion
		
		#region Convert
		
		public static object Convert(Type desiredType, String value, String paramName, IDictionary files, IRailsEngineContext context)
		{
			return Convert(desiredType, new String[] { value }, paramName, files, context);
		}

		public static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context)
		{
			bool conversionSucceeded; 
			return Convert(desiredType, values, paramName, files, context, out conversionSucceeded);
		}
		
		private static object Convert(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context, out bool conversionSucceeded)
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
				return values != null ? ConvertToArray(desiredType, values, paramName, files, context) : null;
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
			else if (desiredType == typeof(Guid))
			{
				if (value == null) return Guid.Empty;
				return new Guid(value.ToString());
			}
			else if (desiredType == typeof(DateTime))
			{
				if (value == null)
				{
					String day = context.Params[paramName + "day"];
					String month = context.Params[paramName + "month"];
					String year = context.Params[paramName + "year"];

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
				conversionSucceeded = false;

				if (context != null)
				{
					String message = String.Format("Ignoring argument '{0}' with value '{1}' " + 
						"as we don't know how to convert from this value to its type. " +
						"desired type = {2}", paramName, value, desiredType);
	
					context.Trace.Warn(message);
				}
			}

			return null;
		}

		private static object ConvertToArray(Type desiredType, String[] values, String paramName, IDictionary files, IRailsEngineContext context)
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
				newArray.SetValue(Convert(elemType, new String[] { values[i] }, paramName, files, context), i);
			}
	
			return newArray;
		}
		
		#endregion		
	}
}
