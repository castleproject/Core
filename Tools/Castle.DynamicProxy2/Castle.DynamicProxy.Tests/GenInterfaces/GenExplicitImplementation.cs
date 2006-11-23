namespace Castle.DynamicProxy.Tests.GenInterfaces
{
	using System;
	using System.Collections.Generic;

	public class GenExplicitImplementation<T> : InterfaceWithExplicitImpl<T>
	{
		IEnumerator<T> InterfaceWithExplicitImpl<T>.GetEnum1()
		{
			return null;
		}
	}
	
	public interface InterfaceWithExplicitImpl<T>
	{
		IEnumerator<T> GetEnum1();
	}
}
