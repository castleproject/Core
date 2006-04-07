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

namespace Castle.MicroKernel.Tests.SubContainers
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.MicroKernel.Tests.ClassComponents;

	/// <summary>
	/// Summary description for SubContainersTestCase.
	/// </summary>
	[TestFixture]
	public class SubContainersTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void DependenciesSatisfiedAmongContainers()
		{
			IKernel subkernel = new DefaultKernel();

			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);

			subkernel.AddComponent( "spamservice", typeof(DefaultSpamService) );

			DefaultSpamService spamservice = (DefaultSpamService) subkernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void DependenciesSatisfiedAmongContainersUsingEvents()
		{
			IKernel subkernel = new DefaultKernel();

			subkernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );

			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) subkernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void ChildKernelFindsAndCreateParentComponent()
		{
			IKernel subkernel = new DefaultKernel();

			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);


			Assert.IsTrue(subkernel.HasComponent(typeof(DefaultTemplateEngine)));
			Assert.IsNotNull(subkernel[typeof(DefaultTemplateEngine)]);

		}

		[Test]
		[ExpectedException(typeof(ComponentNotFoundException))]
		public void ParentKernelFindsAndCreateChildComponent()
		{
			IKernel subkernel = new DefaultKernel();

			subkernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);


			Assert.IsFalse(kernel.HasComponent(typeof(DefaultTemplateEngine)));
			object engine = kernel[typeof(DefaultTemplateEngine)];
		}

		[Test]
		public void ChildKernelOverloadsParentKernel1()
		{
			DefaultTemplateEngine instance1 = new DefaultTemplateEngine();
			DefaultTemplateEngine instance2 = new DefaultTemplateEngine();

			// subkernel added with already registered components that overload parent components.

			IKernel subkernel = new DefaultKernel();
			subkernel.AddComponentInstance("engine", instance1);
			Assert.AreEqual(instance1, subkernel["engine"]);

			kernel.AddComponentInstance("engine", instance2);
			Assert.AreEqual(instance2, kernel["engine"]);

			kernel.AddChildKernel(subkernel);
			Assert.AreEqual(instance1, subkernel["engine"]);			
			Assert.AreEqual(instance2, kernel["engine"]);
		}

		[Test]
		public void ChildKernelOverloadsParentKernel2()
		{
			DefaultTemplateEngine instance1 = new DefaultTemplateEngine();
			DefaultTemplateEngine instance2 = new DefaultTemplateEngine();

			IKernel subkernel = new DefaultKernel();
			kernel.AddChildKernel(subkernel);

			// subkernel added first, then populated with overloaded components after

			kernel.AddComponentInstance("engine", instance2);
			Assert.AreEqual(instance2, kernel["engine"]);
			Assert.AreEqual(instance2, subkernel["engine"]);

			subkernel.AddComponentInstance("engine", instance1);
			Assert.AreEqual(instance1, subkernel["engine"]);
			Assert.AreEqual(instance2, kernel["engine"]);
		}

		[Test]
		public void RemoveChildKernelCleansUp()
		{					
			IKernel subkernel = new DefaultKernel();
			EventsCollector eventCollector = new EventsCollector(subkernel);
			subkernel.RemovedAsChildKernel += new EventHandler(eventCollector.RemovedAsChildKernel);
			subkernel.AddedAsChildKernel += new EventHandler(eventCollector.AddedAsChildKernel);
			
			kernel.AddChildKernel(subkernel);
			Assert.AreEqual(kernel, subkernel.Parent);
			Assert.AreEqual(1, eventCollector.Events.Count);
			Assert.AreEqual(EventsCollector.Added, eventCollector.Events[0]);
			
			kernel.RemoveChildKernel(subkernel);
			Assert.IsNull(subkernel.Parent);			
			Assert.AreEqual(2, eventCollector.Events.Count);
			Assert.AreEqual(EventsCollector.Removed, eventCollector.Events[1]);		
		}

		[Test]
		[ExpectedException(typeof(KernelException), "You can not change the kernel parent once set, use the RemoveChildKernel and AddChildKernel methods together to achieve this.")]
		public void AddChildKernelToTwoParentsThrowsException()
		{
			IKernel kernel2 = new DefaultKernel();

			IKernel subkernel = new DefaultKernel();			

			kernel.AddChildKernel(subkernel);
			Assert.AreEqual(kernel, subkernel.Parent);

			kernel2.AddChildKernel(subkernel);
		}

		[Test]
		public void RemovingChildKernelUnsubscribesFromParentEvents()
		{
			IKernel subkernel = new DefaultKernel();
			EventsCollector eventCollector = new EventsCollector(subkernel);
			subkernel.RemovedAsChildKernel += new EventHandler(eventCollector.RemovedAsChildKernel);
			subkernel.AddedAsChildKernel += new EventHandler(eventCollector.AddedAsChildKernel);
			
			kernel.AddChildKernel(subkernel);
			kernel.RemoveChildKernel(subkernel);
			kernel.AddChildKernel(subkernel);
			kernel.RemoveChildKernel(subkernel);

			Assert.AreEqual(4, eventCollector.Events.Count);
			Assert.AreEqual(EventsCollector.Added, eventCollector.Events[0]);
			Assert.AreEqual(EventsCollector.Removed, eventCollector.Events[1]);
			Assert.AreEqual(EventsCollector.Added, eventCollector.Events[2]);
			Assert.AreEqual(EventsCollector.Removed, eventCollector.Events[3]);			
		}

		/// <summary>
		/// collects events in an array list, used for ensuring we are cleaning up the parent kernel
		/// event subscriptions correctly.
		/// </summary>
		private class EventsCollector
		{
			public const string Added = "added";
			public const string Removed = "removed";						
			
			private ArrayList events;
			private object expectedSender;
		
			public ArrayList Events
			{
				get { return events; }
			}

			public EventsCollector(object expectedSender)
			{
				this.expectedSender = expectedSender;
				this.events = new ArrayList();
			}
			
			public void AddedAsChildKernel(object sender, EventArgs e)
			{
				Assert.AreEqual(expectedSender, sender);
				events.Add(Added);
			}

			public void RemovedAsChildKernel(object sender, EventArgs e)
			{
				Assert.AreEqual(expectedSender, sender);
				events.Add(Removed);
			}
		}
	}
}
