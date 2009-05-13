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

namespace Castle.MicroKernel.Tests
{
	using System;
	using System.Collections;
	using System.Threading;
	using Castle.MicroKernel.SubSystems.Naming;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

	[TestFixture]
	public class KeySearchNamingSubSystemTestCase
	{
		[Test]
		public void EmptyDelegateReturnsFirstTypeLoaded()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey, new KeySearchNamingSubSystem());

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("2.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());
		}

		[Test]
		public void ReturnsCorrectType()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(
			                    	delegate(string key) { return key.StartsWith("castlestronghold.com"); }));

			kernel.AddComponent("castleproject.org.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("castlestronghold.com.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl2), common.GetType());
		}

		[Test]
		public void ReturnsFirstTypeWhenNotFound()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(delegate(string key) { return key.StartsWith("3"); }));

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("2.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());
		}

		[Test]
		public void ReturnsFirstMatchingType()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(delegate(string key) { return key.StartsWith("1"); }));

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("11.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());
		}

		[Test]
		public void ComponentUnregistersProperly()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(delegate(string key) { return key.StartsWith("2"); }));

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("2.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl2), common.GetType());

			kernel.RemoveComponent("2.common");

			common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());

			kernel.RemoveComponent("1.common");
			Assert.AreEqual(0, kernel.GetHandlers(typeof(ICommon)).Length);
		}

		[Test]
		public void FirstLoadedComponentUnregistersProperly()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(delegate(string key) { return key.StartsWith("1"); }));

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));
			kernel.AddComponent("2.common", typeof(ICommon), typeof(CommonImpl2));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());

			kernel.RemoveComponent("1.common");

			common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl2), common.GetType());
		}

		[Test]
		[ExpectedException(typeof(ComponentNotFoundException))]
		public void SingleComponentUnregistersProperly()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey,
			                    new KeySearchNamingSubSystem(delegate(string key) { return key.StartsWith("1"); }));

			kernel.AddComponent("1.common", typeof(ICommon), typeof(CommonImpl1));

			ICommon common = kernel[typeof(ICommon)] as ICommon;

			Assert.IsNotNull(common);
			Assert.AreEqual(typeof(CommonImpl1), common.GetType());

			kernel.RemoveComponent("1.common");

			Assert.IsFalse(kernel.HasComponent("1.common"));
			Assert.IsFalse(kernel.HasComponent(typeof(CommonImpl1)));
			common = kernel[typeof(ICommon)] as ICommon;
		}

		[Test]
		public void MultiThreadedAddResolve()
		{
			int threadCount = 100;
			ArrayList list = ArrayList.Synchronized(new ArrayList());
			Random rand = new Random();
			ManualResetEvent waitEvent = new ManualResetEvent(false);

			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey, new KeySearchNamingSubSystem());
			kernel.AddComponent("common", typeof(ICommon), typeof(CommonImpl1));

			WaitCallback resolveThread = delegate
			                             	{
			                             		waitEvent.WaitOne();
			                             		while(threadCount > 0 && list.Count == 0)
			                             		{
			                             			try
			                             			{
			                             				ICommon common = kernel[typeof(ICommon)] as ICommon;
			                             			}
			                             			catch(Exception e)
			                             			{
			                             				list.Add(e.ToString());
			                             			}
			                             		}
			                             	};
			ThreadPool.QueueUserWorkItem(resolveThread);

			WaitCallback addThread = delegate
			                         	{
			                         		waitEvent.WaitOne();
			                         		kernel.AddComponent(rand.Next() + ".common", typeof(ICommon), typeof(CommonImpl1));
			                         		Interlocked.Decrement(ref threadCount);
			                         	};
			for(int i = 0; i < threadCount; i++)
			{
				ThreadPool.QueueUserWorkItem(addThread);
			}

			waitEvent.Set();
			while(threadCount > 0 && list.Count == 0)
			{
				Thread.Sleep(15);
			}

			if (list.Count > 0)
			{
				Assert.Fail(list[0].ToString());
			}
		}

		/// <summary>
		/// What I consider an edge case (only consistently reproducable with 1000+ threads):
		///   + Large number of components that implement the same service
		///   + Thread 1 Requests component by service
		///   + Thread n is simultaneously removing component
		///   + Predicate does not match any keys for the requested service OR
		///   + Matching component is removed before the Predicate can match it
		/// </summary>
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		[Ignore("This test needs to be reviewed")]
		public void MultiThreaded_RemoveResolve_Throws_When_LargeRatio_Of_ComponentsToService()
		{
			int threadCount = 1000;
			ArrayList list = ArrayList.Synchronized(new ArrayList());
			Random rand = new Random();
			ManualResetEvent waitEvent = new ManualResetEvent(false);

			IKernel kernel = new DefaultKernel();
			kernel.AddSubSystem(SubSystemConstants.NamingKey, new KeySearchNamingSubSystem(delegate { return false; }));

			kernel.AddComponent("common", typeof(ICommon), typeof(CommonImpl1));

			WaitCallback resolveThread = delegate
			                             	{
			                             		waitEvent.WaitOne();
			                             		while(threadCount > 0 && list.Count == 0)
			                             		{
			                             			try
			                             			{
			                             				ICommon common = kernel[typeof(ICommon)] as ICommon;
			                             			}
			                             			catch(Exception e)
			                             			{
			                             				list.Add(e);
			                             			}
			                             		}
			                             	};
			ThreadPool.QueueUserWorkItem(resolveThread);

			WaitCallback removeThread = delegate
			                            	{
			                            		waitEvent.WaitOne();
			                            		kernel.RemoveComponent(threadCount + ".common");
			                            		Interlocked.Decrement(ref threadCount);
			                            	};
			for(int i = 0; i < threadCount; i++)
			{
				kernel.AddComponent(i + ".common", typeof(ICommon), typeof(CommonImpl1));
				ThreadPool.QueueUserWorkItem(removeThread);
			}

			waitEvent.Set();
			while(threadCount > 0 && list.Count == 0)
			{
				Thread.Sleep(15);
			}

			if (list.Count > 0)
			{
				throw (Exception) list[0];
			}
		}
	}
}
