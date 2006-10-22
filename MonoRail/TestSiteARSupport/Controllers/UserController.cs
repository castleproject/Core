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

	public class UserController : ARSmartDispatcherController
	{
		public void New()
		{
		}

		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("user", AutoLoad=AutoLoadBehavior.OnlyNested)] User user)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[user];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				user.Create();
				
				PropertyBag.Add("user", user);
			}
		}

	}
}
