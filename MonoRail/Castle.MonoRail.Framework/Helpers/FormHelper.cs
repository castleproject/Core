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
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Web.UI;

	/// <summary>
	/// Currently being evaluated
	/// </summary>
	public class FormHelper : AbstractHelper
	{
		private static readonly BindingFlags flags = BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;

		#region TextFieldValue

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

		#endregion

		#region TextField

		public String TextField(Object target, String property)
		{
			return TextField(target, property, null);
		}

		public String TextField(Object target, String property, IDictionary attributes)
		{
			object value = ObtainValue(target, property);

			return CreateInputElement("text", String.Format("{0}_{1}", target.GetType().Name, property), 
				String.Format("{0}.{1}", target.GetType().Name, property), 
				value, attributes);
		}

		#endregion

		#region LabelFor

		public String LabelFor(Object target, String property, String label)
		{
			String id = String.Format("{0}_{1}", target.GetType().Name, property);

			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("label");
			writer.WriteAttribute("for", id);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.Write(label);
			writer.WriteEndTag("label");

			return sbWriter.ToString();
		}

		#endregion

		#region HiddenField

		public String HiddenField(Object target, String property)
		{
			object value = ObtainValue(target, property);

			return CreateInputElement("hidden", String.Format("{0}_{1}", target.GetType().Name, property), 
				String.Format("{0}.{1}", target.GetType().Name, property), 
				value, null);
		}

		#endregion

		#region CheckboxField

		public String CheckboxField(Object target, String property)
		{
			return CheckboxField(target, property, null);
		}

		public String CheckboxField(Object target, String property, IDictionary attributes)
		{
			object value = ObtainValue(target, property);

			bool isChecked = ((value != null && value is bool && ((bool)value) == true) || (!(value is bool) && (value != null)));

			if (isChecked)
			{
				if (attributes == null) attributes = new HybridDictionary(true);

				attributes["checked"] = String.Empty;
			}

			return CreateInputElement("checkbox", String.Format("{0}_{1}", target.GetType().Name, property), 
				String.Format("{0}.{1}", target.GetType().Name, property), 
				value is bool ? "true" : value, attributes);
		}

		#endregion

		protected String CreateInputElement(String type, String id, String name, Object value, IDictionary attributes)
		{
			value = value == null ? "" : value;

			return String.Format("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" {4}/>", 
				type, id, name, value, GetAttributes(attributes));
		}

		private static object ObtainValue(object target, string property)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(property, flags);
			return propertyInfo.GetValue(target, null);
		}
	}
}
