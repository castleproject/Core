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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Threading;

	using Castle.MonoRail.Framework.Internal;
	using NUnit.Framework;


	[TestFixture]
	public class DescriptorBuilderMultiThreadedTestCase
	{
		private ManualResetEvent startEvent = new ManualResetEvent(false);
		private ManualResetEvent stopEvent = new ManualResetEvent(false);

		private ControllerDescriptorBuilder builder;

		[SetUp]
		public void Init()
		{
			builder = new ControllerDescriptorBuilder();
		}

		[Test]
		public void MultithreadTest()
		{
			const int threadCount = 10;

			Thread[] threads = new Thread[threadCount];
			
			for(int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new ThreadStart(ExecuteMethodUntilSignal));
				threads[i].Start();
			}

			startEvent.Set();

			Thread.CurrentThread.Join(1 * 2000);

			stopEvent.Set();
		}

		public void ExecuteMethodUntilSignal()
		{
			startEvent.WaitOne(int.MaxValue, false);

			while (!stopEvent.WaitOne(1, false))
			{
				ControllerMetaDescriptor desc1 = builder.BuildDescriptor( new Controller1() );
				ControllerMetaDescriptor desc2 = builder.BuildDescriptor( new Controller2() );

				Assert.AreEqual( 0, desc1.ActionProviders.Count );
				Assert.AreEqual( 1, desc1.Filters.Count );
				Assert.IsNotNull( desc1.Layout );
				Assert.IsNotNull( desc1.Rescues );

				Assert.AreEqual( 0, desc2.ActionProviders.Count );
				Assert.AreEqual( 1, desc2.Filters.Count );
				Assert.IsNotNull( desc2.Layout );
				Assert.IsNotNull( desc2.Rescues );

				ActionMetaDescriptor ac1 = desc1.GetAction( typeof(Controller1).GetMethod("Index") );
				Assert.IsNotNull( ac1.SkipRescue );
				Assert.AreEqual( 1, ac1.SkipFilters.Count );

				ActionMetaDescriptor ac2 = desc2.GetAction( typeof(Controller2).GetMethod("Index", new Type[] { typeof(int) } ) );
				Assert.IsNotNull( ac2.SkipRescue );
				Assert.AreEqual( 1, ac2.SkipFilters.Count );

				ActionMetaDescriptor ac3 = desc2.GetAction( typeof(Controller2).GetMethod("Index", new Type[] { typeof(String) } ) );
				Assert.IsNotNull( ac3.SkipRescue );
				Assert.AreEqual( 0, ac3.SkipFilters.Count );
			}
		}
	}

	[Filter( ExecuteEnum.AfterAction, typeof(MyFilter) )]
	[Rescue("Controller1rescue")]
	[Layout("Controller1layout")]
	internal class Controller1 : Controller
	{
		[SkipRescue, SkipFilter]
		public void Index()
		{
		}
	}

	[Filter( ExecuteEnum.AfterAction, typeof(MyFilter) )]
	[Rescue("Controller1rescue")]
	[Layout("Controller1layout")]
	internal class Controller2 : SmartDispatcherController
	{
		[SkipRescue, SkipFilter]
		public void Index(int id)
		{
		}	

		[SkipRescue]
		public void Index(String name)
		{
		}	
	}

	internal class MyFilter : IFilter
	{
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			return true;
		}
	}
}
