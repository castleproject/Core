using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description résumée de ServiceB.
	/// </summary>
	public class ServiceB : IServiceB
	{

		#region Membres de IServiceB

		public string MyMethod(string a, string b, string c)
		{
			string ret = a+b+c;

			Console.Write( ret.ToString() + Environment.TickCount.ToString() );
			return ret;
		}

		#endregion
	}
}
