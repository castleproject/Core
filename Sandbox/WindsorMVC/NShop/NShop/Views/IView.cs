using System.Collections.Generic;
using System.Web;

namespace NShop.Views
{
	public interface IView
	{
		IDictionary<string, object> Items { get; set; }
	}
}