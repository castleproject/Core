using System;

namespace Castle.MVC.Web.Views
{
	/// <summary>
	/// <summary>
	/// WebViews class simply references the names of
	/// each view that can appear.
	/// Those names are const strings, and will avoid
	/// mistakes in copying names across the numerous files
	/// of the project : every view name is verified by
	/// the compiler.
	/// </summary>
	public class WebViews
	{
		public const string HOME = "/views/home/index.aspx";
		public const string PAGE2 = "page2";
	}
}
