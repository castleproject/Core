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
