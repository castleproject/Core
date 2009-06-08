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
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

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

			MutableConfiguration parameters = new MutableConfiguration("parameters");
			config.Children.Add(parameters);

			parameters.Children.Add(new MutableConfiguration("name", "hammett"));
			parameters.Children.Add(new MutableConfiguration("address", "something"));
			parameters.Children.Add(new MutableConfiguration("age", "25"));

			kernel.ConfigurationStore.AddComponentConfiguration("customer", config);

			kernel.AddComponent("customer", typeof(ICustomer), typeof(CustomerImpl));

			ICustomer customer = (ICustomer) kernel["customer"];

			Assert.IsNotNull(customer);
			Assert.AreEqual("hammett", customer.Name);
			Assert.AreEqual("something", customer.Address);
			Assert.AreEqual(25, customer.Age);
		}

		[Test]
		public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent("spamservice", typeof(DefaultSpamService));
			kernel.AddComponent("mailsender", typeof(DefaultMailSenderService));
			kernel.AddComponent("templateengine", typeof(DefaultTemplateEngine));

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent("spamservice", typeof(DefaultSpamServiceWithConstructor));
			kernel.AddComponent("mailsender", typeof(DefaultMailSenderService));
			kernel.AddComponent("templateengine", typeof(DefaultTemplateEngine));

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
			kernel.AddComponent("spamservice", typeof(DefaultSpamServiceWithConstructor));
			kernel.AddComponent("templateengine", typeof(DefaultTemplateEngine));

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];
		}

		[Test]
		public void FactoryPattern()
		{
			kernel.AddComponent("spamservice", typeof(DefaultSpamServiceWithConstructor));
			kernel.AddComponent("mailsender", typeof(DefaultMailSenderService));
			kernel.AddComponent("templateengine", typeof(DefaultTemplateEngine));

			kernel.AddComponent("factory", typeof(ComponentFactory));

			ComponentFactory factory = (ComponentFactory) kernel["factory"];

			Assert.IsNotNull(factory);

			DefaultSpamServiceWithConstructor spamservice =
				(DefaultSpamServiceWithConstructor) factory.Create("spamservice");

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void DependencyChain()
		{
			kernel.AddComponent("Customer9", typeof(ICustomer), typeof(CustomerChain9));
			kernel.AddComponent("Customer8", typeof(ICustomer), typeof(CustomerChain8));
			kernel.AddComponent("Customer7", typeof(ICustomer), typeof(CustomerChain7));
			kernel.AddComponent("Customer6", typeof(ICustomer), typeof(CustomerChain6));
			kernel.AddComponent("Customer5", typeof(ICustomer), typeof(CustomerChain5));
			kernel.AddComponent("Customer4", typeof(ICustomer), typeof(CustomerChain4));
			kernel.AddComponent("Customer3", typeof(ICustomer), typeof(CustomerChain3));
			kernel.AddComponent("Customer2", typeof(ICustomer), typeof(CustomerChain2));
			kernel.AddComponent("Customer1", typeof(ICustomer), typeof(CustomerChain1));
			kernel.AddComponent("Customer", typeof(ICustomer), typeof(CustomerImpl));

			CustomerChain1 customer = (CustomerChain1) kernel[typeof(ICustomer)];
			Assert.IsInstanceOf(typeof(CustomerChain9), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain8), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain7), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain6), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain5), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain4), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain3), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain2), customer);
			customer = (CustomerChain1)customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerChain1), customer);
			ICustomer lastCustomer = customer.CustomerBase;
			Assert.IsInstanceOf(typeof(CustomerImpl), lastCustomer);
		}
	}
}
