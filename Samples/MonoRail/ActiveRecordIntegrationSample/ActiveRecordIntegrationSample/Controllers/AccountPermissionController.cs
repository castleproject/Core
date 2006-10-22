// Copyright 2004-2006 Castle Project - http://www.railsproject.org/
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

namespace ActiveRecordIntegrationSample.Controllers
{
	using System;
	
	using Castle.Components.Binder;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;

	using Common.Models;

	
	[Layout("default"), Rescue("generalerror")]
	public class AccountPermissionController : ARSmartDispatcherController
	{
		public void New()
		{
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("apermission", AutoLoad=AutoLoadBehavior.Never)] AccountPermission ap)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[ap];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				ap.Create();
				
				PropertyBag.Add("id", ap.Id);
				PropertyBag.Add("ap", ap);
			}
		}
		
		public void Edit([ARFetch("id", false, true)] AccountPermission ap)
		{
			PropertyBag.Add("apermission", ap);
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("apermission", AutoLoad=AutoLoadBehavior.Always)] AccountPermission ap)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[ap];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				ap.Update();
				
				PropertyBag.Add("id", ap.Id);
				PropertyBag.Add("ap", ap);
			}
		}
		
		public void RemoveConfirm([ARFetch("id", false, true)] AccountPermission ap)
		{
			PropertyBag.Add("ap", ap);
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Delete([ARDataBind("ap", AutoLoad=AutoLoadBehavior.Always)] AccountPermission ap)
		{
			ap.Delete();
		}
	}
}
