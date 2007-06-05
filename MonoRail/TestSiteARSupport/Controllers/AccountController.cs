// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace TestSiteARSupport.Controllers
{
	using Castle.ActiveRecord;
	using Castle.Components.Binder;
	using Castle.Components.Validator;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	
	using TestSiteARSupport.Model;
	
	[Layout("default")]
	public class AccountController : ARSmartDispatcherController
	{
		public void New()
		{
			PropertyBag["accounttype"] = typeof(Account);
			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
			PropertyBag.Add("users", User.FindAll());
		}

		public void New2()
		{
			PropertyBag["accounttype"] = typeof(Account);
			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
			PropertyBag.Add("users", User.FindAll());
		}

		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("account", AutoLoad=AutoLoadBehavior.OnlyNested, Validate=true)] Account account)
		{
			ErrorList errorList = BoundInstanceErrors[account];
			ErrorSummary summary = GetErrorSummary(account);
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				account.Create();
				
				PropertyBag.Add("account", account);
			}
		}
		
		public void Edit(int id)
		{
			if (Flash.Contains("account"))
			{
				PropertyBag["account"] = Flash["account"];
			}
			else
			{
				PropertyBag["account"] = ActiveRecordMediator<Account>.FindByPrimaryKey(id);
			}

			PropertyBag.Add("licenses", ProductLicense.FindAll());
			PropertyBag.Add("permissions", AccountPermission.FindAll());
			PropertyBag.Add("users", User.FindAll());
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("account", AutoLoad=AutoLoadBehavior.NullIfInvalidKey, Expect="account.Permissions,account.Users", Validate=true)] Account account)
		{
			ErrorList errorList = BoundInstanceErrors[account];
			ErrorSummary summary = GetErrorSummary(account);

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
		public void Delete([ARFetch("account", false, true)] Account account)
		{
			account.Delete();
		}
	}
}
