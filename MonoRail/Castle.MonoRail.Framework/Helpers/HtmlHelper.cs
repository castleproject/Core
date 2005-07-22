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
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Web.UI;

	/// <summary>
	/// Provides usefull common methods to output html
	/// </summary>
	public class HtmlHelper : AbstractHelper
	{
		/// <summary>
		/// Creates a fieldset tag with a legend
		/// <code>
		/// &lt;fieldset&gt; &lt;legend&gt; legend &lt;/legend&gt;
		/// </code>
		/// </summary>
		/// <param name="legend">Legend that should be used within the fieldset</param>
		/// <returns></returns>
		public String FieldSet(String legend)
		{
			return String.Format("<fieldset><legend>{0}</legend>", legend);
		}

		/// <summary>
		/// Creates a closing fieldset tag
		/// <code>
		/// &lt;/fieldset&gt; 
		/// </code>
		/// </summary>
		/// <returns></returns>
		public String EndFieldSet()
		{
			return "</fieldset>";
		}

		/// <summary>
		/// Creates a form tag 
		/// <code>
		/// &lt;form method="post" action="action"&gt;
		/// </code>
		/// </summary>
		/// <param name="action">The target action</param>
		/// <returns></returns>
		public String Form(String action)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("form");
			writer.WriteAttribute("method", "post");
			writer.WriteAttribute("action", action);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Creates a form tag 
		/// <code>
		/// &lt;form method="post" method="method" id="id" action="action"&gt;
		/// </code>
		/// </summary>
		/// <param name="action">The target action</param>
		/// <param name="id">the form html id</param>
		/// <param name="method">form method (get, post, etc)</param>
		/// <returns></returns>
		public String Form(String action, String id, String method)
		{
			return Form(action, id, method, null);
		}

		/// <summary>
		/// Creates a form tag 
		/// <code>
		/// &lt;form method="post" method="method" id="id" action="action" onsubmit="onSubmit" &gt;
		/// </code>
		/// </summary>
		/// <param name="action">The target action</param>
		/// <param name="id">the form html id</param>
		/// <param name="method">form method (get, post, etc)</param>
		/// <param name="onSubmit">a javascript inline code to be invoked upon form submission</param>
		/// <returns></returns>
		public String Form(String action, String id, String method, String onSubmit)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("form");
			writer.WriteAttribute("method", method);
			writer.WriteAttribute("action", action);
			writer.WriteAttribute("id", id);
			if (onSubmit != null)
				writer.WriteAttribute("onsubmit", onSubmit);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Creates a form closing tag
		/// <code>
		/// &lt;/form&gt;
		/// </code>
		/// </summary>
		/// <returns></returns>
		public String EndForm()
		{
			return "</form>";
		}

		public String CreateSubmit(String name)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("input");
			writer.WriteAttribute("type", "submit");
			writer.WriteAttribute("value", name);
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		public String LinkTo(String name, String action)
		{
			return LinkTo(name, Controller.Name, action);
		}

		public String MapToVirtual(String target)
		{
			String appPath = this.Controller.Context.ApplicationPath.EndsWith("/") ?
				this.Controller.Context.ApplicationPath : this.Controller.Context.ApplicationPath + "/";

			String targetPath = target.StartsWith("/") ? target.Substring(1) : target;
			return String.Concat(appPath, targetPath);
		}

		public String LinkToAbsolute(String name, String action)
		{
			return LinkToAbsolute(name, base.Controller.Name, action);
		}

		public String LinkToAbsolute(String name, String controller, String action)
		{
			String extension = base.Controller.Context.UrlInfo.Extension;
			String url = this.MapToVirtual(String.Format("/{0}/{1}.{2}", controller, action, extension));

			return String.Format("<a href=\"{0}\">{1}</a>", url, name);
		}

		public String LabelFor(String forId, String label)
		{
			return LabelFor(forId, label, null);
		}

		public String LabelFor(String forId, String label, IDictionary attributes)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("label");
			writer.Write( " " );
			writer.Write( GetAttributes(attributes) );
			writer.WriteAttribute("for", forId);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.Write(label);
			writer.WriteEndTag("label");
			writer.WriteLine();

			return sbWriter.ToString();
		}

		public String DateTime(String name, DateTime value)
		{
			return DateTime(name, value, null);
		}

		public String DateTime(String name, DateTime value, IDictionary attributes)
		{
			String[] days = new String[31];
			int index = 0;
			for (int i = 1; i < 32; i++)
				days[index++] = i.ToString();

			String[] months = new String[12];
			index = 0;
			for (int i = 1; i < 13; i++)
				months[index++] = i.ToString();

			String[] years = new String[100];
			index = 0;
			for (int i = 1930; i < 2030; i++)
				years[index++] = i.ToString();

			StringBuilder sb = new StringBuilder();

			sb.Append(Select(name + "day", attributes));
			sb.Append(CreateOptionsFromPrimitiveArray(days, value.Day.ToString()));
			sb.Append(EndSelect());
			sb.Append(' ');
			sb.Append(Select(name + "month", attributes));
			sb.Append(CreateOptionsFromPrimitiveArray(months, value.Month.ToString()));
			sb.Append(EndSelect());
			sb.Append(' ');
			sb.Append(Select(name + "year", attributes));
			sb.Append(CreateOptionsFromPrimitiveArray(years, value.Year.ToString()));
			sb.Append(EndSelect());

			return sb.ToString();
		}

		public String TextArea(String name, int cols, int rows, String value)
		{
			return String.Format("<textarea id=\"{0}\" name=\"{0}\" cols=\"{1}\" rows=\"{2}\">{3}</textarea>",
			                     name, cols, rows, value);
		}

		public String InputText(String name, String value)
		{
			return InputText(name, value, null);
		}

		public String InputText(String name, String value, int size, int maxlength)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" />",
			                     name, value, size, maxlength);
		}

		public String InputText(String name, String value, int size, int maxlength, IDictionary attributes)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" {4}/>",
			                     name, value, size, maxlength, GetAttributes(attributes));
		}

		public String InputText(String name, String value, String id)
		{
			if (id == null) id = name;

			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />", name, id, value);
		}

		public String InputHidden(String name, String value)
		{
			return String.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", name, value);
		}

		public String SubmitButton(String value)
		{
			return String.Format("<input type=\"submit\" value=\"{0}\" />", value);
		}

		public String SubmitButton(String value, IDictionary attributes)
		{
			return String.Format("<input type=\"submit\" value=\"{0}\" {1}/>", value, GetAttributes(attributes));
		}

		public String LinkTo(String name, String controller, String action)
		{
			String url = base.Controller.Context.ApplicationPath;
			String extension = base.Controller.Context.UrlInfo.Extension;

			return String.Format("<a href=\"{0}/{1}/{2}.{3}\">{4}</a>", url, controller, action, extension, name);
		}

		public String LinkTo(String name, String controller, String action, object id)
		{
			String url = base.Controller.Context.ApplicationPath;
			String extension = base.Controller.Context.UrlInfo.Extension;

			return String.Format("<a href=\"{0}/{1}/{2}.{3}?id={4}\">{5}</a>", url, controller, action, extension, id, name);
		}

		public String Select(String name)
		{
			return String.Format("<select name=\"{0}\" id=\"{0}\">", name);
		}

		public String Select(String name, IDictionary attributes)
		{
			String htmlAttrs = GetAttributes(attributes);

			return String.Format("<select name=\"{0}\" id=\"{0}\" {1}>", name, htmlAttrs);
		}

		public String EndSelect()
		{
			return String.Format("</select>");
		}

		public String CreateOptionsFromPrimitiveArray(Array elems, String selected)
		{
			if (elems.GetLength(0) == 0) return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach (object elem in elems)
			{
				sb.AppendFormat("\t<option{0}>{1}</option>\r\n", elem.Equals(selected) ? " selected" : "", elem);
			}

			return sb.ToString();
		}

		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty);
		}

		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty, object selectedValue)
		{
			return CreateOptions(elems, textProperty, valueProperty, selectedValue);
		}

		public String CreateOptions(ICollection elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty, null);
		}

		public String CreateOptions(ICollection elems, String textProperty, String valueProperty, object selectedValue)
		{
			if (elems == null) throw new ArgumentNullException("elems");
			
			if (elems.Count == 0) return String.Empty;

			IEnumerator enumerator = elems.GetEnumerator(); 
			enumerator.MoveNext(); 
			object guidanceElem = enumerator.Current;

			bool isMultiple = (selectedValue != null && selectedValue.GetType().IsArray);
			
			MethodInfo valueMethodInfo = GetMethod(guidanceElem, valueProperty);
			MethodInfo textMethodInfo = null;
			
			if (textProperty != null)
			{
				textMethodInfo = GetMethod(guidanceElem, textProperty);
			}

			StringBuilder sb = new StringBuilder();

			foreach (object elem in elems)
			{
				if (elem == null) continue;

				object value = null;

				if (valueMethodInfo != null) value = valueMethodInfo.Invoke(elem, new object[0]);

				object text = textMethodInfo != null ? textMethodInfo.Invoke(elem, new object[0]) : elem.ToString();

				if (value != null)
				{
					bool selected = IsSelected(value, selectedValue, isMultiple);

					sb.AppendFormat("\t<option value=\"{1}\" {0}>{2}</option>\r\n",
					                selected ? "selected" : "", value, text);
				}
				else
				{
					bool selected = IsSelected(text, selectedValue, isMultiple);

					sb.AppendFormat("\t<option {0}>{1}</option>\r\n",
					                selected ? "selected" : "", text);
				}
			}

			return sb.ToString();
		}

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

		public static String BuildUnorderedList(String[] array)
		{
			return BuildUnorderedList(array, null, null);
		}

		public static String BuildOrderedList(String[] array)
		{
			return BuildOrderedList(array, null, null);
		}

		public static String BuildOrderedList(String[] array, String styleClass, String itemClass)
		{
			return BuildList("ol", array, styleClass, itemClass);
		}

		public static String BuildUnorderedList(String[] array, String styleClass, String itemClass)
		{
			return BuildList("ul", array, styleClass, itemClass);
		}

		private static String BuildList(String tag, String[] array, String styleClass, String itemClass)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag(tag);

			if (styleClass != null)
			{
				writer.WriteAttribute("class", styleClass);
			}

			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			foreach (String item in array)
			{
				writer.WriteLine(BuildListItem(item, itemClass));
			}

			writer.WriteEndTag(tag);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		private static String BuildListItem(String item, String itemClass)
		{
			if (itemClass == null)
			{
				return String.Format("<li>{0}</li>", item);
			}
			else
			{
				return String.Format("<li class=\"{0}\">{1}</li>", itemClass, item);
			}
		}

		private MethodInfo GetMethod(object elem, String property)
		{
			if (elem == null) throw new ArgumentNullException("elem");
			if (property == null) return null;

			return elem.GetType().GetMethod("get_" + property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
		}
	}
}