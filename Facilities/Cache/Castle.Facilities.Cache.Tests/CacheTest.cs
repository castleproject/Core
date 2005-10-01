using System;
using NUnit.Framework;
using Castle.Windsor;

namespace Castle.Facilities.Cache.Tests
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class CacheTest
	{
		[Test]
		public void TestCache()
		{
			IWindsorContainer container = new WindsorContainer("Castle.Facilities.Cache.Tests.config");

			container.AddComponent("ServiceA",typeof(IServiceA), typeof(ServiceA));

			IServiceA serviceA = container[typeof(IServiceA)] as IServiceA;
			IServiceB serviceB= container[typeof(IServiceB)] as IServiceB;

			serviceA.MyMethod(2, 5.5M);
			serviceB.MyMethod("cache", "serviceB", "MyMethod");

			serviceA.MyMethod(2, 5.5M);
		}
	}
}
