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
	using System.Text;
	using System.Reflection;

	using Castle.MonoRail.Framework.Internal;


	public class HtmlHelper : AbstractHelper
	{
		public String LinkTo( string name, string action )
		{
			return LinkTo( name, base.Controller.Name, action );
		}

		public String LinkTo( string name, string controller, string action )
		{
			string url = base.Controller.Context.ApplicationPath;
			string extension = base.Controller.Context.UrlInfo.Extension;

			return string.Format( "<a href=\"{0}{1}/{2}.{3}\">{4}</a>", url, controller, action, extension, name );
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

		private MethodInfo GetMethod(object elem, string property)
		{
			if (elem == null) throw new ArgumentNullException("elem");
			if (property == null) return null;

			return elem.GetType().GetMethod("get_" + property, BindingFlags.Instance|BindingFlags.Public|BindingFlags.IgnoreCase);
		}
	}
}
