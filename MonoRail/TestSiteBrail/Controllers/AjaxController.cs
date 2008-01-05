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

namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	public class AjaxController : SmartDispatcherController
	{
		public void AccountFormValidate(string name, string addressf)
		{
			string text1 = "";
			if ((name == null) || (name.Length == 0))
			{
				text1 = "<b>Please, dont forget to enter the name<b>";
			}
			if ((addressf == null) || (addressf.Length == 0))
			{
				text1 += "<>Please, don't forget to enter the adress<b>";
			}
			this.RenderText(text1);
		}

		public void AddUserWithAjax(string name, string email)
		{
			this.GetList().Add(new User(name, email));
			this.Index();
			this.RenderView("/userlist");
		}

		public void AutoCom()
		{
		}

		public void BuildFormRemoteTag()
		{
			Hashtable values = new Hashtable();
			values.Add("url", "url");
			this.RenderText(this.Helper.BuildFormRemoteTag(values));
		}

		private IList GetList()
		{
			IList list2 = this.Context.Session["list"] as IList;
			if (list2 == null)
			{
				list2 = new ArrayList();
				list2.Add(new User("somefakeuser", "fakeemail@server.net"));
				list2.Add(new User("someotherfakeuser", "otheremail@server.net"));
				this.Context.Session["list"] = list2;
			}
			return list2;
		}

		public void Index()
		{
			IList list1 = this.GetList();
			this.PropertyBag.Add("users", list1);
		}

		public void JsFunctions()
		{
			this.RenderText(this.Helper.InstallScripts());
		}

		public void LinkToFunction()
		{
			this.RenderText(this.Helper.LinkToFunction("<img src='myimg.gid'>", "alert('Ok')"));
		}

		public void LinkToRemote()
		{
			this.RenderText(this.Helper.LinkToRemote("<img src='myimg.gid'>", "/controller/action.rails", new Hashtable()));
		}

		public void NameAutoCompletion(string name)
		{
			this.RenderText("<ul class=\"names\"><li class=\"name\">Jisheng Johnny</li><li class=\"name\">John Diana</li><li class=\"name\">Johnathan Maurice</li></ul>");
		}

		public void ObserveField()
		{
			this.RenderText(this.Helper.ObserveField("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent"));
		}

		public void ObserveForm()
		{
			this.RenderText(this.Helper.ObserveForm("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent"));
		}

		public void PeriodInvocation()
		{
		}

		public void PeriodInvokeTarget()
		{
			this.RenderText("Ok");
		}


		public AjaxHelper Helper
		{
			get
			{
				return (AjaxHelper)this.Helpers["AjaxHelper"];
			}
		}



		[Serializable]
		private class User
		{
			public User(string name, string email)
			{
				this.name = name;
				this.email = email;
			}


			public string Email
			{
				get
				{
					return this.email;
				}
				set
				{
					this.email = value;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}


			protected string email;
			protected string name;
		}
		
	}
}
