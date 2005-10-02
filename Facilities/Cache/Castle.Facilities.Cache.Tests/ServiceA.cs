using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description résumée de ServiceA.
	/// </summary>
	[Cache]
	public class ServiceA : IServiceA
	{

		#region Membres de IServiceA

		[Cache]
		public decimal MyMethod(int a , decimal c)
		{
			decimal ret = a+c;

			Console.Write(ret.ToString() + Environment.TickCount.ToString());
			return (ret);
		}

		#endregion
	}
}
