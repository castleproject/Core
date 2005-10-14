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
	using System.Collections;

	using NUnit.Framework;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.Tests.ClassComponents;


	/// <summary>
	/// This test case ensures that the resolving event
	/// is fired properly.
	/// </summary>
	[TestFixture] 
	public class EventTests
	{
		private IKernel kernel;

		private Castle.Model.ComponentModel expectedClient;
		private IList expectedModels;

		#region Setup / Teardown

		[SetUp] public void SetUp()
		{
			kernel = new DefaultKernel();
			kernel.DependencyResolving += new DependencyDelegate(AssertEvent);
		}

		[TearDown] public void TearDown()
		{
			kernel.Dispose();
		}

		#endregion

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

			expectedClient = kernel.GetHandler("customer").ComponentModel;
			expectedModels = new ArrayList();
			foreach (PropertySet prop in kernel.GetHandler("customer").ComponentModel.Properties)
			{
				expectedModels.Add(prop.Dependency);
			}

			ICustomer customer = (ICustomer) kernel["customer"];

			Assert.IsNotNull(customer);
		}

		[Test] 
		public void ResolvingConcreteClassThroughProperties()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamService) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);

			expectedClient = kernel.GetHandler("spamservice").ComponentModel;
			expectedModels = new ArrayList();
			foreach (PropertySet prop in kernel.GetHandler("spamservice").ComponentModel.Properties)
			{
				expectedModels.Add(prop.Dependency);
			}

			DefaultSpamService spamservice = (DefaultSpamService) kernel["spamservice"];

			Assert.IsNotNull(spamservice);
		}

		[Test] 
		public void ResolvingConcreteClassThroughConstructor()
		{
			kernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );
			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			DefaultMailSenderService mailservice = (DefaultMailSenderService) kernel["mailsender"];
			DefaultTemplateEngine templateengine = (DefaultTemplateEngine) kernel["templateengine"];

			Assert.IsNotNull(mailservice);
			Assert.IsNotNull(templateengine);

			expectedClient = kernel.GetHandler("spamservice").ComponentModel;
			expectedModels = new ArrayList(kernel.GetHandler("spamservice").ComponentModel.Constructors.FewerArgumentsCandidate.Dependencies);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) kernel["spamservice"];

			Assert.IsNotNull(spamservice);
		}

		private void AssertEvent(Castle.Model.ComponentModel client, Castle.Model.DependencyModel model, object dependency)
		{
			bool ok = false;
			Assert.AreEqual(expectedClient, client);
			foreach (DependencyModel expectedModel in expectedModels)
			{
				if (expectedModel.Equals(model))
				{
					ok = true;
					break;
				}
			}
			Assert.IsTrue(ok);
			Assert.IsNotNull(dependency);			
		}
	}
}
