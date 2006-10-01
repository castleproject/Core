// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.Cache.Tests
{
	using System;
	using System.IO;

	using Castle.Facilities.Cache.Manager;
	using Castle.Windsor;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class CacheTest
	{
		private StringWriter _outWriter = new StringWriter();
		private IWindsorContainer _container = null;
		
		[SetUp]
		public void SetUp()
		{
			_container = new WindsorContainer(ConfigHelper.ResolvePath("../Castle.Facilities.Cache.Tests.config"));
			_container.AddComponent("ServiceA",typeof(IServiceA), typeof(ServiceA));
			_container.AddComponent("ServiceC",typeof(IServiceC), typeof(ServiceC));
			_container.AddComponent("ServiceD",typeof(IServiceD), typeof(ServiceD));

			ResetConsoleOut();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_container.Dispose();
		}

		public void ResetConsoleOut()
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

			serviceA.MyMethodNotcached("Gilles");

			serviceA.MyMethod(2, 5.5M);
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );
		}

		[Test]
		public void TestMultipleCacheViaCode()
		{
			IServiceD serviceD = _container[typeof(IServiceD)] as IServiceD;

			// MethodeA
			FifoCacheManager fifoCacheManager = _container["Another.Cache"] as FifoCacheManager;
			Assert.IsTrue(fifoCacheManager.KeyList.Count==0);

			serviceD.MyMethodA(2, 5);
			string consoleContents = _outWriter.GetStringBuilder().ToString();
			Assert.IsTrue(fifoCacheManager.KeyList.Count==1);

			serviceD.MyMethodA(2, 5);
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );

			serviceD.MyMethodA(3, 5);
			Assert.IsFalse(consoleContents == _outWriter.GetStringBuilder().ToString() );

			// MethodeB
			ResetConsoleOut();

			serviceD.MyMethodB( "Castle" );
			consoleContents = _outWriter.GetStringBuilder().ToString();

			serviceD.MyMethodB( "Castle" );
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );

			serviceD.MyMethodB( "iBATIS" );
			Assert.IsFalse(consoleContents == _outWriter.GetStringBuilder().ToString() );
		}

		[Test]
		public void TestCacheViaConfig()
		{
			IServiceB serviceB= _container[typeof(IServiceB)] as IServiceB;

			// MethodeA
			FifoCacheManager fifoCacheManager = _container["Another.Cache"] as FifoCacheManager;
			Assert.IsTrue(fifoCacheManager.KeyList.Count==0);

			serviceB.MyMethodA("cache", "serviceB", "MyMethodA");
			string consoleContents = _outWriter.GetStringBuilder().ToString();
			Assert.IsTrue(fifoCacheManager.KeyList.Count==1);

			serviceB.MyMethodA("cache", "serviceB", "MyMethodA");
			Assert.AreEqual(consoleContents, _outWriter.GetStringBuilder().ToString() );

			// MethodeB
			ResetConsoleOut();

			serviceB.MyMethodB();
			consoleContents = _outWriter.GetStringBuilder().ToString();

			serviceB.MyMethodB();
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

			ResetConsoleOut();
			
			WaitOneMillisecond();

			serviceA.MyMethod(2, 5.5M);
			Assert.IsFalse( consoleContents == _outWriter.GetStringBuilder().ToString() );
		}

		private void WaitOneMillisecond()
		{
			// Wait a moment
			DateTime startTime = DateTime.Now;
			while(startTime.Millisecond==DateTime.Now.Millisecond)
			{}
		}
	}
}
