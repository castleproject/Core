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
	using System.IO;
	using System.Text;
	using System.Web.UI;
	using System.Reflection;

	/// <summary>
	/// Provides usefull common methods to output html
	/// </summary>
	public class HtmlHelper : AbstractHelper
	{
		public String FieldSet(String legend)
		{
			return String.Format( "<fieldset><legend>{0}</legend>", legend );
		}

		public String EndFieldSet()
		{
			return "</fieldset>";
		}

		public String Form( String action )
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter( sbWriter );

			writer.WriteBeginTag( "form" );
			writer.WriteAttribute( "method", "post" );
			writer.WriteAttribute( "action", action );
			writer.Write( HtmlTextWriter.TagRightChar );
			writer.WriteLine();

			return sbWriter.ToString();
		}

		public String EndForm( )
		{
			return "</form>";
		}

		public String CreateSubmit(String name)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter( sbWriter );

			writer.WriteBeginTag( "input" );
			writer.WriteAttribute( "type", "submit" );
			writer.WriteAttribute( "value", name );
			writer.Write( HtmlTextWriter.SelfClosingTagEnd );
			writer.WriteLine();

			return sbWriter.ToString();
		}

		public String LinkTo( String name, String action )
		{
			return LinkTo( name, Controller.Name, action );
		}

		public String LabelFor( String forId, String label )
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter( sbWriter );

			writer.WriteBeginTag( "label" );
			writer.WriteAttribute( "for", forId );
			writer.Write( HtmlTextWriter.TagRightChar );
			writer.Write(label);
			writer.WriteEndTag( "label" );
			writer.WriteLine();

			return sbWriter.ToString();
		}

		public String DateTime(String name, DateTime value)
		{
			String[] days = new String[31]; int index = 0;
			for(int i=1; i < 32; i++)
				days[index++] = i.ToString();
			
			String[] months = new String[12]; index = 0;
			for(int i=1; i < 13; i++)
				months[index++] = i.ToString();
			
			String[] years = new String[100]; index = 0;
			for(int i=1930; i < 2030; i++)
				years[index++] = i.ToString();

			StringBuilder sb = new StringBuilder();

			sb.Append( Select(name+"day") );
			sb.Append( CreateOptionsFromPrimitiveArray(days, value.Day.ToString()) );
			sb.Append( EndSelect() );
			sb.Append( ' ' );
			sb.Append( Select(name+"month") );
			sb.Append( CreateOptionsFromPrimitiveArray(months, value.Month.ToString()) );
			sb.Append( EndSelect() );
			sb.Append( ' ' );
			sb.Append( Select(name+"year") );
			sb.Append( CreateOptionsFromPrimitiveArray(years, value.Year.ToString()) );
			sb.Append( EndSelect() );

			return sb.ToString();
		}

		public String TextArea(String name, int cols, int rows, String value)
		{
			return String.Format("<textarea id=\"{0}\" name=\"{0}\" cols=\"{1}\" rows=\"{2}\">{3}</textarea>", 
				name, cols, rows, value);
		}

		public String InputText( String name, String value )
		{
			return InputText( name, value, null );
		}

		public String InputText( String name, String value, int size, int maxlength )
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" />", 
				name, value, size, maxlength);
		}

		public String InputText( String name, String value, String id )
		{
			if (id == null) id = name;

			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />", name, id, value);
		}

		public String SubmitButton( String value )
		{
			return String.Format("<input type=\"submit\" value=\"{0}\" />", value);
		}

		public String LinkTo( String name, String controller, String action )
		{
			String url = base.Controller.Context.ApplicationPath;
			String extension = base.Controller.Context.UrlInfo.Extension;

			return String.Format( "<a href=\"{0}{1}/{2}.{3}\">{4}</a>", url, controller, action, extension, name );
		}

		public String Select( String name )
		{
			return String.Format("<select name=\"{0}\" id=\"{0}\">", name);
		}

		public String EndSelect()
		{
			return String.Format("</select>");
		}

		public String CreateOptionsFromPrimitiveArray(Array elems, String selected)
		{
			if (elems.GetLength(0) == 0) return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach(object elem in elems)
			{
				sb.AppendFormat("\t<option{0}>{1}</option>\r\n", elem.Equals(selected) ? " selected" : "", elem);
			}

			return sb.ToString();
		}

		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty)
		{
			return CreateOptionsFromArray(elems, textProperty, valueProperty, null);
		}

		public String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty, object selected)
		{
			if (elems == null) throw new ArgumentNullException("elems");
			if (textProperty == null) throw new ArgumentNullException("textProperty");

			if (elems.GetLength(0) == 0) return String.Empty;

			MethodInfo valueMethodInfo = GetMethod(elems.GetValue(0), valueProperty);
			MethodInfo textMethodInfo = GetMethod(elems.GetValue(0), textProperty);

			if (textMethodInfo == null)
			{
				String message = String.Format("Specified {0] could not be identified as a valid readable property", textProperty);
				throw new ArgumentException(message);
			}

			StringBuilder sb = new StringBuilder();

			foreach(object elem in elems)
			{
				object value = null;
				
				if (valueMethodInfo != null) value = valueMethodInfo.Invoke(elem, new object[0]);
				
				object text = textMethodInfo.Invoke(elem, new object[0]);

				if (value != null)
				{
					sb.AppendFormat("\t<option {0} value=\"{1}\">{2}</option>\r\n", 
						value.Equals(selected) ? "selected" : "", value, text);
				}
				else
				{
					sb.AppendFormat("\t<option {0}>{1}</option>\r\n", 
						text.Equals(selected) ? "selected" : "", text);
				}
			}

			return sb.ToString();
		}

		public static String BuildUnorderedList( String[] array )
		{
			return BuildUnorderedList( array, null, null );
		}

		public static String BuildOrderedList( String[] array )
		{
			return BuildOrderedList( array, null, null );
		}

		public static String BuildOrderedList( String[] array, String styleClass, String itemClass )
		{
			return BuildList( "ol", array, styleClass, itemClass );
		}

		public static String BuildUnorderedList( String[] array, String styleClass, String itemClass )
		{
			return BuildList( "ul", array, styleClass, itemClass );
		}

		private static String BuildList(String tag, String[] array, String styleClass, String itemClass)
		{
			StringWriter sbWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter( sbWriter );

			writer.WriteBeginTag( tag );

			if (styleClass != null)
			{
				writer.WriteAttribute("class", styleClass);
			}

			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			foreach(String item in array)
			{
				writer.WriteLine( BuildListItem( item, itemClass ) );
			}

			writer.WriteEndTag( tag );
			writer.WriteLine();

			return sbWriter.ToString();
		}

		private static String BuildListItem(String item, String itemClass)
		{
			if (itemClass == null)
			{
				return String.Format( "<li>{0}</li>", item );
			}
			else
			{
				return String.Format( "<li class=\"{0}\">{1}</li>", itemClass, item );
			}
		}

		private MethodInfo GetMethod(object elem, String property)
		{
			if (elem == null) throw new ArgumentNullException("elem");
			if (property == null) return null;

			return elem.GetType().GetMethod("get_" + property, BindingFlags.Instance|BindingFlags.Public|BindingFlags.IgnoreCase);
		}
	}
}
