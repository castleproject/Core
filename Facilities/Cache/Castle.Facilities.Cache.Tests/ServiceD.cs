using System;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Summary description for ServiceD.
	/// </summary>
	[Cache]
	public class ServiceD : IServiceD
	{
		#region IServiceD Members

		[Cache("FifoCacheManager")]
		public int MyMethodA(int a, int c)
		{
			int ret = a+c;

			Console.Write(ret.ToString() + Environment.TickCount.ToString());
			return (ret);
		}

		[Cache("MemoryCacheManager")]
		public string MyMethodB(string s)
		{
			string ret = "Hello "+s;

			Console.Write(ret + Environment.TickCount.ToString());
			return (ret);
		}

		#endregion
	}
}
