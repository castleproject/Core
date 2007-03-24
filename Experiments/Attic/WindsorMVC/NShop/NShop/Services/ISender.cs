using System;
using System.Collections.Generic;
using System.Text;
using NShop.Model;

namespace NShop.Services
{
	public interface ISender
	{
		void Send(Message msg);
	}
}
