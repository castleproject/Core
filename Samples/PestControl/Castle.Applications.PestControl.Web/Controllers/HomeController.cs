// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	using Castle.CastleOnRails.Framework;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for HomeController.
	/// </summary>
	[Transient]
	public class HomeController : SmartDispatcherController
	{
		private PestControlModel _model;

		public HomeController(PestControlModel model)
		{
			_model = model;
		}

		public void Index()
		{
			
		}

		public void Login(String email, String passwd)
		{
			if (!_model.Users.Authenticate(email, passwd))
			{
				// TODO: Error message

				Redirect("home", "index");
			}
			else
			{
				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(email, true, 14 * 24 * 60);
				String cookieContents = FormsAuthentication.Encrypt( ticket );
				
				Context.Response.Cookies.Add( 
					new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents) );

				Redirect("dashboard", "index");
			}
		}

		public void SignUp()
		{
			Redirect("registration", "signup");
		}
	}
}
