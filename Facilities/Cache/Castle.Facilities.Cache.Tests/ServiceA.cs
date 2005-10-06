using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Description résumée de ServiceA.
	/// </summary>
	[Cache("FifoCacheManager")]
	public class ServiceA : IServiceA
	{
		#region IServiceA Members

		public string MyMethodNotcached(string a)
		{
			return "Hello "+a;
		}

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
