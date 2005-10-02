using System;
using System.IO;
using System.Threading;
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
		private StringWriter _outWriter = new StringWriter();
		private IWindsorContainer _container = null;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			_container = new WindsorContainer("Castle.Facilities.Cache.Tests.config");
			_container.AddComponent("ServiceA",typeof(IServiceA), typeof(ServiceA));
			_container.AddComponent("ServiceC",typeof(IServiceC), typeof(ServiceC));
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_container.Dispose();
		}

		[SetUp]
		public void ReplaceOut()
		{
			_outWriter.GetStringBuilder().Length = 0;
			Console.SetOut(_outWriter);
		}

		[Test]
		public void TestCacheViaCode()
		{
			IServiceA serviceA = _container[typeof(IServiceA)] as IServiceA;

			serviceA.MyMethod(2, 5.5M);
			string consoleContents = _outWriter.GetStringBuilder().ToString();

			serviceA.MyMethod(2, 5.5M);
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );
		}

		[Test]
		public void TestCacheViaConfig()
		{
			IServiceB serviceB= _container[typeof(IServiceB)] as IServiceB;

			serviceB.MyMethod("cache", "serviceB", "MyMethod");
			string consoleContents = _outWriter.GetStringBuilder().ToString();

			serviceB.MyMethod("cache", "serviceB", "MyMethod");
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );
		}

		[Test]
		public void TestFicoCache()
		{
			IServiceA serviceA = _container[typeof(IServiceA)] as IServiceA;
			IServiceC serviceC = _container[typeof(IServiceC)] as IServiceC;

			serviceA.MyMethod(2, 5.5M);
			string consoleContents = _outWriter.GetStringBuilder().ToString();

			serviceC.MyMethod(2, 5.5M);

			ReplaceOut();
			
			// Wait a moment
			DateTime startTime = DateTime.Now;
			while(startTime.Millisecond==DateTime.Now.Millisecond)
			{}

			serviceA.MyMethod(2, 5.5M);
			Assert.IsFalse( consoleContents == _outWriter.GetStringBuilder().ToString() );
		}
	}
}
