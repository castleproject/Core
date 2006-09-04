using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	/// <summary>
	/// Summary description for Helper.
	/// </summary>
	internal class Helper
	{
		public static bool IsEmpty(string val)
		{
			return val == null ||  val == "";
		}
	}
}
