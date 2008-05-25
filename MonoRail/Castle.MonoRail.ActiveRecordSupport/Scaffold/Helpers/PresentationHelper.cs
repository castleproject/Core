// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport.Scaffold.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.MonoRail.Framework.Helpers;
	
	public class PresentationHelper : AbstractHelper
	{
		public String StartAlternateTR(int index, String styleClass, String altStyleClass)
		{
			return String.Format("<tr class=\"{0}\">", index % 2 == 0 ? styleClass : altStyleClass);
		}

		public String BestAlignFor(Type type)
		{
			if (type == typeof(String))
			{
				return "left";
			}

			return "center";
		}
		
		public String LinkToBack(String text, IDictionary attributes)
		{
			return String.Format( "<a href=\"javascript:history.go(-1);\" {1}>{0}</a>", 
			                      text, GetAttributes(attributes) );
		}

		public String LinkToList(ActiveRecordModel model, bool useModelName, String text, IDictionary attributes)
		{
			string targetAction = "list" + (useModelName ? model.Type.Name : "");

			return LinkTo(targetAction, text, attributes);
		}

		public String LinkToNew(ActiveRecordModel model, bool useModelName, String text, IDictionary attributes)
		{
			string targetAction = "new" + (useModelName ? model.Type.Name : "");

			return LinkTo(targetAction, text, attributes);
		}

		public String LinkToEdit(ActiveRecordModel model, bool useModelName, 
		                         String text, object key, IDictionary attributes)
		{
			string targetAction = "edit" + (useModelName ? model.Type.Name : "");

			return LinkTo(targetAction, key, text, attributes);
		}

		public String LinkToConfirm(ActiveRecordModel model, bool useModelName, String text, object key, IDictionary attributes)
		{
			string targetAction = "confirm" + (useModelName ? model.Type.Name : "");

			return LinkTo(targetAction, key, text, attributes);
		}

		public String LinkToRemove(ActiveRecordModel model, bool useModelName, String text, object key, IDictionary attributes)
		{
			string targetAction = "remove" + (useModelName ? model.Type.Name : "");

			return LinkTo(targetAction, text, attributes);
		}

		private string LinkTo(string action, string text, IDictionary attributes)
		{
			HybridDictionary dict = new HybridDictionary(true);
			dict["action"] = action;
			dict["params"] = ControllerContext.RouteMatch.Parameters;

			return UrlHelper.Link(text, dict, attributes);
		}

		private string LinkTo(string action, object key, string text, IDictionary attributes)
		{
			HybridDictionary dict = new HybridDictionary(true);
			dict["action"] = action;
			dict["id"] = key;
			ControllerContext.RouteMatch.AddNamed("id", key.ToString());
			dict["params"] = ControllerContext.RouteMatch.Parameters;

			IDictionary querystring = DictHelper.CreateN("id", key);
			dict["querystring"] = UrlHelper.BuildQueryString(querystring);

			return UrlHelper.Link(text, dict, attributes);
		}
	}
}
