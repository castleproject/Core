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

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Assemble;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for AssemblerTestCase.
	/// </summary>
	[TestFixture]
	public class AssemblerTestCase : Assertion
	{
		private IAvalonKernel m_kernel = new DefaultAvalonKernel();

		private DefaultComponentModelBuilder m_builder;

		[SetUp]
		public void CreateComponentModelBuilder()
		{
			m_builder = new DefaultComponentModelBuilder( m_kernel );
		}

		[Test]
		public void BuildingConstructorArgsWithDefaultConstructor()
		{
			Type service = typeof( IMailService );
			Type implementation = typeof( SimpleMailService );

			IComponentModel model = m_builder.BuildModel( "a", service, implementation );

			object[] args = Assembler.BuildConstructorArguments( 
				model, new object(), new ResolveTypeHandler(Resolver) );

			AssertNotNull( args );
			AssertEquals( 0, args.Length );
		}

		[Test]
		public void BuildingConstructorArgsWithLoggerConstructor()
		{
			Type service = typeof( IMailService );
			Type implementation = typeof( SimpleMailServiceWithLogger );

			IComponentModel model = m_builder.BuildModel( "a", service, implementation );

			object[] args = Assembler.BuildConstructorArguments( 
				model, new object(), new ResolveTypeHandler(Resolver) );

			AssertNotNull( args );
			AssertEquals( 1, args.Length );
			Assert( typeof(ILogger).IsAssignableFrom( args[0].GetType() ) );
		}

		[Test]
		public void BuildingConstructorArgsWithDependencyConstructor()
		{
			Type service = typeof( ISpamService );
			Type implementation = typeof( SimpleSpamService );

			IComponentModel model = m_builder.BuildModel( "a", service, implementation );

			object[] args = Assembler.BuildConstructorArguments( 
				model, new object(), new ResolveTypeHandler(Resolver) );

			AssertNotNull( args );
			AssertEquals( 1, args.Length );
			Assert( typeof(IMailService).IsAssignableFrom( args[0].GetType() ) );
		}

		[Test]
		public void IsKnown()
		{
			Assert( Assembler.IsKnown( typeof(ILogger), "arg" ) );
			Assert( !Assembler.IsKnown( typeof(IMailService), "arg" ) );
		}

		[Test]
		public void AssembleProperties()
		{
			Type service = typeof( ISpamService2 );
			Type implementation = typeof( AvalonSpamService3 );

			object instance = Activator.CreateInstance( implementation );

			IComponentModel model = m_builder.BuildModel( "a", service, implementation );

			Assembler.AssembleProperties( instance, model, new object(), new ResolveTypeHandler(Resolver) );

			ISpamService2 serviceInstance = instance as ISpamService2;

			AssertNotNull( serviceInstance );
			Assert( typeof(IMailService).IsAssignableFrom( serviceInstance.MailService.GetType() ) );
		}

		private void Resolver(
			IComponentModel model, Type typeRequest, String argumentOrPropertyName, 
			object key, out object value )
		{
			AssertEquals( typeof(IMailService), typeRequest );
			value = new SimpleMailService();
		}
	}
}
