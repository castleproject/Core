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

namespace Castle.MicroKernel.Tests.Pools
{
	using System;
	using System.Threading;

	using NUnit.Framework;


	[TestFixture]
	public class MultithreadedPooledTestCase
	{
		private ManualResetEvent  _startEvent = new ManualResetEvent(false);
		private ManualResetEvent  _stopEvent  = new ManualResetEvent(false);
		private IKernel _kernel;

		[Test]
		public void Multithreaded()
		{
			_kernel = new DefaultKernel();
			_kernel.AddComponent( "a", typeof(PoolableComponent1) );

			const int threadCount = 15;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			_startEvent.Set();

			Thread.CurrentThread.Join(3 * 1000);

			_stopEvent.Set();
		}

		public void ExecuteMethodUntilSignal()
		{
			_startEvent.WaitOne(int.MaxValue, false);

			while (!_stopEvent.WaitOne(1, false))
			{
				PoolableComponent1 instance = _kernel["a"] as PoolableComponent1;
				
				Assert.IsNotNull( instance );

				Thread.Sleep(1 * 500);

				_kernel.ReleaseComponent(instance);
			}
		}
	}
}
