using System;
using System.Collections.Generic;
using System.Text;
using NShop.Model;
using NShop.Services;

namespace NShop.Impl
{
	public class NotificationManager : ISender
	{

		EmailSender emailSender;
		SmsSender smsSender;

		public EmailSender EmailSender
		{
			get { return emailSender; }
			set { emailSender = value; }
		}

		public SmsSender SmsSender
		{
			get { return smsSender; }
			set { smsSender = value; }
		}

		#region INotifyUser Members

		public void Send(Message msg)
		{
			if (this.EmailSender != null)
				this.EmailSender.Send(msg);
			//silly business logic
			if (msg.To.Orders.Count > 0)
				this.SmsSender.Send(msg);
		}

		#endregion
	}
}
