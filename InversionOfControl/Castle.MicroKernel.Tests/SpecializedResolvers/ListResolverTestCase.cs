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

namespace Castle.MicroKernel.Tests.SpecializedResolvers
{
	using System.Collections.Generic;
	using Castle.Core;
	using System.Linq;
	using Core.Configuration;
	using MicroKernel.Registration;
	using NUnit.Framework;
	using Resolvers.SpecializedResolvers;

	[TestFixture]
	public class ListResolverTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
			kernel.Resolver.AddSubResolver(new ListResolver(kernel));
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void DependencyOnListOfServices_OnProperty()
		{
			kernel.Register(Component.For<IService>().ImplementedBy<A>(),
							Component.For<IService>().ImplementedBy<B>(),
							Component.For<CollectionDepAsProperty>());

			var comp = kernel.Resolve<CollectionDepAsProperty>();

			Assert.IsNotNull(comp);
			Assert.IsNotNull(comp.Services);
			Assert.AreEqual(2, comp.Services.Count);
			comp.Services.AsEnumerable().ForEach(Assert.IsNotNull);
		}

		[Test]
		public void DependencyOnListOfServices_OnConstructor()
		{
			kernel.Register(Component.For<IService>().ImplementedBy<A>(),
							Component.For<IService>().ImplementedBy<B>(),
							Component.For<CollectionDepAsConstructor>());

			var comp = kernel.Resolve<CollectionDepAsConstructor>();

			Assert.IsNotNull(comp);
			Assert.IsNotNull(comp.Services);
			Assert.AreEqual(2, comp.Services.Count);
			comp.Services.AsEnumerable().ForEach(Assert.IsNotNull);
		}

		public class CollectionDepAsProperty
		{
			public IList<IService> Services { get; set; }
		}

		public class CollectionDepAsConstructor
		{
			private readonly IList<IService> services;

			public CollectionDepAsConstructor(IList<IService> services)
			{
				this.services = services;
			}

			public IList<IService> Services
			{
				get { return services; }
			}
		}

		public interface IService
		{
		}

		public class A : IService
		{
		}

		public class B : IService
		{
		}
	}
}
