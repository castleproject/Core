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

namespace Castle.Windsor.Tests
{
	using System;
	using Castle.Windsor.Configuration.Interpreters;
	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.Windsor.Tests.Components;


	[TestFixture]
	public class ChildContainerSupportTestCase
	{
		private IWindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer();						
			container.AddComponent("A", typeof(A));
		}

		[Test]
		public void ResolveAgainstParentContainer()
		{			
			IWindsorContainer childcontainer = new WindsorContainer();			
			container.AddChildContainer(childcontainer);
			
			Assert.AreEqual(container, childcontainer.Parent);
			
			childcontainer.AddComponent("B", typeof(B));
			B b = childcontainer["B"] as B;
			
			Assert.IsNotNull(b);	
		}

		[Test]
		public void AddAndRemoveChildContainer()
		{
			IWindsorContainer childcontainer = new WindsorContainer();			
			container.AddChildContainer(childcontainer);
			Assert.AreEqual(container, childcontainer.Parent);

			container.RemoveChildContainer(childcontainer);
			Assert.IsNull(childcontainer.Parent);

			container.AddChildContainer(childcontainer);
			Assert.AreEqual(container, childcontainer.Parent);
		}

		[Test]
		[ExpectedException(typeof(KernelException))]
		public void AddingToTwoParentContainsThrowsKernelException()
		{
			IWindsorContainer container3 = new WindsorContainer();
			IWindsorContainer childcontainer = new WindsorContainer();			
			container.AddChildContainer(childcontainer);
			container3.AddChildContainer(childcontainer);
		}


		[Test]
		public void StartWithParentContainer()
		{
			IWindsorContainer childcontainer = new WindsorContainer(container, new XmlInterpreter());
			
			Assert.AreEqual(container, childcontainer.Parent);

			childcontainer.AddComponent("B", typeof(B));
			B b = childcontainer["B"] as B;

			Assert.IsNotNull(b);
		}

		public class A
		{
			public A()
			{
			}
		}

		public class B
		{
			public B(A a)
			{
			}
		}
	}
}
