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
	using Castle.Core;
	using Castle.Core.Configuration;
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
			
			IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
			IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

			Assert.IsNotNull( instance1 );
			Assert.IsNotNull( instance2 );

			Assert.IsTrue( !instance1.Equals( instance2 ) );
			Assert.IsTrue( instance1.ID != instance2.ID );

			handler.Release( instance1 );
			handler.Release( instance2 );
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void BadLifestyleSetProgromatically()
		{
			kernel.AddComponent("a", typeof(IComponent), typeof(NoInfoComponent), LifestyleType.Undefined);
		}
		
		[Test]
		public void LifestyleSetProgromatically()
		{
			TestHandlersLifestyle(typeof(NoInfoComponent), LifestyleType.Transient, false);
			TestHandlersLifestyle(typeof(NoInfoComponent), LifestyleType.Singleton, false);
			TestHandlersLifestyle(typeof(NoInfoComponent), LifestyleType.Thread, false);
			TestHandlersLifestyle(typeof(NoInfoComponent), LifestyleType.Transient, false);
			TestHandlersLifestyle(typeof(NoInfoComponent), LifestyleType.PerWebRequest, false);

			TestHandlersLifestyleWithService(typeof(NoInfoComponent), LifestyleType.Transient, false);
			TestHandlersLifestyleWithService(typeof(NoInfoComponent), LifestyleType.Singleton, false);
			TestHandlersLifestyleWithService(typeof(NoInfoComponent), LifestyleType.Thread, false);
			TestHandlersLifestyleWithService(typeof(NoInfoComponent), LifestyleType.Transient, false);
			TestHandlersLifestyleWithService(typeof(NoInfoComponent), LifestyleType.PerWebRequest, false);

			TestLifestyleAndSameness(typeof(PerThreadComponent), LifestyleType.Transient, true, false);
			TestLifestyleAndSameness(typeof(SingletonComponent), LifestyleType.Transient, true, false);
			TestLifestyleAndSameness(typeof(TransientComponent), LifestyleType.Singleton, true, true);

			TestLifestyleWithServiceAndSameness(typeof(PerThreadComponent), LifestyleType.Transient, true, false);
			TestLifestyleWithServiceAndSameness(typeof(SingletonComponent), LifestyleType.Transient, true, false);
			TestLifestyleWithServiceAndSameness(typeof(TransientComponent), LifestyleType.Singleton, true, true);
		}
		
		private void TestLifestyleAndSameness(Type componentType, LifestyleType lifestyle, bool overwrite, bool areSame)
		{
			string key = TestHandlersLifestyle(componentType, lifestyle, overwrite);
			TestSameness(key, areSame);
		}
		
		private void TestLifestyleWithServiceAndSameness(Type componentType, LifestyleType lifestyle, bool overwrite, bool areSame)
		{
			string key = TestHandlersLifestyleWithService(componentType, lifestyle, overwrite);
			TestSameness(key, areSame);
		}
		
		private void TestSameness(string key, bool areSame)
		{
			IComponent one = kernel[key] as IComponent;
			IComponent two = kernel[key] as IComponent;
			if(areSame)
			{
				Assert.AreSame(one, two);
			}
			else
			{
				Assert.AreNotSame(one, two);
			}
		}
		
		private string TestHandlersLifestyle(Type componentType, LifestyleType lifestyle, bool overwrite)
		{
			string key = Guid.NewGuid().ToString();
			kernel.AddComponent(key, componentType, lifestyle, overwrite);
			IHandler handler = kernel.GetHandler(key);
			Assert.AreEqual(lifestyle, handler.ComponentModel.LifestyleType);
			return key;
		}

		private string TestHandlersLifestyleWithService(Type componentType, LifestyleType lifestyle, bool overwrite)
		{
			string key = Guid.NewGuid().ToString();
			kernel.AddComponent(key, typeof(IComponent), componentType, lifestyle, overwrite);
			IHandler handler = kernel.GetHandler(key);
			Assert.AreEqual(lifestyle, handler.ComponentModel.LifestyleType);
			return key;
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

			kernel.AddComponent("d", typeof(PerWebRequestComponent));
			handler = kernel.GetHandler("d");
			Assert.AreEqual(LifestyleType.PerWebRequest, handler.ComponentModel.LifestyleType);
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

			confignode = new MutableConfiguration("component");
			confignode.Attributes.Add("lifestyle", "perWebRequest");
			kernel.ConfigurationStore.AddComponentConfiguration("d", confignode);
			kernel.AddComponent("d", typeof(NoInfoComponent));
			handler = kernel.GetHandler("d");
			Assert.AreEqual(LifestyleType.PerWebRequest, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void TestSingleton()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(SingletonComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
			IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

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
			
			IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;

			Assert.IsNotNull( instance1 );
		}

		[Test]
		public void TestPerThread()
		{
			kernel.AddComponent( "a", typeof(IComponent), typeof(PerThreadComponent) );

			IHandler handler = kernel.GetHandler("a");
			
			IComponent instance1 = handler.Resolve(CreationContext.Empty) as IComponent;
			IComponent instance2 = handler.Resolve(CreationContext.Empty) as IComponent;

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
			this.instance3 = handler.Resolve(CreationContext.Empty) as IComponent;
		}
	}
}
