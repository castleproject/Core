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

namespace Castle.MicroKernel.Tests
{
	using NUnit.Framework;

	using Castle.Core.Configuration;

	using Castle.MicroKernel.Handlers;

	using Castle.MicroKernel.Tests.ClassComponents;

	/// <summary>
	/// Summary description for DependencyResolvers.
	/// </summary>
	[TestFixture]
	public class DependencyResolvers
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
		public void ResolvingPrimitivesThroughProperties()
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
			Assert.AreEqual("hammett", customer.Name);
			Assert.AreEqual("something", customer.Address);
			Assert.AreEqual(25, customer.Age);
		}

		[Test]
		public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamService) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) kernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		[ExpectedException(typeof(HandlerException))]
		public void UnresolvedDependencies()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];
		}

		[Test]
		public void FactoryPattern()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddComponent( "factory", typeof(ComponentFactory) );

			ComponentFactory factory = (ComponentFactory) kernel["factory"];

			Assert.IsNotNull(factory);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) factory.Create("spamservice");

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}
	}
}
