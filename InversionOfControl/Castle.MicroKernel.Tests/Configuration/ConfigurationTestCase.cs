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

namespace Castle.MicroKernel.Tests.Configuration
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.MicroKernel.Resolvers;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.MicroKernel.Tests.Configuration.Components;
	using Castle.MicroKernel.Tests.ClassComponents;

	/// <summary>
	/// Summary description for ConfigurationTestCase.
	/// </summary>
	[TestFixture]
	public class ConfigurationTestCase
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
		[ExpectedException(typeof(DependecyResolverException))]
		public void ConstructorWithUnsatisfiedParameters()
		{
			kernel.AddComponent( "key", typeof(ClassWithConstructors) );
			object res = kernel["key"];
		}

		[Test]
		public void ConstructorWithStringParameters()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");
			
			IConfiguration parameters = 
				confignode.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("host", "castleproject.org") );

			kernel.ConfigurationStore.AddComponentConfiguration( "key", confignode );

			kernel.AddComponent( "key", typeof(ClassWithConstructors) );
			
			ClassWithConstructors instance = (ClassWithConstructors) kernel["key"];
			Assert.IsNotNull( instance );
			Assert.IsNotNull( instance.Host );
			Assert.AreEqual( "castleproject.org", instance.Host );
		}

		[Test]
		public void ServiceOverride()
		{
			MutableConfiguration confignode = new MutableConfiguration("key");
			
			IConfiguration parameters = 
			 	confignode.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("common", "#{commonservice2}") );

			kernel.ConfigurationStore.AddComponentConfiguration( "commonserviceuser", confignode );

			kernel.AddComponent( "commonservice1", typeof(ICommon), typeof(CommonImpl1) );
			kernel.AddComponent( "commonservice2", typeof(ICommon), typeof(CommonImpl2) );

			kernel.AddComponent( "commonserviceuser", typeof(CommonServiceUser) );

			CommonServiceUser instance = (CommonServiceUser) kernel["commonserviceuser"];

			Assert.IsNotNull(instance);
			Assert.AreEqual( typeof(CommonImpl2), instance.CommonService.GetType() );
		}
	}
}
