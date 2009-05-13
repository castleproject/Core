// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT // we do not support xml config on SL

namespace Castle.Windsor.Tests.Configuration2
{
	using System;
	using System.IO;
	using System.Threading;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture, Explicit]
	public class SynchronizationProblemTestCase
	{
		private string dir = ConfigHelper.ResolveConfigPath("Configuration2/");
		private WindsorContainer container;
		private ManualResetEvent startEvent = new ManualResetEvent(false);
		private ManualResetEvent stopEvent = new ManualResetEvent(false);

		[SetUp]
		public void Init()
		{
			string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir +
																			  "synchtest_config.xml");

			container = new WindsorContainer(new XmlInterpreter(file));

			container.Resolve(typeof(ComponentWithConfigs));
		}

		[TearDown]
		public void Terminate()
		{
			container.Dispose();
		}

		[Test]
		public void ResolveWithConfigTest()
		{
			const int threadCount = 50;

			Thread[] threads = new Thread[threadCount];

			for (int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(10 * 2000);

			stopEvent.Set();
		}

		private void ExecuteMethodUntilSignal()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while (!stopEvent.WaitOne(1, false))
			{
				try
				{
					ComponentWithConfigs comp = (ComponentWithConfigs) container.Resolve(typeof(ComponentWithConfigs));

					Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, comp.Name);
					Assert.AreEqual(90, comp.Port);
					Assert.AreEqual(1, comp.Dict.Count);
				}
				catch(Exception ex)
				{
					Console.WriteLine(DateTime.Now.Ticks + " ---------------------------\r\n" + ex);
				}
			}
		}
	}
}

#endif