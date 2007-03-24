using System;

namespace Castle.MVC.Test
{
	/// <summary>
	/// Description résumée de IServcieA.
	/// </summary>
	public interface IServiceA
	{
		decimal MyMethod(int a , decimal c);

		string MyMethodNotcached(string a);
	}
}
