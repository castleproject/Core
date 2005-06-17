// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using Castle.DynamicProxy.Test.Classes;
	using Castle.DynamicProxy.Test.Interceptors;

	/// <summary>
	/// Summary description for MultithreadTestCase.
	/// </summary>
	[TestFixture]
	public class MultithreadTestCase
	{
		private ManualResetEvent  _startEvent = new ManualResetEvent(false);
		private ManualResetEvent  _stopEvent = new ManualResetEvent(false);
		private SpecializedServiceClass _service;

		private ProxyGenerator _generator;

		[SetUp]
		public void Init()
		{
			_generator = new ProxyGenerator();
		}

		[Test]
		public void MultithreadTest()
		{
			_service = (SpecializedServiceClass) _generator.CreateClassProxy( 
				typeof(SpecializedServiceClass), 
				new ResultModifiedInvocationHandler( ) );

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
				Assert.AreEqual( 44, _service.Sum( 20, 25 ) );
				Assert.AreEqual( -6, _service.Subtract( 20, 25 ) );
				Assert.AreEqual( true, _service.Valid );
			}
		}

	}
}
