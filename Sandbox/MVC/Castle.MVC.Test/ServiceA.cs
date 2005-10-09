using System;

namespace Castle.MVC.Test
{
	/// <summary>
	/// Description résumée de ServiceA.
	/// </summary>
	public class ServiceA : IServiceA
	{
		#region IServiceA Members

		public string MyMethodNotcached(string a)
		{
			return "Hello "+a;
		}


		public decimal MyMethod(int a , decimal c)
		{
			decimal ret = a+c;
			return (ret);
		}

		#endregion

	}
}
