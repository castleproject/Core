namespace Castle.MonoRail.Framework.Test
{
	using Castle.Components.Common.EmailSender;

	public class MockSmtpSender : IEmailSender
	{
		private readonly MockRailsEngineContext context;

		public MockSmtpSender(MockRailsEngineContext context)
		{
			this.context = context;
		}

		public void Send(string from, string to, string subject, string messageText)
		{
			Send(new Message(from, to, subject, messageText));
		}

		public void Send(Message message)
		{
			context.AddEmailMessageSent(message);
		}

		public void Send(Message[] messages)
		{
			foreach(Message message in messages)
			{
				context.AddEmailMessageSent(message);
			}
		}
	}
}
