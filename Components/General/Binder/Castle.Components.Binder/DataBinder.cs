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
	using System.Reflection;
	using System.Collections;

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
		protected internal static readonly BindingFlags PropertiesBindingFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		protected String prefix;
		protected IDictionary files;
		protected IList errorList;
		protected String excludedProperties;
		protected String allowedProperties;
		protected String[] excludedPropertyList;
		protected String[] allowedPropertyList;

		public DataBinder(String prefix) : this(prefix, new Hashtable(), new ArrayList(), "", "")
		{
		}

		public DataBinder(String prefix, IDictionary files, IList errorList, 
			String allowedProperties, String excludedProperties)
		{
			this.prefix = prefix;
			this.files = files;
			this.errorList = errorList;
			this.allowedProperties = allowedProperties;
			this.excludedProperties = excludedProperties;
		}

		public IList Errors
		{
			get { return errorList; }
		}

		public object BindObject(Type targetType, IBindingDataSourceNode dataSource)
		{
			excludedPropertyList = CreateNormalizedList(excludedProperties);
			allowedPropertyList = CreateNormalizedList(allowedProperties);

			return InternalBindObject(targetType, prefix, dataSource.ObtainNode(prefix));
		}

		public void BindObjectInstance(object instance, IBindingDataSourceNode dataSource)
		{
			InternalRecursiveBindObjectInstance(instance, prefix, dataSource.ObtainNode(prefix));
		}

		protected object InternalBindObject(Type instanceType, String paramPrefix, IBindingDataSourceNode node)
		{
			if (ShouldIgnoreType(instanceType)) return null;

			if (instanceType.IsArray)
			{
				return InternalBindObjectArray(instanceType, paramPrefix, node);
			}
			else
			{
				object instance = CreateInstance(instanceType, paramPrefix, node);
				InternalRecursiveBindObjectInstance(instance, paramPrefix, node);
				return instance;
			}
		}

		protected void InternalRecursiveBindObjectInstance(
			object instance, String paramPrefix, IBindingDataSourceNode node)
		{
			if (node == null || node.ShouldIgnore) return;

			BeforeBinding(instance, paramPrefix, node);

			PropertyInfo[] props = instance.GetType().GetProperties(PropertiesBindingFlags);

			foreach(PropertyInfo prop in props)
			{
				if (ShouldIgnoreProperty(prop)) continue;

				Type propType = prop.PropertyType;
				String paramName = prop.Name;

				try
				{
					if (!IsSimpleProperty(propType))
					{
						// if the property is an object, we look if it is already instanciated
						object value = prop.GetValue(instance, null);

						IBindingDataSourceNode nestedNode = node.ObtainNode(paramName);

						if (nestedNode != null)
						{
							// if it's not there, we create it
							// Or if is an array
							if (value == null || propType.IsArray)
							{
								value = InternalBindObject(propType, paramName, nestedNode);
								prop.SetValue(instance, value, null);
							}
							else // if the object already instanciated, then we use it 
							{
								InternalRecursiveBindObjectInstance(value, paramName, nestedNode);
							}
						}
					}
					else
					{
						bool conversionSucceeded;

						// String[] values = ctx.ParamList.GetValues(paramName);

						object value = ConvertUtils.Convert(prop.PropertyType, paramName, node, files, out conversionSucceeded);

						// we don't want to set the value if the form param was missing
						// to avoid loosing existing values in the object instance
						if (conversionSucceeded && value != null)
						{
							prop.SetValue(instance, value, null);
						}
					}
				}
				catch(Exception ex)
				{
					if (Errors != null)
					{
						Errors.Add(new DataBindError(prefix, (paramPrefix == "") ? paramName : prop.Name, ex));
					}
					else
					{
						throw;
					}
				}
			}

			AfterBinding(instance, paramPrefix, node);
		}

		protected virtual void AfterBinding(object instance, String prefix, IBindingDataSourceNode node)
		{
		}

		protected virtual void BeforeBinding(object instance, String prefix, IBindingDataSourceNode node)
		{
		}

		private object[] InternalBindObjectArray(Type instanceType, String paramPrefix, IBindingDataSourceNode node)
		{
			if (node == null || node.ShouldIgnore)
			{
				// No data, returns an empty array

				return (object[]) Array.CreateInstance(instanceType.GetElementType(), 0);
			}

			ArrayList bindArray = new ArrayList();

			if (node.IsIndexed)
			{
				foreach(IBindingDataSourceNode elementNode in node.IndexedNodes)
				{
					AddArrayElement(bindArray, instanceType, paramPrefix, elementNode);
				}
			}

			return (object[]) bindArray.ToArray(instanceType.GetElementType());
		}

		private void AddArrayElement(ArrayList bindArray, Type instanceType, String prefix, IBindingDataSourceNode elementNode)
		{
			if (!elementNode.ShouldIgnore)
			{
				object instance = CreateInstance(instanceType.GetElementType(), prefix, elementNode);

				InternalRecursiveBindObjectInstance(instance, prefix, elementNode);

				bindArray.Add(instance);
			}
		}

		#region CreateInstance

		protected virtual object CreateInstance(Type instanceType, String paramPrefix, IBindingDataSourceNode dataSource)
		{
			return Activator.CreateInstance(instanceType);
		}

		#endregion

		protected String[] CreateNormalizedList(String csv)
		{
			if (csv == null || csv.Trim() == String.Empty)
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
			for(int i = 0; i < list.Length; i++)
			{
				list[i] = list[i].Trim();
			}
		}

		private bool ShouldIgnoreProperty(PropertyInfo prop)
		{
			return !prop.CanWrite ||
				(allowedPropertyList != null && Array.IndexOf(allowedPropertyList, prop.Name) == -1) ||
				(excludedPropertyList != null && Array.IndexOf(excludedPropertyList, prop.Name) != -1);
		}

		private bool ShouldIgnoreType(Type instanceType)
		{
			return instanceType.IsAbstract || instanceType.IsInterface;
		}

		private bool IsSimpleProperty(Type propType)
		{
			// When dealing with arrays or lists we want to check
			// the type of the array element type
			if (propType.IsArray || typeof(IList).IsAssignableFrom(propType))
			{
				propType = propType.GetElementType();
			}

			bool isSimple = propType.IsPrimitive || 
				propType.IsEnum ||
				propType == typeof(String) ||
				propType == typeof(Guid) ||
				propType == typeof(DateTime) ||
				propType == typeof(Decimal);

			if (isSimple) return true;

			TypeConverter converter = TypeDescriptor.GetConverter(propType);
			return converter.CanConvertFrom( typeof(String) );
		}
	}
}