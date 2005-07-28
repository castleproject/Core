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

		/// <summary>
		/// Creates an anchor (link)
		/// <code>
		/// &lt;a href="action" &gt;name&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public String LinkTo(String name, String action)
		{
			return LinkTo(name, Controller.Name, action);
		}

		public String LinkTo(String name, String controller, String action)
		{
			String url = Controller.Context.ApplicationPath;
			String extension = Controller.Context.UrlInfo.Extension;

			return String.Format("<a href=\"{0}/{1}/{2}.{3}\">{4}</a>", 
				url, controller, action, extension, name);
		}

		public String LinkTo(String name, String controller, String action, object id)
		{
			String url = Controller.Context.ApplicationPath;
			String extension = Controller.Context.UrlInfo.Extension;

			return String.Format("<a href=\"{0}/{1}/{2}.{3}?id={4}\">{5}</a>", 
				url, controller, action, extension, id, name);
		}

		public String MapToVirtual(String target)
		{
			String appPath = Controller.Context.ApplicationPath.EndsWith("/") ?
				Controller.Context.ApplicationPath : 
				Controller.Context.ApplicationPath + "/";

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

		/// <summary>
		/// Creates a label for the element indicated with 
		/// the specified text
		/// <code>
		/// &lt;label for="forId" &gt;name&lt;/label&gt;
		/// </code>
		/// </summary>
		/// <param name="forId"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public String LabelFor(String forId, String label)
		{
			return LabelFor(forId, label, null);
		}

		/// <summary>
		/// Creates a label for the element indicated with 
		/// the specified text
		/// <code>
		/// &lt;label for="forId" &gt;name&lt;/label&gt;
		/// </code>
		/// </summary>
		/// <param name="forId"></param>
		/// <param name="label"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Creates three inputs for day, month and year
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public String DateTime(String name, DateTime value)
		{
			return DateTime(name, value, null);
		}

		/// <summary>
		/// Creates three inputs for day, month and year
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Creates a text area element 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="cols"></param>
		/// <param name="rows"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public String TextArea(String name, int cols, int rows, String value)
		{
			return String.Format("<textarea id=\"{0}\" name=\"{0}\" cols=\"{1}\" rows=\"{2}\">{3}</textarea>",
			                     name, cols, rows, value);
		}

		/// <summary>
		/// Creates an input element of text type
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public String InputText(String name, String value)
		{
			return InputText(name, value, null);
		}

		/// <summary>
		/// Creates an input element of text type
		/// </summary>
		public String InputText(String name, String value, int size, int maxlength)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" />",
			                     name, value, size, maxlength);
		}

		/// <summary>
		/// Creates an input element of text type
		/// </summary>
		public String InputText(String name, String value, int size, int maxlength, IDictionary attributes)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" {4}/>",
			                     name, value, size, maxlength, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates an input element of text type
		/// </summary>
		public String InputText(String name, String value, String id)
		{
			if (id == null) id = name;

			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />", name, id, value);
		}

		/// <summary>
		/// Creates an input hidden element
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public String InputHidden(String name, String value)
		{
			return String.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", name, value);
		}

		/// <summary>
		/// Creates a submit button
		/// <code>
		/// &lt;input type="submit" value="value" /&gt;
		/// </code>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public String SubmitButton(String value)
		{
			return SubmitButton(value, null);
		}

		/// <summary>
		/// Creates a submit button
		/// <code>
		/// &lt;input type="submit" value="value" /&gt;
		/// </code>
		/// </summary>
		public String SubmitButton(String value, IDictionary attributes)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("input");
			writer.WriteAttribute("type", "submit");
			writer.WriteAttribute("value", value);
			writer.Write(" ");
			writer.Write( GetAttributes(attributes) );
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Starts a select element
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public String Select(String name)
		{
			return String.Format("<select name=\"{0}\" id=\"{0}\">", name);
		}

		/// <summary>
		/// Starts a select element
		/// </summary>
		public String Select(String name, IDictionary attributes)
		{
			String htmlAttrs = GetAttributes(attributes);

			return String.Format("<select name=\"{0}\" id=\"{0}\" {1}>", name, htmlAttrs);
		}

		/// <summary>
		/// Ends a select element
		/// </summary>
		public String EndSelect()
		{
			return String.Format("</select>");
		}

		/// <summary>
		/// Create options elements from an array. Marks the
		/// option that matches the <c>selected</c> argument (if provided)
		/// </summary>
		/// <param name="elems"></param>
		/// <param name="selected"></param>
		/// <returns></returns>
		public String CreateOptionsFromPrimitiveArray(Array elems, String selected)
		{
			if (elems.GetLength(0) == 0) return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach (object elem in elems)
			{
				sb.AppendFormat("\t<option{0}>{1}</option>\r\n", 
					elem.Equals(selected) ? " selected" : "", elem);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Create options elements from an array with the text provided by 
		/// the <c>textProperty</c> and the value provided by the <c>valueProperty</c>
		/// </summary>
		/// <param name="elems"></param>
		/// <param name="textProperty"></param>
		/// <param name="valueProperty"></param>
		/// <returns></returns>
		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty);
		}

		/// <summary>
		/// Create options elements from an array with the text provided by 
		/// the <c>textProperty</c> and the value provided by the <c>valueProperty</c>
		/// Marks the option that matches the <c>selected</c> argument (if provided)
		/// </summary>
		/// <param name="elems"></param>
		/// <param name="textProperty"></param>
		/// <param name="valueProperty"></param>
		/// <param name="selectedValue"></param>
		/// <returns></returns>
		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty, object selectedValue)
		{
			return CreateOptions(elems, textProperty, valueProperty, selectedValue);
		}

		/// <summary>
		/// Create options elements from an <see cref="ICollection"/> with the text provided by 
		/// the <c>textProperty</c> and the value provided by the <c>valueProperty</c>
		/// </summary>
		/// <param name="elems"></param>
		/// <param name="textProperty"></param>
		/// <param name="valueProperty"></param>
		/// <returns></returns>
		public String CreateOptions(ICollection elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty, null);
		}

		/// <summary>
		/// Create options elements from an <see cref="ICollection"/> with the text provided by 
		/// the <c>textProperty</c> and the value provided by the <c>valueProperty</c>
		/// Marks the option that matches the <c>selected</c> argument (if provided)
		/// </summary>
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

		/// <summary>
		/// Builds an unordered list with the elements on the specified <see cref="ICollection"/>
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static String BuildUnorderedList(ICollection elements)
		{
			return BuildUnorderedList(elements, null, null);
		}
		/// <summary>
		/// Builds an ordered list with the elements on the specified <see cref="ICollection"/>
		/// </summary>
		public static String BuildOrderedList(ICollection elements)
		{
			return BuildOrderedList(elements, null, null);
		}

		/// <summary>
		/// Builds an ordered list with the elements on the specified <see cref="ICollection"/>
		/// </summary>
		public static String BuildOrderedList(ICollection elements, String styleClass, String itemClass)
		{
			return BuildList("ol", elements, styleClass, itemClass);
		}

		/// <summary>
		/// Builds an unordered list with the elements on the specified <see cref="ICollection"/>
		/// </summary>
		public static String BuildUnorderedList(ICollection elements, String styleClass, String itemClass)
		{
			return BuildList("ul", elements, styleClass, itemClass);
		}

		private static String BuildList(String tag, ICollection elements, String styleClass, String itemClass)
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

			foreach (object item in elements)
			{
				if (item == null) continue;
				
				writer.WriteLine(BuildListItem(item.ToString(), itemClass));
			}

			writer.WriteEndTag(tag);
			writer.WriteLine();

			return sbWriter.ToString();
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