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

namespace Castle.MicroKernel.Tests.DependencyResolving
{
	using System;

	using NUnit.Framework;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.Tests.ClassComponents;
	using Castle.MicroKernel.Resolvers;


	/// <summary>
	/// This test case is used to demonstrate how to 
	/// block dependencies by changing the dependency.
	/// </summary>
	[TestFixture] public class BlockedDependencyWithClearing
	{

		private IKernel kernel;

		#region Setup / Teardown

		[SetUp] public void SetUp()
		{
			kernel = new DefaultKernel();
			kernel.Resolver.DependencyResolving += new DependancyDelegate(ClearDependency);
		}

		[TearDown] public void TearDown()
		{
			kernel.Dispose();
		}


		#endregion
		
		[Test] public void ResolvingPrimitivesThroughProperties()
		{
			MutableConfiguration config = new MutableConfiguration("component");
			
			MutableConfiguration parameters = (MutableConfiguration ) 
				config.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("name", "hammett") );
			parameters.Children.Add( new MutableConfiguration("address", "something") );
			parameters.Children.Add( new MutableConfiguration("age", "25") );

			kernel.ConfigurationStore.AddComponentConfiguration("customer", config);

			kernel.AddComponent( "customer", typeof(ICustomer), typeof(CustomerImpl) );

			ICustomer customer = (ICustomer) kernel["customer"];

			Assert.IsNotNull(customer);
			Assert.AreEqual(null, customer.Name);
			Assert.AreEqual(null, customer.Address);
			Assert.AreEqual(0, customer.Age);
		}

