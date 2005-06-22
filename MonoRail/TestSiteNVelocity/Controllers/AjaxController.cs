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

namespace TestSiteNVelocity.Controllers
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	public class AjaxController : SmartDispatcherController
	{
		private AjaxHelper Helper
		{
			get
			{
				return (AjaxHelper) Helpers["AjaxHelper"];
			}
		}

		public AjaxController()
		{
		}

		public void JsFunctions()
		{
			RenderText( Helper.GetJavascriptFunctions() );
		}

		public void LinkToFunction()
		{
			RenderText( Helper.LinkToFunction("<img src='myimg.gid'>", "alert('Ok')") );
		}

		public void LinkToRemote()
		{
			Hashtable options = new Hashtable();
			RenderText( Helper.LinkToRemote("<img src='myimg.gid'>", "/controller/action.rails", options) );
		}

		public void BuildFormRemoteTag()
		{
			RenderText( Helper.BuildFormRemoteTag("url", null, null) );
		}

		public void ObserveField()
		{
			RenderText( Helper.ObserveField("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent") );
		}

		public void ObserveForm()
		{
			RenderText( Helper.ObserveForm("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent") );
		}

		public void Index()
		{
			IList list = GetList();

			PropertyBag.Add("users", list);
		}

		public void AddUserWithAjax(String name, String email)
		{
			GetList().Add( new User(name, email) );

			Index();

			RenderView("/userlist");
		}

		public void InferAddress()
		{
			RenderText("<b>pereira leite st, 44<b>");
		}

		public void AccountFormValidate(String name, String addressf)
		{
			String message = "";

			if (name == null || name.Length == 0)
			{
				message = "<b>Please, dont forget to enter the name<b>";
			}
			if (addressf == null || addressf.Length == 0)
			{
				message += "<b>Please, dont forget to enter the address<b>";
			}

			if (message == "")
			{
				message = "Seems that you know how to fill a form! :-)";
			}

			RenderText(message);
		}

		private IList GetList()
		{
			IList list = Context.Session["list"] as IList;
			
			if (list == null)
			{
				list = new ArrayList();

				list.Add( new User("somefakeuser", "fakeemail@server.net") );
				list.Add( new User("someotherfakeuser", "otheremail@server.net") );

				Context.Session["list"] = list;
			}
			
			return list;
		}
	}

	public class User
	{
		private String name;
		private String email;

		public User(string name, string email)
		{
			this.name = name;
			this.email = email;
		}

		public string Name
		{
			get { return name; }
		}

		public string Email
		{
			get { return email; }
		}
	}
}
