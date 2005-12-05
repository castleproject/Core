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

namespace Castle.MicroKernel.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.Resolvers;
	using Castle.MicroKernel.Tests.ClassComponents;

	[TestFixture]
	public class UnsatisfiedDependenciesTestCase
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
		[ExpectedException( typeof(HandlerException), "Can't create component 'key' as it has dependencies to be satisfied. \r\nWaiting for the following services: \r\n- Castle.MicroKernel.Tests.ClassComponents.ICommon \r\n" )]
		public void UnsatisfiedService()
		{
			kernel.AddComponent("key", typeof(CommonServiceUser));
			object instance = kernel["key"];
		}
		
		[Test]
		[ExpectedException( typeof(DependencyResolverException), "Could not resolve non-optional dependency for 'key' (Castle.MicroKernel.Tests.ClassComponents.CustomerImpl2). Parameter 'name' type 'System.String'" )]
		public void UnsatisfiedConfigValues()
		{
			MutableConfiguration config = new MutableConfiguration("component");
			
			MutableConfiguration parameters = (MutableConfiguration ) 
				config.Children.Add( new MutableConfiguration("parameters") );

			parameters.Children.Add( new MutableConfiguration("name", "hammett") );

			kernel.ConfigurationStore.AddComponentConfiguration("customer", config);

			kernel.AddComponent("key", typeof(CustomerImpl2));
			object instance = kernel["key"];
		}
	}
}
