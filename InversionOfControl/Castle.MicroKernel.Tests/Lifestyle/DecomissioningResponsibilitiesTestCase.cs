// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;

	[TestFixture, Explicit]
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
		public void ReferencedComponentsAreReleased()
		{
			kernel.AddComponent("spamservice", typeof(DisposableSpamService), LifestyleType.Transient);
			kernel.AddComponent("mailsender", typeof(DefaultMailSenderService), LifestyleType.Singleton);
			kernel.AddComponent("templateengine", typeof(DisposableTemplateEngine), LifestyleType.Transient);
			kernel.AddComponent("poolable", typeof(PoolableComponent1));

			DisposableSpamService instance1 = (DisposableSpamService) kernel["spamservice"];
			Assert.IsFalse(instance1.IsDisposed);
			Assert.IsFalse(instance1.TemplateEngine.IsDisposed);
			PoolableComponent1 poolable = instance1.Pool;

			kernel.ReleaseComponent(instance1);

			Assert.IsTrue(instance1.IsDisposed);
			Assert.IsTrue(instance1.TemplateEngine.IsDisposed);

			DisposableSpamService instance2 = (DisposableSpamService) kernel["spamservice"];
			Assert.IsFalse(instance2.IsDisposed);
			Assert.IsFalse(instance2.TemplateEngine.IsDisposed);
			Assert.AreSame(instance2.Pool, poolable);
		}
	}

	public class DisposableSpamService : IDisposable
	{
		private bool isDisposed = false;
		private DefaultMailSenderService mailSender;
		private DisposableTemplateEngine templateEngine;
		private PoolableComponent1 pool;

		public DisposableSpamService(DefaultMailSenderService mailsender, DisposableTemplateEngine templateEngine)
		{
			mailSender = mailsender;
			this.templateEngine = templateEngine;
		}

		public DisposableSpamService(DefaultMailSenderService mailSender, DisposableTemplateEngine templateEngine,
		                             PoolableComponent1 pool)
		{
			this.mailSender = mailSender;
			this.templateEngine = templateEngine;
			this.pool = pool;
		}

		public bool IsDisposed
		{
			get { return isDisposed; }
		}

		public DefaultMailSenderService MailSender
		{
			get { return mailSender; }
		}

		public DisposableTemplateEngine TemplateEngine
		{
			get { return templateEngine; }
		}

		public PoolableComponent1 Pool
		{
			get { return pool; }
		}

		public void Dispose()
		{
			isDisposed = true;
		}
	}

	public class DisposableTemplateEngine : DefaultTemplateEngine, IDisposable
	{
		private bool isDisposed = false;

		public bool IsDisposed
		{
			get { return isDisposed; }
		}

		public void Dispose()
		{
			isDisposed = true;
		}
	}
}