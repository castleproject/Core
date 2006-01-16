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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Reflection;

	public class FormHelper : AbstractHelper
	{
		private static readonly BindingFlags flags = BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;

		public String TextFieldValue(Object target, String property, object value)
		{
			return TextFieldValue(target, property, value, null);
		}

		public String TextFieldValue(Object target, String property, object value, IDictionary attributes)
		{
			return CreateInputElement("text", String.Format("{0}_{1}", target.GetType().Name, property), 
				String.Format("{0}.{1}", target.GetType().Name, property), 
				value, attributes);
		}

		public String TextField(Object target, String property)
		{
			return TextField(target, property, null);
		}

		public String TextField(Object target, String property, IDictionary attributes)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(property, flags);

			return CreateInputElement("text", String.Format("{0}_{1}", target.GetType().Name, property), 
				String.Format("{0}.{1}", target.GetType().Name, property), 
				propertyInfo.GetValue(target, null), attributes);
		}

		protected String CreateInputElement(String type, String id, String name, Object value, IDictionary attributes)
		{
			value = value == null ? "" : value;

			return String.Format("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" {4}/>", 
				type, id, name, value, GetAttributes(attributes));
		}
	}
}
