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

namespace Castle.MicroKernel.Test.Model
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for DefaultComponentModelBuilderTestCase.
	/// </summary>
	[TestFixture]
	public class DefaultComponentModelBuilderTestCase : Assertion
	{
		private IKernel m_kernel;

		[SetUp]
		public void CreateKernel()
		{
			m_kernel = new BaseKernel();
		}

		[Test]
		public void SimpleComponent()
		{
			DefaultComponentModelBuilder builder = 
				new DefaultComponentModelBuilder( m_kernel );

			Type service = typeof( IMailService );
			Type implementation = typeof( SimpleMailService );

			IComponentModel model = 
				builder.BuildModel( "a", service, implementation );

			AssertNotNull( model );
			AssertNotNull( model.Logger );
			AssertNotNull( model.Name );
			AssertNotNull( model.Configuration );
			AssertNotNull( model.Context );
			AssertNotNull( model.Dependencies );
			AssertNotNull( model.Properties );

			AssertEquals( 0, model.Properties.Count );
			AssertEquals( 0, model.Dependencies.Length );
			AssertEquals( "a", model.Name );
			AssertEquals( Avalon.Framework.Lifestyle.Transient, model.SupportedLifestyle );
			AssertEquals( Avalon.Framework.Activation.Undefined, model.ActivationPolicy );
		}

		[Test]
		public void DependencyInConstructor()
		{
			DefaultComponentModelBuilder builder = 
				new DefaultComponentModelBuilder( m_kernel );

			Type service = typeof( ISpamService );
			Type implementation = typeof( SimpleSpamService );

			IComponentModel model = 
				builder.BuildModel( "a", service, implementation );

			AssertNotNull( model );
			AssertNotNull( model.Logger );
			AssertNotNull( model.Configuration );
			AssertNotNull( model.Context );
			AssertNotNull( model.Dependencies );
			AssertEquals( 1, model.Dependencies.Length );
		}

		[Test]
		public void DependencyInSetters()
		{
			DefaultComponentModelBuilder builder = 
				new DefaultComponentModelBuilder( m_kernel );

			Type service = typeof( IMailMarketingService );
			Type implementation = typeof( SimpleMailMarketingService );

			IComponentModel model = 
				builder.BuildModel( "a", service, implementation );

			AssertNotNull( model );
			AssertNotNull( model.Logger );
			AssertNotNull( model.Configuration );
			AssertNotNull( model.Context );
			AssertNotNull( model.Dependencies );
			AssertEquals( 2, model.Dependencies.Length );
		}

		[Test]
		public void AvalonSimpleService()
		{
			DefaultComponentModelBuilder builder = 
				new DefaultComponentModelBuilder( m_kernel );

			Type service = typeof( IMailService );
			Type implementation = typeof( AvalonMailService );

			IComponentModel model = 
				builder.BuildModel( "a", service, implementation );

			AssertEquals( Apache.Avalon.Framework.Lifestyle.Singleton, model.SupportedLifestyle );
			AssertNotNull( model );
			AssertNotNull( model.Logger );
			AssertNotNull( model.Configuration );
			AssertNotNull( model.Context );
			AssertNotNull( model.Dependencies );
			AssertEquals( 0, model.Dependencies.Length );
		}

		[Test]
		public void AvalonServiceWithDependencies()
		{
			DefaultComponentModelBuilder builder = 
				new DefaultComponentModelBuilder( m_kernel );

			Type service = typeof( ISpamService );
			Type implementation = typeof( AvalonSpamService );

			IComponentModel model = 
				builder.BuildModel( "a", service, implementation );

			AssertNotNull( model );
			AssertNotNull( model.Logger );
			AssertNotNull( model.Configuration );
			AssertNotNull( model.Context );
			AssertNotNull( model.Dependencies );
			AssertEquals( 1, model.Dependencies.Length );
		}
	}
}
