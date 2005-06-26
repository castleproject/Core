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
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Reflection;
	using System.Web;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// A DataBinder can be used to map properties from 
	/// a NameValueCollection to one or more instance types.
	/// </summary>
	public class DataBinder
	{
		private IRailsEngineContext context;
		private IInstanceFactory instanceFactory;
		
		public DataBinder( IInstanceFactory instanceFactory, IRailsEngineContext context )
		{
			this.context = context;
			this.instanceFactory = instanceFactory;
		}

		public object BindObject( Type instanceType, string paramPrefix, NameValueCollection paramList, IDictionary files )
		{
			string prefix = (paramPrefix != null && paramPrefix != string.Empty) ?  paramPrefix.ToLower( CultureInfo.InvariantCulture ) + "." : string.Empty;
			object instance = instanceFactory.GetInstance( instanceType, context );
			
			PropertyInfo[] props = instanceType.GetProperties( BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

			foreach ( PropertyInfo prop in props )
			{
				if ( prop.CanWrite )
				{
					Type propType = prop.PropertyType;

					object value = null;

					if ( !propType.IsPrimitive && !propType.IsArray && propType != typeof(String) && propType != typeof(Guid)
							&& propType != typeof(DateTime) && propType != typeof(HttpPostedFile) )
					{
						value = BindObject( prop.PropertyType, prefix + prop.Name, paramList, files );
					}
					else
					{
						value = Convert( prop.PropertyType, paramList.GetValues( prefix + prop.Name ), prop.Name, files );
					}
					
					prop.SetValue( instance, value, null );
				}
			}

			return instance;
		}

		public object Convert( Type desiredType, string[] values, string paramName, IDictionary files )
		{
			string value = ( values != null && values.Length > 0 ) ? values[0] : null;

			if (desiredType == typeof(String))
			{
				return value;
			}
			else if (desiredType == typeof(Guid))
			{
				if (value != null)
				{
					return new Guid( value.ToString() );
				}
				else
				{
					return Guid.Empty; 
				}
			}
			else if (desiredType == typeof(UInt16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt16( value );
			}
			else if (desiredType == typeof(UInt32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt32( value );
			}
			else if (desiredType == typeof(UInt64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt64( value );
			}
			else if (desiredType == typeof(Int16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt16( value );
			}
			else if (desiredType == typeof(Int32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt32( value );
			}
			else if (desiredType == typeof(Int64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt64( value );
			}
			else if (desiredType == typeof(Byte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToByte( value );
			}
			else if (desiredType == typeof(SByte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSByte( value );
			}
			else if (desiredType == typeof(Single))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSingle( value );
			}
			else if (desiredType == typeof(Double))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToDouble( value );
			}
			else if (desiredType == typeof(DateTime))
			{
				return value == null || value == string.Empty ? new DateTime(): DateTime.Parse(value);
			}
			else if (desiredType == typeof(Boolean))
			{
				// TODO: Add true/on/1 variants
				return value != null;
			}
			else if (desiredType == typeof(HttpPostedFile))
			{
				// TODO: An array of posted files ??
				return files[paramName];
			}
			else if (desiredType.IsArray)
			{
				return values != null ? ConvertToArray( desiredType, values, paramName, files ) : null;
			}
			else if ( context != null )
			{
				String message = String.Format("Ignoring argument {0} with value {1} " + 
					" as we don't know how to convert from this value to its type", paramName, value);
				context.Trace(message);
			}

			return null;
		}

		private object ConvertToArray( Type desiredType, string[] values, string paramName, IDictionary files )
		{
			Type elemType	= desiredType.GetElementType();
			Array newArray	= Array.CreateInstance(elemType, values.Length);
	
			for( int i=0; i < values.Length; i++)
			{
				newArray.SetValue( Convert(elemType, new string[] { values[i] }, paramName, files), i );
			}
	
			return newArray;
		}
	}
}
