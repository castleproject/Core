using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace NShop.Model
{
	public class Message
	{
		Customer to;
		string contents;

		public Customer To
		{
			get { return to; }
		}

		public string Contents
		{
			get { return contents; }
		}

		public Message(Customer to, string message)
		{
			this.to = to;
			this.contents = message;
		}
	}
}
