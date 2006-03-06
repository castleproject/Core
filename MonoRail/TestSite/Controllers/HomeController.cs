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

namespace TestSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	public class HomeController : Controller
	{
		public HomeController()
		{
		}

		public void Index()
		{
		}

        [AccessibleThrough(Verb.Get)]
        public void GetOnlyMethod()
        {
            CancelLayout();
            RenderText("GetOnlyMethod");
        }
    
        [AccessibleThrough(Verb.Post)]
        public void PostOnlyMethod()
        {
            CancelLayout();
            RenderText("PostOnlyMethod");
        }

		public void Flash1()
		{
			Flash.Add("errormessage", "Some error");

			RenderText("RenderText output");
		}

		public void Other()
		{
			RenderView("display");
		}

		public void Welcome()
		{
			RenderView("heyhello");
		}

		public void RedirectAction()
		{
			Redirect("home", "index");
		}

		public void RedirectForOtherArea()
		{
			Redirect("subarea", "home", "index");
		}

		public void Bag()
		{
			PropertyBag.Add( "CustomerName", "hammett" );
			PropertyBag.Add( "List", new String[] { "1", "2", "3" } );
		}

		public void Bag2()
		{
			PropertyBag.Add( "CustomerName", "hammett" );
			PropertyBag.Add( "List", new String[] { "1", "2", "3" } );
		}
	}
}
