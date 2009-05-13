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

namespace NVelocity
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Threading;
	using App;
	using NUnit.Framework;
	using Test;

	[TestFixture, Explicit]
	public class MultiThreadTestCase
	{
		private ManualResetEvent startEvent = new ManualResetEvent(false);
		private ManualResetEvent stopEvent = new ManualResetEvent(false);
		private ArrayList items;
		private VelocityEngine velocityEngine;

		[SetUp]
		public void Setup()
		{
			items = new ArrayList();

			items.Add("a");
			items.Add("b");
			items.Add("c");
			items.Add("d");

			velocityEngine = new VelocityEngine();
			velocityEngine.Init();
		}

		[Test]
		public void Multithreaded_ASTExecuteDoesNotShareParams()
		{
			const int threadCount = 1;

			Thread[] threads = new Thread[threadCount];

			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(ExecuteMethodUntilSignal3);
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(3 * 5000);

			stopEvent.Set();
		}

		[Test]
		public void Multithreaded1()
		{
			const int threadCount = 30;

			Thread[] threads = new Thread[threadCount];

			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(ExecuteMethodUntilSignal1);
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(1 * 5000);

			stopEvent.Set();
		}

		[Test]
		public void Multithreaded_ReuseVelocityEngineInstanceConcurrently()
		{
			const int threadCount = 30;

			Thread[] threads = new Thread[threadCount];

			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(ExecuteMethodUntilSignal2);
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(1 * 5000);

			stopEvent.Set();
		}

		/// <summary>
		/// This test starts a VelocityEngine for each execution
		/// </summary>
		public void ExecuteMethodUntilSignal1()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while(!stopEvent.WaitOne(0, false))
			{
				VelocityEngine velocityEngine = new VelocityEngine();
				velocityEngine.Init();

				StringWriter sw = new StringWriter();

				VelocityContext c = new VelocityContext();
				c.Put("x", new Something());
				c.Put("items", items);

				bool ok = velocityEngine.Evaluate(c, sw,
				                                  "ContextTest.CaseInsensitive",
				                                  @"
					#foreach( $item in $items )
						$item,
					#end
				");

				Assert.IsTrue(ok, "Evaluation returned failure");
				Assert.AreEqual("a,b,c,d,", Normalize(sw));
			}
		}

		/// <summary>
		/// This test uses the previously created velocity engine
		/// </summary>
		public void ExecuteMethodUntilSignal2()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while(!stopEvent.WaitOne(0, false))
			{
				StringWriter sw = new StringWriter();

				VelocityContext c = new VelocityContext();
				c.Put("x", new Something());
				c.Put("items", items);

				bool ok = velocityEngine.Evaluate(c, sw,
				                                  "ContextTest.CaseInsensitive",
				                                  @"
					#foreach($item in $items)
						$item,
					#end

					$x.Print('hey') $x.Contents('test', '1')
				");

				Assert.IsTrue(ok, "Evaluation returned failure");
				Assert.AreEqual("a,b,c,d,heytest,1", Normalize(sw));
			}
		}

		public void ExecuteMethodUntilSignal3()
		{
			startEvent.WaitOne(int.MaxValue, false);

			try
			{
				while(!stopEvent.WaitOne(0, false))
				{
					StringWriter sw = new StringWriter();

					VelocityContext c = new VelocityContext();
					c.Put("x", new Urlhelper());

					bool ok = velocityEngine.Evaluate(c, sw, string.Empty,
					                                  "#foreach($i in [1..3000]) \r\n" +
					                                  "#set($temp = $x.For(\"%{controller='Test',id=$i}\")) \r\n" +
					                                  "#end \r\n");

					Assert.IsTrue(ok, "Evaluation returned failure");
				}
			}
			catch(System.Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private string Normalize(StringWriter sw)
		{
			return Regex.Replace(sw.ToString(), "\\s+", string.Empty);
		}

		private class Urlhelper
		{
			public string For(IDictionary parameters)
			{
				if (parameters == null)
				{
					throw new ArgumentNullException("parameters", "parameters cannot be null");
				}

				try
				{
					string controller = (string) parameters["controller"];
					int id = (int) parameters["id"];

					parameters.Remove("controller");
					parameters.Remove("id");

					return controller + " " + id;
				}
				catch(System.Exception ex)
				{
					Console.WriteLine(ex.ToString());
					throw;
				}
			}
		}
	}
}