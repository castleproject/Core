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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text.RegularExpressions;

	using Castle.MonoRail.Framework.Internal;

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
		#region Constants

		protected internal static readonly String MetadataIdentifier = "@";
		protected internal static readonly String IgnoreAttribute = MetadataIdentifier + "ignore";
		protected internal static readonly String CountAttribute = MetadataIdentifier + "count";
		protected internal static readonly String Yes = "yes";
		protected internal static readonly String No = "no";
		protected internal static readonly int DefaultNestedLevelsLeft = 3;
		protected internal static readonly BindingFlags PropertiesBindingFlags = 
			BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		#endregion
		
		#region BindObject family

		public object BindObject(Type instanceType, String paramPrefix, NameValueCollection paramList)
		{
			return BindObject(instanceType, paramPrefix, paramList, null, null, DefaultNestedLevelsLeft, null);
		}

		public object BindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel, String excludedProperties)
		{
			return BindObject(instanceType, paramPrefix, paramList, files, errorList, nestedLevel, excludedProperties, String.Empty );
		}
		
		public object BindObject(Type instanceType, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList, int nestedLevel, String excludedProperties, String allowProperties)
		{
			String root = GetRoot(instanceType, paramPrefix);
			paramPrefix = NormalizeParamPrefix(paramPrefix);

			String[] excludedPropertyList = CreateNormalizedList(excludedProperties);
			String[] allowPropertyList = CreateNormalizedList(allowProperties);

			DataBindContext ctx = new DataBindContext(root, paramList, files, errorList, excludedPropertyList, allowPropertyList );
			
			return InternalBindObject(instanceType, paramPrefix, nestedLevel, ctx);
		}

		public object BindObjectInstance(object instance, String paramPrefix, NameValueCollection paramList, 
			IDictionary files, IList errorList)
		{
			paramPrefix = NormalizeParamPrefix(paramPrefix);
			String root = GetRoot(instance.GetType(), paramPrefix);
			
			DataBindContext ctx = new DataBindContext(root, paramList, files, errorList, null, null );

			return InternalRecursiveBindObjectInstance( instance, paramPrefix, DefaultNestedLevelsLeft, ctx );
		}
		
		#endregion

		#region CreateInstance
			
		protected virtual object CreateInstance( Type instanceType, String paramPrefix, NameValueCollection paramsList )
		{
			return Activator.CreateInstance(instanceType);
		}

		#endregion
		
		#region Array Support
		
		private object[] InternalBindObjectArray(Type instanceType, String paramPrefix, int nestedLevelsLeft, DataBindContext ctx)
		{
			ArrayList bindArray = new ArrayList();
	
			// check if a count attribute is present, if so we assume the
			// params are in order i.e.
			// param[0], param[1], ... param[count-1]
			// otherwise we have to find all uniques id for that identifier
			// which is probably slower but is more flexible
			String countBeforeCast = ctx.ParamList[paramPrefix + CountAttribute];	
		
			if( countBeforeCast != null )
			{
				Int32 count = System.Convert.ToInt32( countBeforeCast );					
				
				// if count > paramList.Count that means that there is a problem
				// in the count variable
				if( count > ctx.ParamList.Count ) count = ctx.ParamList.Count;
					
				for(int prefix=0; prefix < count; prefix++)
				{
					AddArrayElement(bindArray, instanceType, prefix, paramPrefix, nestedLevelsLeft, ctx);											
				}				
			}
			else
			{
				String[] uniquePrefixes = Grep( ctx.ParamList.AllKeys, "^" + Regex.Escape( paramPrefix ) + @"\[(.*?)]", 1 );

				foreach( String prefix in uniquePrefixes )
				{				
					AddArrayElement(bindArray, instanceType, prefix, paramPrefix, nestedLevelsLeft, ctx );
				}				
			}
						
			return (object[]) bindArray.ToArray( instanceType.GetElementType() );		
		}

		// Only here so we can support 2 types of loop in the BindObjectArrayInstance
		// without duplicating too much code
		private void AddArrayElement( ArrayList bindArray, Type instanceType, object arrayPrefix, String paramPrefix, int nestedLevelsLeft, DataBindContext ctx )
		{
			String arrayParamPrefix = paramPrefix + "[" + arrayPrefix + "]";
			
			if( !ShouldIgnoreElement( ctx.ParamList, arrayParamPrefix ) )
			{
				object instance = CreateInstance(
					instanceType.GetElementType(), arrayParamPrefix, ctx.ParamList);
				
				InternalRecursiveBindObjectInstance( instance, arrayParamPrefix, nestedLevelsLeft, ctx );

				bindArray.Add( instance );
			}
		}

		#endregion
		
		#region InternalBindObject

		protected object InternalBindObject(Type instanceType, String paramPrefix, int nestedLevelsLeft, DataBindContext ctx)
		{		
			if (ShouldIgnoreType(instanceType) ||
			    ShouldIgnoreElement( ctx.ParamList, paramPrefix )) return null;
					
			if (instanceType.IsArray)
			{
				return InternalBindObjectArray(instanceType, paramPrefix, nestedLevelsLeft, ctx);
			}
			else
			{
				object instance = CreateInstance(instanceType, paramPrefix, ctx.ParamList);				
				return InternalRecursiveBindObjectInstance(instance, paramPrefix, nestedLevelsLeft, ctx);
			}			
		}
			
		protected object InternalRecursiveBindObjectInstance(object instance, String paramPrefix, 
			int nestedLevelsLeft, DataBindContext ctx)
		{
			if (--nestedLevelsLeft < 0) return instance;			
			if (ShouldIgnoreElement (ctx.ParamList, paramPrefix)) return instance;

			BeforeBinding(instance, paramPrefix, ctx);

			PropertyInfo[] props = instance.GetType().GetProperties(PropertiesBindingFlags);

			foreach (PropertyInfo prop in props)
			{
				if( ShouldIgnoreProperty(prop, ctx) ) continue;
				
				Type propType = prop.PropertyType;								
				String paramName = BuildParamName(paramPrefix, prop.Name);

				try
				{
					if ( !IsSimpleProperty(propType) )
					{
						if( nestedLevelsLeft > 0 )
						{
							// if the property is an object, we look if it is already instanciated
							object value = prop.GetValue(instance, null);
						
							// if it's not there, we create it
							// Or if is an array
							if ( value == null || propType.IsArray ) 
							{
								value = InternalBindObject(propType, paramName, nestedLevelsLeft, ctx);							
								prop.SetValue(instance, value, null);
							}
							else // if the object already instanciated, then we use it 
							{
								InternalRecursiveBindObjectInstance(value, paramName, nestedLevelsLeft, ctx);
							}							
						}
					}
					else
					{
						bool conversionSucceeded;

						// String[] values = ctx.ParamList.GetValues(paramName);
						
						object value = ConvertUtils.Convert(prop.PropertyType, paramName, ctx.ParamList, ctx.Files, out conversionSucceeded);
						
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
					if (ctx.Errors != null)
					{
						ctx.Errors.Add(new DataBindError( ctx.Root, 
							(paramPrefix == "") ? paramName : prop.Name, ex ));
					}
					else
					{
						throw;
					}
				}
			}

			AfterBinding(instance, paramPrefix, ctx);

			return instance;
		}

		#endregion

		protected virtual void BeforeBinding(object instance, String paramPrefix, DataBindContext context)
		{
			
		}

		protected virtual void AfterBinding(object instance, String paramPrefix, DataBindContext context)
		{
			
		}
		
		#region Helpers

		private bool ShouldIgnoreElement( NameValueCollection paramList,  String paramPrefix )
		{		
			return Yes.Equals( paramList.Get(paramPrefix + IgnoreAttribute) ); 
		}

		private bool ShouldIgnoreProperty( PropertyInfo prop, DataBindContext ctx )
		{
			return !prop.CanWrite || 
				   ( ctx.AllowedProperties != null && 
				     Array.IndexOf( ctx.AllowedProperties, prop.Name) == -1 ) ||
				   ( ctx.ExcludedProperties != null &&  
				     Array.IndexOf( ctx.ExcludedProperties, prop.Name) != -1 ) ;
		}

		private bool ShouldIgnoreType(Type instanceType)
		{
			return instanceType.IsAbstract ||
			       instanceType.IsInterface;
		}
		
		private bool IsSimpleProperty(Type propType)
		{
			// When dealing with arrays or lists we want to check
			// the type of the array element type
			if( propType.IsArray || typeof(IList).IsAssignableFrom(propType) )
				propType = propType.GetElementType();
				
			return propType.IsPrimitive || propType.IsEnum ||
				   propType == typeof(String) ||
				   propType == typeof(Guid) || 
				   propType == typeof(DateTime) ||
				   propType == typeof(Decimal);
		}

		protected String GetRoot( Type type, String prefix )
		{
			return (prefix == null) ? type.Name : prefix;
		}
		
		protected String[] CreateNormalizedList(String csv)
		{
			if( csv == null || csv.Trim() == String.Empty )
			{
				return null;
			}
			else
			{
				String[] list = csv.Split(',');
				NormalizeList(list);
				return list;
			}
		}

		private void NormalizeList(String[] list)
		{
			for(int i=0; i < list.Length; i++)
			{
				list[i] = list[i].Trim();
			}
		}
				
		protected static String NormalizeParamPrefix(String paramPrefix)
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
		public static String[] Grep( String[] values, String pattern, int captureNumber )
		{
			NameValueCollection results = new NameValueCollection();
			Regex re = new Regex( pattern, RegexOptions.IgnoreCase );
			
			foreach( String value in values )
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
	}		
}
