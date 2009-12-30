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

	public class ProxyPropertyDescriptor : PropertyDescriptor
	{
		private readonly string actualName;
		private readonly Type proxyType;
		private readonly PropertyInfo property;

		public ProxyPropertyDescriptor(string name, Attribute[] attrs,Type proxyType) : base(StripName(name), attrs)
		{
			actualName = name;
			property = proxyType.GetProperty(actualName,
			                                 BindingFlags.Public | BindingFlags.NonPublic |
			                                 BindingFlags.Instance | BindingFlags.Static);
			this.proxyType = proxyType;
		}

		private static string StripName(string name)
		{
			var indexOfLastDot = name.LastIndexOf('.');
			if (indexOfLastDot == -1)
			{
				return name;
			}

			if (indexOfLastDot + 1 == name.Length)
			{
				// is it legal that property name ends with a dot?
				return name;
			}

			return name.Substring(indexOfLastDot + 1);
		}

		public override bool CanResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override object GetValue(object component)
		{
			return property.GetValue(component, null);
		}

		public override void ResetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override void SetValue(object component, object value)
		{
			property.SetValue(component, value, null);
		}

		public override bool ShouldSerializeValue(object component)
		{
			throw new NotImplementedException();
		}

		public override Type ComponentType
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsReadOnly
		{
			get { return property.CanWrite == false;}
		}

		public override Type PropertyType
		{
			get { return property.PropertyType; }
		}
	}
}