using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using NShop.Model;
using NShop.Services;

namespace NShop.Impl
{
	public class EmailSender : ISender
	{
		public void Send(Message msg)
		{
			// send email ...
		}
	}
}
