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

namespace Castle.Facilities.TypedFactory.Tests
{
	using System;

	using Castle.Windsor;

	using Castle.Facilities.TypedFactory.Tests.Components;
	using Castle.Facilities.TypedFactory.Tests.Factories;

	using NUnit.Framework;

	[TestFixture]
	public class ExternalConfigurationTestCase
	{
		private IWindsorContainer _container;

		[SetUp]
		public void Init()
		{
			_container = new WindsorContainer( @"..\typedFactory_castle_config.xml" );
			
			_container.AddFacility( "typedfactory", new TypedFactoryFacility() );

			_container.AddComponent( "miranda", typeof(IProtocolHandler), typeof(MirandaProtocolHandler) );
			_container.AddComponent( "messenger", typeof(IProtocolHandler), typeof(MessengerProtocolHandler) );
			_container.AddComponent( "comp1", typeof(IDummyComponent), typeof(Component1) );
			_container.AddComponent( "comp2", typeof(IDummyComponent), typeof(Component2) );
		}

		[TearDown]
		public void Finish()
		{
			_container.Dispose();
		}

		[Test]
		public void Factory1()
		{
			IProtocolHandlerFactory1 factory = 
				(IProtocolHandlerFactory1) _container["protocolFac1"];

			Assert.IsNotNull( factory );
			
			IProtocolHandler handler = factory.Create();

			Assert.IsNotNull( handler );

			factory.Release( handler );
		}

		[Test]
		public void Factory2()
		{
			IProtocolHandlerFactory2 factory = 
				(IProtocolHandlerFactory2) _container["protocolFac2"];

			Assert.IsNotNull( factory );
			
			IProtocolHandler handler = factory.Create( "miranda" );
			Assert.IsNotNull( handler );
			Assert.IsTrue( handler is MirandaProtocolHandler );
			factory.Release( handler );

			handler = factory.Create( "messenger" );
			Assert.IsNotNull( handler );
			Assert.IsTrue( handler is MessengerProtocolHandler );
			factory.Release( handler );
		}

		[Test]
		public void Factory3()
		{
			IComponentFactory1 factory = 
				(IComponentFactory1) _container["compFactory1"];

			Assert.IsNotNull( factory );
			
			IDummyComponent comp1 = factory.Construct();
			Assert.IsNotNull( comp1 );

			IDummyComponent comp2 = factory.Construct();
			Assert.IsNotNull( comp2 );
		}

		[Test]
		public void Factory4()
		{
			IComponentFactory2 factory = 
				(IComponentFactory2) _container["compFactory2"];

			Assert.IsNotNull( factory );
			
			IDummyComponent comp1 = (IDummyComponent) factory.Construct("comp1");
			Assert.IsTrue( comp1 is Component1 );
			Assert.IsNotNull( comp1 );

			IDummyComponent comp2 = (IDummyComponent) factory.Construct("comp2");
			Assert.IsTrue( comp2 is Component2 );
			Assert.IsNotNull( comp2 );
		}
	}
}
