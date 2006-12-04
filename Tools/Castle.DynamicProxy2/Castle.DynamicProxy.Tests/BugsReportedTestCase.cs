namespace Castle.DynamicProxy.Tests
{
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.BugsReported;
	using NUnit.Framework;

	[TestFixture]
	public class BugsReportedTestCase
	{
		[Test]
		public void InterfaceInheritance()
		{
			ProxyGenerator generator = new ProxyGenerator();

			ICameraService proxy = (ICameraService)
			   generator.CreateInterfaceProxyWithTarget(typeof(ICameraService), 
			                                            new CameraService(), 
			                                            new StandardInterceptor());

			Assert.IsNotNull(proxy);
			
			proxy.Add("", "");
			proxy.Record(null);
		}
	}
}
