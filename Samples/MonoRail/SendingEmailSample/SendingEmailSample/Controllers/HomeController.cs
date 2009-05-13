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

namespace SendingEmailSample.Controllers
{
	using System;
	using Castle.Components.Common.EmailSender;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
		}
		
		[Rescue("problemsendingemail")]
		public void SendSimple(String to, String subject)
		{
			PropertyBag["to"] = to;
			PropertyBag["subject"] = subject;
			
			RenderEmailAndSend("simple");
			
			RenderView("EmailSent");
		}

		[Rescue("problemsendingemail")]
		public void SendHtml(String to, String subject)
		{
			Message message = RenderMailMessage("htmlemail");
			
			message.From = "you@yourserver.com";
			message.To = to;
			message.Subject = subject;
			
			DeliverEmail(message);
			
			PropertyBag["to"] = to;
			PropertyBag["subject"] = subject;

			RenderView("EmailSent");
		}
	}
}
