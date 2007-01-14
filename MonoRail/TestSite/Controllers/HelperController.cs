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

namespace TestSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using DateFormatHelper=TestSite.Helpers.DateFormatHelper;

	[Helper(typeof(DateFormatHelper))]
	public class HelperController : SmartDispatcherController
	{
		public HelperController()
		{
		}

		public void InheritedHelpers()
		{
			PropertyBag.Add( "date", new DateTime(1979, 7, 16) );
		}

		public void MyDeclaredHelpers()
		{
			PropertyBag.Add( "date", new DateTime(1979, 7, 16) );
		}

		public void HelpMe()
		{
			
		}

		public virtual void LinkTo(string name, string action)
		{
			HtmlHelper htmlHelper = (HtmlHelper) Helpers["HtmlHelper"];

			RenderText(htmlHelper.LinkTo(name, action));
		}
	}

	[ControllerDetails(Area="test")]
	public class Helper2Controller : HelperController
	{
		public Helper2Controller()
		{
		}

		public override void LinkTo(string name, string action)
		{
			base.LinkTo(name, action);
		}
	}
}