		[Test] public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamService) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];
			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);
			Assert.IsNotNull(spamservice);
			Assert.IsNull(spamservice.MailSender);
			Assert.IsNull(spamservice.TemplateEngine);
		}

		[ExpectedException(typeof(DependencyResolverException))]
		[Test] public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) kernel["spamservice"];

		}


		private void ClearDependency(Castle.Model.ComponentModel client, Castle.Model.DependencyModel model, ref object dependency)
		{
			dependency = null;
		}

	}


	/// <summary>
	/// This test case is used to demonstrate how to 
	/// block dependencies by throwing exceptions.
	/// </summary>
	[TestFixture] public class BlockedDependencyWithException
	{

		private IKernel kernel;

		#region Setup / Teardown

		[SetUp] public void SetUp()
		{
			kernel = new DefaultKernel();
			kernel.Resolver.DependencyResolving += new DependancyDelegate(ThrowException);
		}

		[TearDown] public void TearDown()
		{
			kernel.Dispose();
		}


		#endregion

		[ExpectedException(typeof(TestException))]
		[Test] public void ResolvingPrimitivesThroughProperties()
		{
			MutableConfiguration config = new MutableConfiguration("component");
			
			MutableConfiguration parameters = (MutableConfiguration ) 
				config.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("name", "hammett") );
			parameters.Children.Add( new MutableConfiguration("address", "something") );
			parameters.Children.Add( new MutableConfiguration("age", "25") );

			kernel.ConfigurationStore.AddComponentConfiguration("customer", config);

			kernel.AddComponent( "customer", typeof(ICustomer), typeof(CustomerImpl) );

			ICustomer customer = (ICustomer) kernel["customer"];
		}

		[ExpectedException(typeof(TestException))]
		[Test] public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamService) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];
		}

		[ExpectedException(typeof(TestException))]
		[Test] public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) kernel["spamservice"];

		}


		private void ThrowException(Castle.Model.ComponentModel client, Castle.Model.DependencyModel model, ref object dependency)
		{
			throw new TestException();
		}


		private class TestException : Exception 
		{
		}
	}


	/// <summary>
	/// This test case is used to demonstrate how to 
	/// change dependencies dynamically.
	/// </summary>
	[TestFixture] public class ChangingDependencies
	{

		private IKernel kernel;
		private DefaultMailSenderService defaultMailSenderService;
		private DefaultTemplateEngine defaultTemplateEngine;
		private DefaultSpamService defaultSpamService;
		private DefaultSpamServiceWithConstructor defaultSpamServiceWithConstructor;

		#region Setup / Teardown

		[TestFixtureSetUp] public void FixtureSetUp()
		{
			defaultMailSenderService = new TestMailSenderService();
			defaultTemplateEngine = new TestTemplateEngine();
			defaultSpamService = new TestSpamService(defaultMailSenderService, defaultTemplateEngine);
			defaultSpamServiceWithConstructor = new TestSpamServiceWithConstructor(defaultMailSenderService, defaultTemplateEngine);
		}

		[SetUp] public void SetUp()
		{
			kernel = new DefaultKernel();
			kernel.Resolver.DependencyResolving += new DependancyDelegate(ChangeDependency);
		}

		[TearDown] public void TearDown()
		{
			kernel.Dispose();
		}


		#endregion
		
		[Test] public void ResolvingPrimitivesThroughProperties()
		{
			MutableConfiguration config = new MutableConfiguration("component");
			
			MutableConfiguration parameters = (MutableConfiguration ) 
				config.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("name", "hammett") );
			parameters.Children.Add( new MutableConfiguration("address", "something") );
			parameters.Children.Add( new MutableConfiguration("age", "25") );

			kernel.ConfigurationStore.AddComponentConfiguration("customer", config);

			kernel.AddComponent( "customer", typeof(ICustomer), typeof(CustomerImpl) );

			ICustomer customer = (ICustomer) kernel["customer"];

			Assert.IsNotNull(customer);
			Assert.AreEqual("ttemmah", customer.Name);
			Assert.AreEqual("gnihtemos", customer.Address);
			Assert.AreEqual(-25, customer.Age);
		}

		[Test] public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamService) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];
			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];

			// Check that all services are returned properly
			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);
			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);

			// Check that the services we requested directly are not
			// the ones the event handler change them to.
			Assert.IsFalse(defaultMailSenderService == mailservice);
			Assert.IsFalse(defaultTemplateEngine == templateengine);
			Assert.IsFalse(defaultSpamService == spamservice);

			// Check that the services that was requested as dependencies
			// *were* changed to the ones the event handler uses.
			Assert.AreSame(defaultMailSenderService, spamservice.MailSender);
			Assert.AreSame(defaultTemplateEngine, spamservice.TemplateEngine);
		}

		[Test] public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];
			DefaultSpamServiceWithConstructor spamservice = (DefaultSpamServiceWithConstructor) kernel["spamservice"];

			// Check that all services are returned properly
			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);
			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);

			// Check that the services we requested directly are not
			// the ones the event handler change them to.
			Assert.IsFalse(defaultMailSenderService == mailservice);
			Assert.IsFalse(defaultTemplateEngine == templateengine);
			Assert.IsFalse(defaultSpamServiceWithConstructor == spamservice);

			// Check that the services that was requested as dependencies
			// *were* changed to the ones the event handler uses.
			Assert.AreSame(defaultMailSenderService, spamservice.MailSender);
			Assert.AreSame(defaultTemplateEngine, spamservice.TemplateEngine);

		}


		private void ChangeDependency(Castle.Model.ComponentModel client, Castle.Model.DependencyModel model, ref object dependency)
		{
			if (model.DependencyType == DependencyType.Parameter)
			{
				if (dependency is string)
				{
					if (model.DependencyKey.ToLower() == "name")
						dependency = "ttemmah";
					if (model.DependencyKey.ToLower() == "address")
						dependency = "gnihtemos";
				}
				else if (dependency is int)
				{
					dependency = -(int)dependency;
				}
			}
			else
			{
				if (model.TargetType == typeof(DefaultMailSenderService))
				{
					dependency = defaultMailSenderService;
				}
				else if (model.TargetType == typeof(DefaultTemplateEngine))
				{
					dependency = defaultTemplateEngine;
				}
				else if (model.TargetType == typeof(DefaultSpamService))
				{
					dependency = defaultSpamService;
				}
				else if (model.TargetType == typeof(DefaultSpamServiceWithConstructor))
				{
					dependency = defaultSpamServiceWithConstructor;
				}
			}
		}


		private class TestSpamService : DefaultSpamService
		{
			public TestSpamService(DefaultMailSenderService mailSender, DefaultTemplateEngine templateEngine)
			{
				base.MailSender = mailSender;
				base.TemplateEngine = templateEngine;
			}
		}
		private class TestSpamServiceWithConstructor : DefaultSpamServiceWithConstructor
		{
			public TestSpamServiceWithConstructor(DefaultMailSenderService mailSender, DefaultTemplateEngine templateEngine) : base(mailSender, templateEngine)
			{
			}
		}
		private class TestMailSenderService : DefaultMailSenderService
		{
		}
		private class TestTemplateEngine : DefaultTemplateEngine
		{
		}
	}


}
