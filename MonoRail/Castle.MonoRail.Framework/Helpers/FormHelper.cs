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

	using Castle.Components.Binder;

	public enum RequestContext
	{
		All,
		PropertyBag,
		Flash,
		Session,
		Request
	}

	/// <summary>
	/// Currently being evaluated
	/// </summary>
	public class FormHelper : AbstractHelper
	{
		private static readonly BindingFlags PropertyFlags = BindingFlags.GetProperty|BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;
		private static readonly BindingFlags FieldFlags = BindingFlags.GetField|BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;

		#region TextFieldValue

		public String TextFieldValue(String target, object value)
		{
			return TextFieldValue(target, value, null);
		}

		public String TextFieldValue(String target, object value, IDictionary attributes)
		{
			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region TextField

		public String TextField(String target)
		{
			return TextField(target, null);
		}

		public String TextField(String target, IDictionary attributes)
		{
			object value = ObtainValue(target);

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region TextFieldFormat

		public String TextFieldFormat(String target, String formatString)
		{
			return TextFieldFormat(target, formatString, null);
		}

		public String TextFieldFormat(String target, String formatString, IDictionary attributes)
		{
			object value = ObtainValue(target);

			if (value != null)
			{
				IFormattable formattable = value as IFormattable;

				if (formattable != null)
				{
					value = formattable.ToString(formatString, null);
				}
			}

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region LabelFor

		public String LabelFor(String target, String label)
		{
			String id = target.Replace('.', '_');

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

		public String HiddenField(String target)
		{
			object value = ObtainValue(target);

			return CreateInputElement("hidden", target, value, null);
		}

		#endregion

		#region CheckboxField

		public String CheckboxField(String target)
		{
			return CheckboxField(target, null);
		}

		public String CheckboxField(String target, IDictionary attributes)
		{
			object value = ObtainValue(target);

			bool isChecked = ((value != null && value is bool && ((bool)value) == true) || 
				(!(value is bool) && (value != null)));

			if (isChecked)
			{
				if (attributes == null)
				{
					attributes = new HybridDictionary(true);
				}

				attributes["checked"] = String.Empty;
			}

			return CreateInputElement("checkbox", target, value is bool ? "true" : value, attributes);
		}

		#endregion

		public String Select(String target, IEnumerable dataSource)
		{
			return Select(target, dataSource, null);
		}

		public String Select(String target, IEnumerable dataSource, IDictionary attributes)
		{
			String id = target.Replace('.', '_');

			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			object selectedValue = ObtainValue(target);

			String firstOption = null; 
			String valueProperty = null; 
			String textProperty = null;

			if (attributes != null)
			{
				firstOption = (String) attributes["firstoption"];
				attributes.Remove("firstoption");
				
				valueProperty = (String) attributes["value"];
				attributes.Remove("value");
				
				textProperty = (String) attributes["text"];
				attributes.Remove("text");
			}

			writer.WriteBeginTag("select");
			writer.WriteAttribute("id", id);
			writer.WriteAttribute("name", target);
			writer.Write(" ");
			writer.Write(GetAttributes(attributes));
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			if (firstOption != null)
			{
				writer.WriteBeginTag("option");
				writer.WriteAttribute("value", "0");
				writer.Write(HtmlTextWriter.TagRightChar);
				writer.Write(firstOption);
				writer.WriteEndTag("option");
				writer.WriteLine();
			}

			if (dataSource != null)
			{
				IEnumerator enumerator = dataSource.GetEnumerator(); 

				if (enumerator.MoveNext())
				{
					object guidanceElem = enumerator.Current;

					bool isMultiple = (selectedValue != null && selectedValue.GetType().IsArray);
				
					MethodInfo valueMethodInfo = GetMethod(guidanceElem, valueProperty);
					MethodInfo textMethodInfo = null;
				
					if (textProperty != null)
					{
						textMethodInfo = GetMethod(guidanceElem, textProperty);
					}

					foreach(object elem in dataSource)
					{
						object value = null;

						if (valueMethodInfo != null) 
							value = valueMethodInfo.Invoke(elem, null);

						object text = textMethodInfo != null ? 
							textMethodInfo.Invoke(elem, null) : elem.ToString();

						writer.WriteBeginTag("option");
					
						bool selected = false;

						if (value != null)
						{
							selected = IsSelected(value, selectedValue, isMultiple);

							writer.WriteAttribute("value", value.ToString());
						}
						else
						{
							selected = IsSelected(text, selectedValue, isMultiple);
						}

						if (selected) writer.Write(" selected");
						writer.Write(HtmlTextWriter.TagRightChar);
						writer.Write(text);
						writer.WriteEndTag("option");
						writer.WriteLine();
					}
				}
			}

			writer.WriteEndTag("select");

			return sbWriter.ToString();
		}

		protected String CreateInputElement(String type, String target, Object value, IDictionary attributes)
		{
			value = value == null ? "" : value;

			String id = target.Replace('.', '_');

			return String.Format("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" {4}/>", 
				type, id, target, value, GetAttributes(attributes));
		}

		private object ObtainValue(String target)
		{
			return ObtainValue(RequestContext.All, target);
		}

		private object ObtainValue(RequestContext context, String target)
		{
			String[] pieces = target.Split(new char[] {'.'}, 2);

			String root = pieces[0];

			Object rootInstance = ObtainRootInstance(context, root);

			if (rootInstance == null)
			{
				return null;
			}

			return QueryProperty(rootInstance, pieces[1]);
		}

		private object QueryProperty(object rootInstance, string path)
		{
			String[] properties = path.Split('.');

			object instance = rootInstance;

			foreach(String property in properties)
			{
				Type instanceType = instance.GetType();

				PropertyInfo propertyInfo = instanceType.GetProperty(property, PropertyFlags);

				if (propertyInfo == null)
				{
					FieldInfo fieldInfo = instanceType.GetField(property, FieldFlags);

					if (fieldInfo == null)
					{
						throw new BindingException("No public property or field '{0}' found on type '{1}'", 
							property, instanceType.FullName);
					}

					instance = fieldInfo.GetValue(instance);
				}
				else
				{
					if (!propertyInfo.CanRead)
					{
						throw new BindingException("Property '{0}' for type '{1}' can not be read", 
							propertyInfo.Name, instanceType.FullName);
					}
					if (propertyInfo.GetIndexParameters().Length != 0)
					{
						throw new BindingException("Property '{0}' for type '{1}' has indexes, which is not supported", 
							propertyInfo.Name, instanceType.FullName);
					}

					instance = propertyInfo.GetValue(instance, null);
				}
			}

			return instance;
		}

		private object ObtainRootInstance(RequestContext context, String target)
		{
			object rootInstance = null;

			if (context == RequestContext.All || context == RequestContext.PropertyBag)
			{
				rootInstance = Controller.PropertyBag[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Flash))
			{
				rootInstance = Controller.Context.Flash[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Session))
			{
				rootInstance = Controller.Context.Session[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Request))
			{
				rootInstance = Controller.Context.UnderlyingContext.Items[target];
			}

			return rootInstance;
		}

		/// <summary>
		/// Determines whether the specified value is selected.
		/// </summary>
		/// <param name="value">Value to be tested.</param>
		/// <param name="selectedValue">Selected value.</param>
		/// <param name="isMultiple"><see langword="true"/> if <paramref name="selectedValue"/> is
		/// <see cref="Type.IsArray"/>; otherwise, <see langword="false"/>.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified <paramref name="value"/> is selected; otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>Specified <paramref name="value"/> is selected if it <see cref="Object.Equals"/>
		/// to the <paramref name="selectedValue"/>. Or if <paramref name="selectedValue"/> is an
		/// array <paramref name="value"/> is selected if <see cref="Array.IndexOf"/> can find it
		/// in <paramref name="selectedValue"/>.</remarks>
		private bool IsSelected(object value, object selectedValue, bool isMultiple)
		{
			if (!isMultiple)
			{
				return value.Equals(selectedValue);
			}
			else
			{
				return Array.IndexOf( (Array) selectedValue, value ) != -1;
			}
		}
		
		/// <summary>
		/// Gets the property get method.
		/// </summary>
		/// <param name="elem">Object specifying the type for which to get the method.</param>
		/// <param name="property">Property name.</param>
		/// <returns><see cref="MethodInfo"/> to be used to retrieve the property value.
		/// If <paramref name="property"/> is <c>null</c> <c>null</c> is returned.</returns>
		/// <remarks>This method is used to get the <see cref="MethodInfo"/> to retrieve
		/// specified property from the specified type.</remarks>
		/// <exception cref="ArgumentNullException">Thrown is <paramref name="elem"/> is <c>null</c>.</exception>
		private MethodInfo GetMethod(object elem, String property)
		{
			if (elem == null) throw new ArgumentNullException("elem");
			if (property == null) return null;

			return elem.GetType().GetMethod("get_" + property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
		}
	}
}
