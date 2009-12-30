// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.ComponentModel
{
	using System;
	using System.ComponentModel;
	using System.Reflection;

	public class ProxyTypeDescriptor : ICustomTypeDescriptor
	{
		private readonly Type type;
		private readonly object instance;
		private PropertyDescriptor[] properties;

		public ProxyTypeDescriptor(Type type, object instance)
		{
			this.type = type;
			this.instance = instance;
		}

		public AttributeCollection GetAttributes()
		{
			throw new NotImplementedException();
		}

		public string GetClassName()
		{
			throw new NotImplementedException();
		}

		public string GetComponentName()
		{
			throw new NotImplementedException();
		}

		public TypeConverter GetConverter()
		{
			throw new NotImplementedException();
		}

		public EventDescriptor GetDefaultEvent()
		{
			throw new NotImplementedException();
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			throw new NotImplementedException();
		}

		public object GetEditor(Type editorBaseType)
		{
			throw new NotImplementedException();
		}

		public EventDescriptorCollection GetEvents()
		{
			throw new NotImplementedException();
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			throw new NotImplementedException();
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return new PropertyDescriptorCollection(Properties, true);
		}

		protected PropertyDescriptor[] Properties
		{
			get
			{
				if (properties == null)
				{
					properties = GetPropertiesInternal();
				}
				return properties;
			}
		}

		protected virtual PropertyDescriptor[] GetPropertiesInternal()
		{
			var proxyProperties = type.GetProperties();
			var propertyDescriptors = new ProxyPropertyDescriptor[proxyProperties.Length];
			for (int i = 0; i < proxyProperties.Length; i++)
			{
				var attributes = GetAttributes(proxyProperties[i]);
				propertyDescriptors[i] = new ProxyPropertyDescriptor(proxyProperties[i].Name,
				                                                     attributes, type);
			}
			return propertyDescriptors;
		}

		private Attribute[] GetAttributes(PropertyInfo proxyProperty)
		{
			var customAttributes = proxyProperty.GetCustomAttributes(true);
			var attributes = new Attribute[customAttributes.Length];
			customAttributes.CopyTo(attributes, 0);
			return attributes;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			throw new NotImplementedException();
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			throw new NotImplementedException();
		}
	}
}