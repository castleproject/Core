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

namespace TestSiteARSupport.Controllers
{
	using System;
	using Castle.Components.Binder;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	
	using TestSiteARSupport.Model;

	public class ProductLicenseController : ARSmartDispatcherController
	{
		public void New()
		{
			PropertyBag.Add("pl", new ProductLicense());
		}
		
		public void NewWithAccounts()
		{
			PropertyBag.Add("pl", new ProductLicense());
			PropertyBag.Add("accounts", Account.FindAll());
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("pl", AutoLoad=AutoLoadBehavior.OnlyNested)] ProductLicense pl)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[pl];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				pl.Create();
				
				PropertyBag.Add("pl", pl);
			}
		}
		
		public void Edit([ARFetch("id", false, true)] ProductLicense pl)
		{
			PropertyBag.Add("pl", pl);
			PropertyBag.Add("accounts", Account.FindAll());
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("pl", AutoLoad=AutoLoadBehavior.Always)] ProductLicense pl)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[pl];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				pl.Update();
				
				PropertyBag.Add("pl", pl);
			}
		}
		
		public void RemoveConfirm([ARFetch("id", false, true)] ProductLicense pl)
		{
			PropertyBag.Add("pl", pl);
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Delete([ARDataBind("pl", AutoLoad=AutoLoadBehavior.Always)] ProductLicense pl)
		{
			pl.Delete();
		}
	}
}
