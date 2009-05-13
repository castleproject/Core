// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SampleSite.Controllers.AjaxArea
{
    using System;
	using System.Threading;
	using System.Collections;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class FormRemoteTagController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }

		public void AddContact(String name, String email)
		{
			ObtainContacts().Add( new Contact(name, email) );

			PropertyBag.Add( "contacts", ObtainContacts() );

			RenderView("contactlist");
		}

		public void AddContact2(String name, String email)
		{
			// We pretend that this is a slow operation
			Thread.Sleep( 1000 * 3 );

			ObtainContacts().Add( new Contact(name, email) );

			PropertyBag.Add( "contacts", ObtainContacts() );

			RenderView("contactlist");
		}

		protected IList ObtainContacts()
		{
			LayoutName = null;

			if (!Session.Contains("contacts"))
			{
				Session["contacts"] = new ArrayList();
			}

			return Session["contacts"] as IList;
		}
	}

	public class Contact
	{
		private String name;
		private String email;

		public Contact(string name, string email)
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
