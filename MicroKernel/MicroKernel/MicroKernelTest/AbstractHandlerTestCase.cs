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

	using NUnit.Framework;

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Handler;
	using Castle.MicroKernel.Handler.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for AbstractHandlerTestCase.
	/// </summary>
	[TestFixture]
	public class AbstractHandlerTestCase : Assertion
	{
		private IKernel kernel = new DefaultAvalonKernel();
		private Type service = typeof( IMailService );
		private Type implementation = typeof( SimpleMailService );
		private IComponentModel model;

		[SetUp]
		public void Init()
		{
			model = new DefaultComponentModelBuilder(kernel).BuildModel( 
				"a", service, implementation );
		}

		[Test]
		public void TransientReferences()
		{
			MyHandler handler = new MyHandler( model );
			handler.Init( kernel );
			
			object instance = handler.Resolve();
			AssertNotNull( instance );
			
			Assert( handler.IsOwner( instance ) );

			handler.Release( instance );

			Assert( !handler.IsOwner( instance ) );
		}

		[Test]
		public void MultipleTransientReferences()
		{
			MyHandler handler = new MyHandler( model );
			handler.Init( kernel );
			
			object instance1 = handler.Resolve();
			object instance2 = handler.Resolve();
			object instance3 = handler.Resolve();
			
			AssertNotNull( instance1 );
			AssertNotNull( instance2 );
			AssertNotNull( instance3 );
			
			Assert( handler.IsOwner( instance1 ) );
			Assert( handler.IsOwner( instance2 ) );
			Assert( handler.IsOwner( instance3 ) );
			Assert( !handler.IsOwner( new object() ) );

			handler.Release( instance3 );
			Assert( !handler.IsOwner( instance3 ) );
			handler.Release( instance2 );
			Assert( !handler.IsOwner( instance2 ) );
			handler.Release( instance1 );
			Assert( !handler.IsOwner( instance1 ) );
		}

		public class MyHandler : AbstractHandler
		{
			public MyHandler( IComponentModel model ) : base( model )
			{
			}

			public override object Resolve()
			{
				object instance = new object();
				base.RegisterInstance( instance );
				return instance;
			}
		
			public override void Release(object instance)
			{
				if (IsOwner( instance ))
				{
					UnregisterInstance(instance);
				}
			}
		}

		[Test]
		public void SingletonReferences()
		{
			MySingletonHandler handler = new MySingletonHandler( model );
			handler.Init( kernel );
			
			object instance1 = handler.Resolve();
			object instance2 = handler.Resolve();
			AssertNotNull( instance1 );
			AssertNotNull( instance2 );
			AssertEquals( instance1, instance2 );
			
			Assert( handler.IsOwner( instance1 ) );
			Assert( handler.IsOwner( instance2 ) );

			handler.Release( instance1 );
			handler.Release( instance2 );

			Assert( !handler.IsOwner( instance1 ) );
			Assert( !handler.IsOwner( instance2 ) );
		}

		public class MySingletonHandler : AbstractHandler
		{
			public object instance = new object();

			public MySingletonHandler( IComponentModel model ) : base( model )
			{
			}

			public override object Resolve()
			{
				base.RegisterInstance( instance );
				return instance;
			}
		
			public override void Release(object instance)
			{
				if (IsOwner( instance ))
				{
					UnregisterInstance(instance);
				}
			}
		}
	}
}
