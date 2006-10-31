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

namespace GettingStartedSample.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class ContactController : SmartDispatcherController
	{
		public void ContactForm()
		{
		}
		
		public void SendContactMessage([DataBind("contact")] Contact contact)
		{
			// Pretend to save the contact ...
			
			// ..work work work..
			
			// Now lets add the contact to the property bag
			// so we can render a nice message back to the user
			
			PropertyBag["contact"] = contact;
			
			RenderView("confirmation");
		}
	}
	
	public class Contact
	{
		private string from;
		private string area;
		private string subject;
		private string message;

		public string From
		{
			get { return from; }
			set { from = value; }
		}

		public string Area
		{
			get { return area; }
			set { area = value; }
		}

		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}
