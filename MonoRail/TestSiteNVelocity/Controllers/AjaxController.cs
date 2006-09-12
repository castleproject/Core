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

namespace TestSiteNVelocity.Controllers
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework;

	
	public class AjaxController : SmartDispatcherController
	{
		#region ObserveField
		
		public void ObserveField()
		{
		}

		public void InferAddress(String value)
		{
			RenderText("Address " + value);
		}
		
		#endregion
		
		#region ObserveForm
		
		public void ObserveForm()
		{
			
		}

		public void AccountFormValidate(String name, String addressf)
		{
			RenderText("name {0} address {1}", name, addressf);
		}

		#endregion
		
		#region AutoCompletion

		/// <summary>
		/// Auto completion action
		/// </summary>
		public void AutoCom()
		{
		}

		/// <summary>
		/// Auto completion results
		/// </summary>
		public void NameAutoCompletion(String name)
		{
			RenderText("<ul class=\"names\"><li class=\"name\">Jisheng Johnny</li><li class=\"name\">John Diana</li><li class=\"name\">Johnathan Maurice</li></ul>");
		}
		
		#endregion
		
		#region Periodically calls
		
		public void PeriodicallyCall()
		{
		}
		
		/// <summary>
		/// Invoked by Ajax
		/// </summary>
		public void PeriodicallyCalled(int value)
		{
			RenderText((value + value).ToString());
		}
		
		#endregion
		
		#region JS Proxies

		public void JsProxies()
		{
			
		}
		
		[AjaxAction]
		public void InvocableMethod1()
		{
			RenderText("Success");
		}

		[AjaxAction("friendlyName")]
		public void InvocableMethod2(String value)
		{
			RenderText("Success " + value);
		}
		
		#endregion
		
		#region BuildFormRemoteTag
		
		public void FormRemoteTag()
		{
			IList list = GetList();
			PropertyBag.Add("users", list);
		}

		public void AddUserWithAjax(String name, String email)
		{
			GetList().Add(new User(name, email));
			
			IList list = GetList();
			PropertyBag.Add("users", list);

			RenderView("/userlist");
		}

		private IList GetList()
		{
			IList list = Context.Session["list"] as IList;

			if (list == null)
			{
				list = new ArrayList();

				list.Add(new User("somefakeuser", "fakeemail@server.net"));
				list.Add(new User("someotherfakeuser", "otheremail@server.net"));

				Context.Session["list"] = list;
			}

			return list;
		}
		
		#endregion
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
