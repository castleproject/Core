// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Windsor.Tests
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using Castle.Model;
	using Castle.Model.Interceptor;
	using Castle.MicroKernel;
	using Castle.Windsor.Tests.Components;

	/// <summary>
	/// Summary description for InterceptorsTestCase.
	/// </summary>
	[TestFixture]
	public class InterceptorsTestCase
	{
		private IWindsorContainer _container;
		private ManualResetEvent  _startEvent = new ManualResetEvent(false);
		private ManualResetEvent  _stopEvent = new ManualResetEvent(false);
		private CalculatorService _service;

		[SetUp]
		public void Init()
		{
			_container = new WindsorContainer();

			_container.AddFacility( "1", new MyInterceptorGreedyFacility() );
			_container.AddFacility( "2", new MyInterceptorGreedyFacility() );
			_container.AddFacility( "3", new MyInterceptorGreedyFacility() );
		}

		[TearDown]
		public void Terminate()
		{
			_container.Dispose();
		}

		[Test]
		public void InterfaceProxy()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", 
				typeof(ICalcService), typeof(CalculatorService)  );

			ICalcService service = (ICalcService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.AreEqual( 7, service.Sum(2,2) );
		}

		[Test]
		public void InterfaceProxyWithLifecycle()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", 
				typeof(ICalcService), typeof(CalculatorServiceWithLifecycle)  );

			ICalcService service = (ICalcService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.IsTrue( service.Initialized );
			Assert.AreEqual( 7, service.Sum(2,2) );

			Assert.IsFalse( service.Disposed );

			_container.Release( service );

			Assert.IsTrue( service.Disposed );
		}

		[Test]
		public void ClassProxy()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(CalculatorService)  );

			CalculatorService service = (CalculatorService) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.AreEqual( 7, service.Sum(2,2) );
		}

		[Test]
		public void ClassProxyWithAttributes()
		{
			_container = new WindsorContainer(); // So we wont use the facilities

			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(CalculatorServiceWithAttributes)  );

			CalculatorServiceWithAttributes service = 
				(CalculatorServiceWithAttributes) _container.Resolve("key");

			Assert.IsNotNull(service);
			Assert.AreEqual( 5, service.Sum(2,2) );
		}

		[Test]
		public void Multithreaded()
		{
			_container.AddComponent( "interceptor", typeof(ResultModifierInterceptor) );
			_container.AddComponent( "key", typeof(CalculatorService)  );

			_service = (CalculatorService) _container.Resolve("key");

			const int threadCount = 10;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			_startEvent.Set();

			Thread.CurrentThread.Join(1 * 2000);

			_stopEvent.Set();
		}

		public void ExecuteMethodUntilSignal()
		{
			_startEvent.WaitOne(int.MaxValue, false);

			while (!_stopEvent.WaitOne(1, false))
			{
				Assert.AreEqual( 7, _service.Sum(2,2) );
				Assert.AreEqual( 8, _service.Sum(3,2) );
				Assert.AreEqual( 10, _service.Sum(3,4) );
			}
		}
	}

	public class MyInterceptorGreedyFacility : IFacility
	{
		#region IFacility Members

		public void Init(IKernel kernel, Model.Configuration.IConfiguration facilityConfig)
		{
			kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);
		}

		public void Terminate()
		{
		}

		#endregion

		private void OnComponentRegistered(String key, IHandler handler)
		{
			if (key == "key")
			{
				handler.ComponentModel.Interceptors.Add( 
					new InterceptorReference( "interceptor" ) );
			}
		}
	}

	public class ResultModifierInterceptor : IMethodInterceptor
	{
		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			if (invocation.Method.Name.Equals("Sum"))
			{
				object result = invocation.Proceed(args);
				return ((int)result) + 1;
			}
			
			return invocation.Proceed(args);
		}
	}
}
