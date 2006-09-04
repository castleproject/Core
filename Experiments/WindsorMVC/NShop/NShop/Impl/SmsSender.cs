using System;
using System.Collections.Generic;
using System.Text;
using NShop.Model;
using NShop.Services;

namespace NShop.Impl
{
	public class SmsSender : ISender
	{
		#region INotifyUser Members

		public void Send(Message msg)
		{
			//send sms
		}

		#endregion
	}
}
