// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in comaccountiance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by apaccounticable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or imaccountied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace TestSiteARSupport.Controllers
{
	using System;
	using Castle.Components.Binder;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	using TestSiteARSupport.Model;

	public class AccountController : ARSmartDispatcherController
	{
		public void New()
		{
			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
		}

		public void New2()
		{
			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
		}

		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("account", AutoLoad=AutoLoadBehavior.OnlyNested)] Account account)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[account];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				account.Create();
				
				PropertyBag.Add("account", account);
			}
		}
		
		public void Edit([ARFetch("id", false, true)] Account account)
		{
			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
			PropertyBag.Add("account", account);
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("account", AutoLoad=AutoLoadBehavior.Always)] Account account)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[account];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				account.Update();
				
				PropertyBag.Add("account", account);
			}
		}
		
		public void RemoveConfirm([ARFetch("id", false, true)] Account account)
		{
			PropertyBag.Add("account", account);
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Delete([ARDataBind("account", AutoLoad=AutoLoadBehavior.Always)] Account account)
		{
			account.Delete();
		}
	}
}
