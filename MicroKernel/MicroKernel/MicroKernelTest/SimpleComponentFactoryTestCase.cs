// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test
{
	using System;
	using System.Reflection;
	using System.Collections;

	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Factory;
	using Castle.MicroKernel.Factory.Default;
	using Castle.MicroKernel.Handler;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for SimpleComponentFactoryTestCase.
	/// </summary>
	[TestFixture]
	public class SimpleComponentFactoryTestCase : Assertion
	{
		private BaseKernel kernel;

		[SetUp]
		public void CreateKernel()
		{
			kernel = new BaseKernel();
		}

		[Test]
		public void NoArgumentsConstructor()
		{
			Type service = typeof( IMailService );
			Type implementation = typeof( SimpleMailService );

			kernel.AddComponent( "a", service, implementation );

			IComponentModel model = new DefaultComponentModelBuilder(kernel).BuildModel( "a", service, implementation );

			BaseComponentFactory factory = new BaseComponentFactory( 
				kernel, null, 
				model, new Hashtable() );

			Object instance = factory.Incarnate();

			AssertNotNull( instance );
			AssertNotNull( instance as IMailService );

			factory.Etherialize( instance );
		}

		[Test]
		public void DependencyInConstructor()
		{
			Type service = typeof( ISpamService );
			Type implementation = typeof( SimpleSpamService );
			Type serviceDep = typeof( IMailService );
			Type implementationDep = typeof( SimpleMailService );

			kernel.AddComponent( "a", service, implementation );
			kernel.AddComponent( "b", serviceDep, implementationDep );

			IComponentModel model = new DefaultComponentModelBuilder(kernel).BuildModel( 
				"a", service, implementation );

			Hashtable serv2Handler = new Hashtable();
			serv2Handler[ serviceDep ] = kernel.GetHandlerForService( serviceDep );

			BaseComponentFactory factory = new BaseComponentFactory( 
				kernel, null, model, serv2Handler );

			Object instance = factory.Incarnate();

			AssertNotNull( instance );
			AssertNotNull( instance as ISpamService );
			
			SimpleSpamService spamService = (SimpleSpamService) instance;
			AssertNotNull( spamService.m_mailService );

			factory.Etherialize( instance );
		}

		[Test]
		public void DependencyInSetMethods()
		{
			Type service = typeof( IMailMarketingService );
			Type implementation = typeof( SimpleMailMarketingService );
			Type serviceDep1 = typeof( IMailService );
			Type implementationDep1 = typeof( SimpleMailService );
			Type serviceDep2 = typeof( ICustomerManager );
			Type implementationDep2 = typeof( SimpleCustomerManager );

			kernel.AddComponent( "a", service, implementation );
			kernel.AddComponent( "b", serviceDep1, implementationDep1 );
			kernel.AddComponent( "c", serviceDep2, implementationDep2 );

			IComponentModel model = new DefaultComponentModelBuilder(kernel).BuildModel( 
				"a", service, implementation );

			Hashtable serv2Handler = new Hashtable();
			serv2Handler[ serviceDep1 ] = kernel.GetHandlerForService( serviceDep1 );
			serv2Handler[ serviceDep2 ] = kernel.GetHandlerForService( serviceDep2 );

			BaseComponentFactory factory = new BaseComponentFactory( 
				kernel, null, model, serv2Handler );

			Object instance = factory.Incarnate();

			AssertNotNull( instance );
			AssertNotNull( instance as IMailMarketingService );
			
			SimpleMailMarketingService mailMarketing = (SimpleMailMarketingService) instance;
			AssertNotNull( mailMarketing.m_mailService );
			AssertNotNull( mailMarketing.m_customerManager );

			mailMarketing.AnnoyMillionsOfPeople( "Say something" );

			factory.Etherialize( instance );
		}
	}
}
