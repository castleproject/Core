// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Applications.PestControl.Web.Controllers
{
	using System;
	using System.Web;
	using System.Web.Security;

	using Castle.Model;

	using Castle.MonoRail.Framework;

	using Castle.Applications.PestControl.Model;

	[Layout("default")]
	public class HomeController : SmartDispatcherController
	{
		private PestControlModel _model;

		public HomeController(PestControlModel model)
		{
			_model = model;
		}

		// Actions

		public void Index()
		{
		}

		public void Authentication()
		{
			Context.Flash["ErrorMessage"] = "You must authenticate first";
			RenderView("index");
		}

		// This is a hack to avoid the infinite cycle with
		// Asp.Net page processing
		public void NotFound()
		{
			Context.Flash["ErrorMessage"] = "User not found or wrong password.";
			RenderView("index");
		}

		public void Login(String email, String passwd)
		{
			if (!_model.Users.Authenticate(email, passwd))
			{
				// This is a hack to avoid the infinite cycle with
				// Asp.Net page processing
				Redirect("home", "NotFound");
			}
			else
			{
				User user = _model.Users.FindByEmail(email);

				Session["user"] = user;

				if (HasFromUrl)
				{
					Redirect( ObtainAndRemoveFromUrl() );
				}
				else
				{
					Redirect("dashboard", "index");
				}
			}
		}

		public void SignUp()
		{
			Redirect("registration", "signup");
		}

		private String ObtainAndRemoveFromUrl()
		{
			String url = (String) Session["FromUrl"];
			Session.Remove("FromUrl");
			return url;
		}

		private bool HasFromUrl
		{
			get { return Session.Contains("FromUrl"); }
		}
	}
}
