// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Helpers;


	[Helper( typeof(AjaxHelper) )]
	public class AjaxController : SmartDispatcherController
	{
		public AjaxController()
		{
		}

		public void JsFunctions()
		{
			RenderText( new AjaxHelper().GetJavascriptFunctions() );
		}

		public void LinkToFunction()
		{
			RenderText( new AjaxHelper().LinkToFunction("<img src='myimg.gid'>", "alert('Ok')") );
		}

		public void LinkToRemote()
		{
			Hashtable options = new Hashtable();
			RenderText( new AjaxHelper().LinkToRemote("<img src='myimg.gid'>", "/controller/action.rails", options) );
		}

		public void BuildFormRemoteTag()
		{
			RenderText( new AjaxHelper().BuildFormRemoteTag("url", "post") );
		}

		public void ObserveField()
		{
			RenderText( new AjaxHelper().ObserveField("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent") );
		}

		public void ObserveForm()
		{
			RenderText( new AjaxHelper().ObserveForm("myfieldid", 2, "/url", "elementToBeUpdated", "newcontent") );
		}

		public void Index()
		{
			
		}

		public void InferAddress()
		{
			RenderText("<b>pereira leite st, 44<b>");
		}

		public void AccountFormValidate(String name, String addressf)
		{
			String message = "";

			if (name == null || name.Length == 0)
			{
				message = "<b>Please, dont forget to enter the name<b>";
			}
			if (addressf == null || addressf.Length == 0)
			{
				message += "<b>Please, dont forget to enter the address<b>";
			}

			if (message == "")
			{
				message = "Seems that you know how to fill a form! :-)";
			}

			RenderText(message);
		}
	}
}
