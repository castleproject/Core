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

	public class ConditionController : ARSmartDispatcherController
	{
		public void New()
		{
			PropertyBag["comments"] = Comment.FindAll();
			PropertyBag["types"] = ConditionType.FindAll();
		}

		[AccessibleThrough(Verb.Post)]
		public void Insert([ARDataBind("condition", AutoLoad=AutoLoadBehavior.OnlyNested)] Condition condition)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[condition];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				condition.Create();
				
				PropertyBag.Add("condition", condition);
			}
		}
		
		public void Edit([ARFetch("id", false, true)] Condition condition)
		{
			PropertyBag.Add("condition", condition);
			PropertyBag["types"] = ConditionType.FindAll();
		}
		
		[AccessibleThrough(Verb.Post)]
		public void Update([ARDataBind("condition", AutoLoad=AutoLoadBehavior.Always)] Condition condition)
		{
			ErrorList errorList = (ErrorList) BoundInstanceErrors[condition];
			
			PropertyBag.Add("errorlist", errorList);
			
			if (errorList.Count == 0)
			{
				condition.Update();
				
				PropertyBag.Add("condition", condition);
			}
		}
	}
}
