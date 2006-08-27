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

namespace NVelocity
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Threading;
	using NUnit.Framework;
	using NVelocity.App;
	using NVelocity.Test;

	[TestFixture, Explicit]
	public class MultiThreadTestCase
	{
		private ManualResetEvent startEvent = new ManualResetEvent(false);
		private ManualResetEvent stopEvent = new ManualResetEvent(false);
		private ArrayList items;
		private VelocityEngine ve;

		[SetUp]
		public void Setup()
		{
			items = new ArrayList();

			items.Add("a");
			items.Add("b");
			items.Add("c");
			items.Add("d");

			ve = new VelocityEngine();
			ve.Init();
		}

		[Test]
		public void Multithreaded1()
		{
			const int threadCount = 20;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal1));
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(1 * 5000);

			stopEvent.Set();
		}

		[Test]
		public void Multithreaded2()
		{
			const int threadCount = 20;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal2));
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(1 * 5000);

			stopEvent.Set();
		}

		public void ExecuteMethodUntilSignal1()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while (!stopEvent.WaitOne(1, false))
			{
				VelocityEngine ve = new VelocityEngine();
				ve.Init();

				StringWriter sw = new StringWriter();

				VelocityContext c = new VelocityContext();
				c.Put("x", new Something());
				c.Put("items", items);

				bool ok = ve.Evaluate(c, sw,
					"ContextTest.CaseInsensitive",
					@"
					#foreach( $item in $items )
						$item,
					#end
				");

				Assert.IsTrue(ok, "Evalutation returned failure");
				Assert.AreEqual("a,b,c,d,", Normalize(sw));
			}
		}

		public void ExecuteMethodUntilSignal2()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while (!stopEvent.WaitOne(1, false))
			{
				StringWriter sw = new StringWriter();

				VelocityContext c = new VelocityContext();
				c.Put("x", new Something());
				c.Put("items", items);

				bool ok = ve.Evaluate(c, sw,
					"ContextTest.CaseInsensitive",
					@"
					#foreach( $item in $items )
						$item,
					#end
				");

				Assert.IsTrue(ok, "Evalutation returned failure");
				Assert.AreEqual("a,b,c,d,", Normalize(sw));
			}
		}
		
		private string Normalize(StringWriter sw)
		{
			return Regex.Replace(sw.ToString(), "\\s+", "");
		}
	}
}
