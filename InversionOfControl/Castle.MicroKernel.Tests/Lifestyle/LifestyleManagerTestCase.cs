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

namespace Castle.MicroKernel.Tests.Lifestyle
{
	using System;
	using System.Threading;
	using Castle.Model;
	using Castle.Model.Configuration;
	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Tests.Lifestyle.Components;

	/// <summary>
	/// Summary description for LifestyleManagerTestCase.
	/// </summary>
	[TestFixture]
	public class LifestyleManagerTestCase
	{
		private IKernel kernel;

		private IComponent instance3;

		[SetUp]
		public void CreateContainer()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void DisposeContainer()
		{
			kernel.Dispose();
		}

		[Test]
		public void TestTransient()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(TransientComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve() as IComponent;
			IComponent instance2 = handler.Resolve() as IComponent;

			Assert.IsNotNull( instance1 );
			Assert.IsNotNull( instance2 );

			Assert.IsTrue( !instance1.Equals( instance2 ) );
			Assert.IsTrue( instance1.ID != instance2.ID );

			handler.Release( instance1 );
			handler.Release( instance2 );
		}

		[Test]
		public void LifestyleSetThroughAttribute()
		{
			kernel.AddComponent( "a", typeof(TransientComponent) );
			IHandler handler = kernel.GetHandler("a");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);

			kernel.AddComponent( "b", typeof(SingletonComponent) );
			handler = kernel.GetHandler("b");
			Assert.AreEqual(LifestyleType.Singleton, handler.ComponentModel.LifestyleType);

			kernel.AddComponent( "c", typeof(CustomComponent) );
			handler = kernel.GetHandler("c");
			Assert.AreEqual(LifestyleType.Custom, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void LifestyleSetThroughExternalConfig()
		{
			IConfiguration confignode = new MutableConfiguration("component");
			confignode.Attributes.Add("lifestyle", "transient");
			kernel.ConfigurationStore.AddComponentConfiguration("a", confignode);
			kernel.AddComponent("a", typeof(NoInfoComponent));
			IHandler handler = kernel.GetHandler("a");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);

			confignode = new MutableConfiguration("component");
			confignode.Attributes.Add("lifestyle", "singleton");
			kernel.ConfigurationStore.AddComponentConfiguration("b", confignode);
			kernel.AddComponent("b", typeof(NoInfoComponent));
			handler = kernel.GetHandler("b");
			Assert.AreEqual(LifestyleType.Singleton, handler.ComponentModel.LifestyleType);

			confignode = new MutableConfiguration("component");
			confignode.Attributes.Add("lifestyle", "thread");
			kernel.ConfigurationStore.AddComponentConfiguration("c", confignode);
			kernel.AddComponent("c", typeof(NoInfoComponent));
			handler = kernel.GetHandler("c");
			Assert.AreEqual(LifestyleType.Thread, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void TestSingleton()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(SingletonComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve() as IComponent;
			IComponent instance2 = handler.Resolve() as IComponent;

			Assert.IsNotNull( instance1 );
			Assert.IsNotNull( instance2 );

			Assert.IsTrue( instance1.Equals( instance2 ) );
			Assert.IsTrue( instance1.ID == instance2.ID );

			handler.Release( instance1 );
			handler.Release( instance2 );
		}

		[Test]
		public void TestCustom()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(CustomComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve() as IComponent;

			Assert.IsNotNull( instance1 );
		}

		[Test]
		public void TestPerThread()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(PerThreadComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve() as IComponent;
			IComponent instance2 = handler.Resolve() as IComponent;

			Assert.IsNotNull( instance1 );
			Assert.IsNotNull( instance2 );

			Assert.IsTrue( instance1.Equals( instance2 ) );
			Assert.IsTrue( instance1.ID == instance2.ID );

			Thread thread = new Thread( new ThreadStart(OtherThread) );
			thread.Start();
			thread.Join();

			Assert.IsNotNull( this.instance3 );
			Assert.IsTrue( !instance1.Equals( this.instance3 ) );
			Assert.IsTrue( instance1.ID != this.instance3.ID );

			handler.Release( instance1 );
			handler.Release( instance2 );
		}

		private void OtherThread()
		{
			IHandler handler = kernel.GetHandler( "a" );
			this.instance3 = handler.Resolve() as IComponent;
		}
	}
}
