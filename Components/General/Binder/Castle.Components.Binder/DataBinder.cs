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
	/// a <see cref="IBindingDataSourceNode"/> to one or more instance types.
	/// </summary>
	public class DataBinder : IDataBinder
	{
		#region Fields

		protected internal static readonly BindingFlags PropertiesBindingFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		/// <summary>Collect the databind errors</summary>
		protected IList errors;
		
		/// <summary>Holds a reference to a hash of string to <c>HttpPostedFiles</c></summary>
		private IDictionary files;

		/// <summary>Holds a sorted array of properties names that should be ignored</summary>
		private String[] excludedPropertyList;

		/// <summary>Holds a sorted array of properties names that are on the white list</summary>
		private String[] allowedPropertyList;

		private Stack instanceStack;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a <c>DataBinder</c>
		/// </summary>
		public DataBinder() : this(new Hashtable())
		{
		}

		/// <summary>
		/// Constructs a <c>DataBinder</c> 
		/// with a hash of string to <c>HttpPostedFiles</c>
		/// </summary>
		public DataBinder(IDictionary files)
		{
			this.files = files;
		}

		#endregion

		#region IDataBinder

		/// <summary>
		/// Create an instance of the specified type and binds the properties that
		/// are available on the datasource.
		/// </summary>
		/// <param name="targetType">The target type. Can be an array</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="dataSource">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		public object BindObject(Type targetType, String prefix, IBindingDataSourceNode dataSource)
		{
			return BindObject(targetType, prefix, null, null, dataSource);
		}

		/// <summary>
		/// Create an instance of the specified type and binds the properties that
		/// are available on the datasource respecting the white and black list
		/// </summary>
		/// <param name="targetType">The target type. Can be an array</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="excludedProperties">A list of comma separated values specifing the properties that should be ignored</param>
		/// <param name="allowedProperties">A list of comma separated values specifing the properties that should not be ignored</param>
		/// <param name="dataSource">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		public object BindObject(Type targetType, String prefix, String excludedProperties, String allowedProperties, IBindingDataSourceNode dataSource)
		{
			if (targetType == null) throw new ArgumentNullException("targetType");
			if (prefix == null) throw new ArgumentNullException("prefix");
			if (dataSource == null) throw new ArgumentNullException("dataSource");

			errors = new ArrayList();
			instanceStack = new Stack();

			excludedPropertyList = CreateNormalizedList(excludedProperties);
			allowedPropertyList = CreateNormalizedList(allowedProperties);

			return InternalBindObject(targetType, prefix, dataSource.ObtainNode(prefix));
		}

		/// <summary>
		/// Binds the properties that are available on the datasource to the specified object instance.
		/// </summary>
		/// <param name="instance">The target instance.</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="dataSource">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		public void BindObjectInstance(object instance, String prefix, IBindingDataSourceNode dataSource)
		{
			BindObjectInstance(instance, prefix, null, null, dataSource);
		}

		/// <summary>
		/// Binds the properties that
		/// are available on the datasource respecting the white and black list
		/// </summary>
		/// <param name="instance">The target type.</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="excludedProperties">A list of comma separated values specifing the properties that should be ignored</param>
		/// <param name="allowedProperties">A list of comma separated values specifing the properties that should not be ignored</param>
		/// <param name="dataSource">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		public void BindObjectInstance(object instance, String prefix, String excludedProperties, String allowedProperties, IBindingDataSourceNode dataSource)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (prefix == null) throw new ArgumentNullException("prefix");
			if (dataSource == null) throw new ArgumentNullException("dataSource");

			errors = new ArrayList();
			instanceStack = new Stack();

			excludedPropertyList = CreateNormalizedList(excludedProperties);
			allowedPropertyList = CreateNormalizedList(allowedProperties);

			InternalRecursiveBindObjectInstance(instance, prefix, dataSource.ObtainNode(prefix));
		}

		/// <summary>
		/// Represents the databind errors
		/// </summary>
		public ErrorList ErrorList
		{
			get { return new ErrorList(errors); }
		}

		/// <summary>
		/// Holds a reference to a hash of string to <c>HttpPostedFiles</c>
		/// </summary>
		public IDictionary Files
		{
			get { return files; }
			set { files = value; }
		}

		#endregion

		protected object InstanceOnStack
		{
			get
			{
				if (instanceStack.Count == 0) return null;
				
				return instanceStack.Peek();
			}
		}

		#region Overridables

		protected virtual void AfterBinding(object instance, String prefix, IBindingDataSourceNode node)
		{
		}

		protected virtual void BeforeBinding(object instance, String prefix, IBindingDataSourceNode node)
		{
		}

		protected virtual bool ShouldRecreateInstance(object value, Type type, String prefix, IBindingDataSourceNode node)
		{
			return value == null || type.IsArray;
		}

		protected virtual bool ShouldIgnoreType(Type instanceType)
		{
			return instanceType.IsAbstract || instanceType.IsInterface;
		}

		protected virtual bool PerformCustomBinding(object instance, string prefix, IBindingDataSourceNode node)
		{
			return false;
		}

		#endregion

		#region Internal implementation

		protected object InternalBindObject(Type instanceType, String paramPrefix, IBindingDataSourceNode node)
		{
			bool succeeded;
			return InternalBindObject(instanceType, paramPrefix, node, out succeeded);
		}

		protected object InternalBindObject(Type instanceType, String paramPrefix, IBindingDataSourceNode node, out bool succeeded)
		{
			succeeded = false;

			if (ShouldIgnoreType(instanceType)) return null;

			if (instanceType.IsArray)
			{
				return InternalBindObjectArray(instanceType, paramPrefix, node, out succeeded);
			}
			else
			{
				succeeded = true;
				object instance = CreateInstance(instanceType, paramPrefix, node);
				InternalRecursiveBindObjectInstance(instance, paramPrefix, node);
				return instance;
			}
		}

		protected void InternalRecursiveBindObjectInstance(object instance, String prefix, IBindingDataSourceNode node)
		{
			if (node == null || node.ShouldIgnore) return;

			BeforeBinding(instance, prefix, node);

			if (PerformCustomBinding(instance, prefix, node))
			{
				return;
			}

			PushInstance(instance);

			PropertyInfo[] props = instance.GetType().GetProperties(PropertiesBindingFlags);

			foreach(PropertyInfo prop in props)
			{
				if (ShouldIgnoreProperty(prop)) continue;

				Type propType = prop.PropertyType;
				String paramName = prop.Name;

				try
				{
					bool conversionSucceeded;

					if (IsSimpleProperty(propType))
					{
						object value = ConvertUtils.Convert(prop.PropertyType, paramName, node, files, out conversionSucceeded);

						if (conversionSucceeded && value != null)
						{
							prop.SetValue(instance, value, null);
						}
					}
					else
					{
						IBindingDataSourceNode nestedNode = node.ObtainNode(paramName);

						if (nestedNode != null)
						{
							// if the property is an object, we look if it is already instanciated
							object value = prop.GetValue(instance, null);

							if (ShouldRecreateInstance(value, propType, paramName, nestedNode))
							{
								value = InternalBindObject(propType, paramName, nestedNode, out conversionSucceeded);
								
								if (conversionSucceeded)
								{
									prop.SetValue(instance, value, null);
								}
							}
							else
							{
								InternalRecursiveBindObjectInstance(value, paramName, nestedNode);
							}
						}
					}
				}
				catch(Exception ex)
				{
					errors.Add(new DataBindError(prefix, prop.Name, ex));
				}
			}

			PopInstance(instance);

			AfterBinding(instance, prefix, node);
		}

		private object[] InternalBindObjectArray(Type instanceType, String paramPrefix, IBindingDataSourceNode node, out bool succeeded)
		{
			succeeded = false;

			if (node == null || node.ShouldIgnore)
			{
				return null;
			}

			if (node.IsIndexed)
			{
				ArrayList bindArray = new ArrayList();

				succeeded = true;

				foreach(IBindingDataSourceNode elementNode in node.IndexedNodes)
				{
					AddArrayElement(bindArray, instanceType, paramPrefix, elementNode);
				}

				return (object[]) bindArray.ToArray(instanceType.GetElementType());
			}

			return null;
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

		#endregion

		#region CreateInstance

		protected virtual object CreateInstance(Type instanceType, String paramPrefix, IBindingDataSourceNode dataSource)
		{
			return Activator.CreateInstance(instanceType);
		}

		#endregion

		#region Support methods

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
			Array.Sort(list, CaseInsensitiveComparer.Default);
		}

		private bool ShouldIgnoreProperty(PropertyInfo prop)
		{
			if (!prop.CanWrite) return true;

			int index1 = 0; 
			int index2 = -1;

			if (allowedPropertyList != null)
			{
				index1 = Array.BinarySearch(allowedPropertyList, prop.Name, CaseInsensitiveComparer.Default);
			}
			if (excludedPropertyList != null)
			{
				index2 = Array.BinarySearch(excludedPropertyList, prop.Name, CaseInsensitiveComparer.Default);
			}

			return (index1 <= -1) || (index2 >= 0);
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

		private void PushInstance(object instance)
		{
			instanceStack.Push(instance);
		}

		private void PopInstance(object instance)
		{
			object actual = instanceStack.Pop();

			if (actual != instance)
			{
				throw new BindingException("Unexpected item on the stack: found {0}, expecting {1}", actual, instance);
			}
		}

		#endregion
	}
}