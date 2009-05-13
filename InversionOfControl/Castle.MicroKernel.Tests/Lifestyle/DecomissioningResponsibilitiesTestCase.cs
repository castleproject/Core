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

namespace Castle.MicroKernel.Tests.Lifestyle
{
	using System;
	using Castle.Core;
	using Castle.MicroKernel.Tests.ClassComponents;
	using Castle.MicroKernel.Tests.Pools;
	using MicroKernel.Lifestyle.Pool;
	using MicroKernel.Registration;
	using NUnit.Framework;
	using Releasers;

	[TestFixture]
	public class DecomissioningResponsibilitiesTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Setup()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void TearDown()
		{
			kernel.Dispose();
		}

		[Test]
		public void TransientReferencesAreNotHeldByContainer()
		{
			kernel.Register( Component.For<EmptyClass>().LifeStyle.Transient ); 
			var emptyClassWeakReference = new WeakReference(kernel.Resolve<EmptyClass>());
			
			GC.Collect();
			GC.WaitForPendingFinalizers();
			
			Assert.IsFalse(emptyClassWeakReference.IsAlive);
		}

		[Test]
		public void TransientReferencedComponentsAreReleasedInChain()
		{
			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);

			DisposableSpamService instance1 = (DisposableSpamService) kernel["spamservice"];
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.IsDisposed);
			Assert.IsTrue(instance1.TemplateEngine.IsDisposed);
		}

		[Test]
		public void DisposingSubLevelBurdenWontDisposeComponentAsTheyAreDisposedAlready()
		{
			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);

			DisposableSpamService instance1 = (DisposableSpamService)kernel["spamservice"];
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);

			kernel.ReleaseComponent(instance1);
			kernel.ReleaseComponent(instance1.TemplateEngine);
		}

		[Test]
		public void ComponentsAreOnlyDisposedOnce()
		{
			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);

			DisposableSpamService instance1 = (DisposableSpamService)kernel["spamservice"];
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);

			kernel.ReleaseComponent(instance1);
			kernel.ReleaseComponent(instance1);
			kernel.ReleaseComponent(instance1);
		}

		[Test]
		public void GenericTransientComponentsAreReleasedInChain()
		{
			kernel.AddComponent("gena", typeof(GenA<>), LifestyleType.Transient);
			kernel.AddComponent("genb", typeof(GenB<>), LifestyleType.Transient);

			GenA<string> instance1 = kernel.Resolve<GenA<string>>();
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.GenBField.IsDisposed);

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.IsDisposed);
			Assert.IsTrue(instance1.GenBField.IsDisposed);
		}

		[Test]
		public void SingletonReferencedComponentIsNotDisposed()
		{
			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
			kernel.AddComponent("mailsender", typeof(DefaultMailSenderService), LifestyleType.Singleton);
			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);

			DisposableSpamService instance1 = (DisposableSpamService)kernel["spamservice"];
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.IsDisposed);
			Assert.IsTrue(instance1.TemplateEngine.IsDisposed);
			Assert.IsFalse(instance1.MailSender.IsDisposed);
		}

		[Test]
		public void WhenRootComponentIsNotDisposableButDependenciesAre_DependenciesShouldBeDisposed()
		{
			kernel.AddComponent("root", typeof(NonDisposableRoot), LifestyleType.Transient);
			kernel.AddComponent("a", typeof(A), LifestyleType.Transient);
			kernel.AddComponent("b", typeof(B), LifestyleType.Transient);

			NonDisposableRoot instance1 = kernel.Resolve<NonDisposableRoot>();
			Assert.IsFalse(instance1.A.IsDisposed);
			Assert.IsFalse(instance1.B.IsDisposed);

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.A.IsDisposed);
			Assert.IsTrue(instance1.B.IsDisposed);
		}

		[Test]
		public void WhenRootComponentIsNotDisposableButThirdLevelDependenciesAre_DependenciesShouldBeDisposed()
		{
			kernel.AddComponent("root", typeof(Indirection), LifestyleType.Transient);
			kernel.AddComponent("secroot", typeof(NonDisposableRoot), LifestyleType.Transient);
			kernel.AddComponent("a", typeof(A), LifestyleType.Transient);
			kernel.AddComponent("b", typeof(B), LifestyleType.Transient);

			Indirection instance1 = kernel.Resolve<Indirection>();
			Assert.IsFalse(instance1.FakeRoot.A.IsDisposed);
			Assert.IsFalse(instance1.FakeRoot.B.IsDisposed);

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.FakeRoot.A.IsDisposed);
			Assert.IsTrue(instance1.FakeRoot.B.IsDisposed);
		}

//		[Test]
//		public void PooledComponentIsReleasedWhenRootComponentIsReleased()
//		{
//			kernel.AddComponentInstance("pool.fac", typeof(IPoolFactory), mockedPool);
//
//			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
//			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);
//			kernel.AddComponent("poolable", typeof(PoolableComponent1));
//
//			DisposableSpamService instance1 = (DisposableSpamService)kernel["spamservice"];
//			Assert.IsFalse(instance1.IsDisposed);
//			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);
//			PoolableComponent1 poolable = instance1.Pool;
//
//			kernel.ReleaseComponent(instance1);
//
//			// TODO: Assert that pool had its Release called
//		}

		public class Indirection
		{
			private NonDisposableRoot fakeRoot;

			public Indirection(NonDisposableRoot fakeRoot)
			{
				this.fakeRoot = fakeRoot;
			}

			public NonDisposableRoot FakeRoot
			{
				get { return fakeRoot; }
			}
		}

		public class NonDisposableRoot
		{
			private A a;
			private B b;

			public NonDisposableRoot(A a, B b)
			{
				this.a = a;
				this.b = b;
			}

			public A A
			{
				get { return a; }
			}

			public B B
			{
				get { return b; }
			}
		}

		public class A : DisposableBase
		{
		}

		public class B : DisposableBase
		{
		}

		public class C : DisposableBase
		{
		}

		public class GenA<T> : DisposableBase
		{
			private B bField;
			private GenB<T> genBField;

			public B BField
			{
				get { return bField; }
				set { bField = value; }
			}

			public GenB<T> GenBField
			{
				get { return genBField; }
				set { genBField = value; }
			}
		}

		public class GenB<T> : DisposableBase
		{

		}

		public class DisposableSpamService : DisposableBase
		{
			private DefaultMailSenderService mailSender;
			private DisposableTemplateEngine templateEngine;
			private PoolableComponent1 pool;

			public DisposableSpamService(DisposableTemplateEngine templateEngine)
			{
				this.templateEngine = templateEngine;
			}

			public DisposableSpamService(DisposableTemplateEngine templateEngine,
										 PoolableComponent1 pool)
			{
				this.templateEngine = templateEngine;
				this.pool = pool;
			}

			public DefaultMailSenderService MailSender
			{
				get { return mailSender; }
				set { mailSender = value; }
			}

			public DisposableTemplateEngine TemplateEngine
			{
				get { return templateEngine; }
			}

			public PoolableComponent1 Pool
			{
				get { return pool; }
			}
		}

		public class DisposableTemplateEngine : DisposableBase
		{
		}

		public class EmptyClass
		{

		}
	}
}