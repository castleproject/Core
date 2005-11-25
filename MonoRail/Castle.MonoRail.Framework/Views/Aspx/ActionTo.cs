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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web.UI;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Redirects the post for an especified Action, Controller and Area.
	/// </summary>
	public class ActionTo : Control
	{
		private const String scriptTag = @"<script language='javascript'>{0}</script>";

		private String _controller;
		private String _action;
		private String _area;
		private String _formID;
		private String _extension;

		public String Area
		{
			get { return _area; }
			set { _area = value; }
		}

		public String Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public String Action
		{
			get { return _action; }
			set { _action = value; }
		}

		/// <summary>
		/// The Form that will be redirected.
		/// </summary>
		/// <value>A string represeting the FormID.</value>
		public String FormID
		{
			get { return _formID; }
			set { _formID = value; }
		}

		public override void DataBind()
		{
			base.DataBind();

			UrlInfo urlInfo = UrlTokenizer.ExtractInfo(Context.Request.Path, Context.Request.ApplicationPath);

			_extension = urlInfo.Extension;

			if (Controller == null || Controller.Length == 0)
			{
				Controller = urlInfo.Controller;
			}

			if (Action == null || Action.Length == 0)
			{
				Action = urlInfo.Action;
			}

			if (Area == null || Area.Length == 0)
			{
				Area = urlInfo.Area;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			DataBind();

			String redirectFunction = GetRedirectFunction();

			Page.RegisterStartupScript("rails.actionTo", String.Format(scriptTag, redirectFunction));
		}

		private  String GetRedirectFunction()
		{
			String url;

			if (Area == null || Area.Length == 0)
			{
				url = UrlInfo.CreateAbsoluteRailsUrl(Context.Request.Path, Controller, Action, _extension);
			}
			else
			{
				url = UrlInfo.CreateAbsoluteRailsUrl(Context.Request.Path, Area, Controller, Action, _extension);
			}

			if (FormID == null || FormID.Length == 0)
			{
				return String.Format("document.forms[0].action = '{0}';", url);
			}
			else
			{
				return String.Format("document.getElementById('{0}').action = '{1}';", FormID, url);
			}
		}
	}
}