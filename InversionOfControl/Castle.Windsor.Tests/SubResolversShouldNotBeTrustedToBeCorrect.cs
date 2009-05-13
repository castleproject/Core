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

namespace Castle.Windsor.Tests
{
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Exceptions;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Resolvers;
	using NUnit.Framework;

	[TestFixture]
	public class SubResolversShouldNotBeTrustedToBeCorrect
	{
		public interface IItemService
		{
		}

		public class ItemService : IItemService
		{
			private IBookStore bookStore;

			public ItemService(IBookStore bookStore)
			{
				this.bookStore = bookStore;
			}
		}

		public interface IBookStore
		{
		}

		public class BookStore : IBookStore
		{
			private IItemService itemService;

			public BookStore(IItemService itemService)
			{
				this.itemService = itemService;
			}
		}

		public class BadDependencyResolver : ISubDependencyResolver
		{
			private IKernel kernel;

			public BadDependencyResolver(IKernel kernel)
			{
				this.kernel = kernel;
			}

			public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			                      DependencyModel dependency)
			{
				return kernel.Resolve<IBookStore>();
			}

			public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			                       DependencyModel dependency)
			{
				return dependency.TargetType == typeof(IBookStore);
			}
		}

		public class GoodDependencyResolver : ISubDependencyResolver
		{
			public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			                      DependencyModel dependency)
			{
				return contextHandlerResolver.Resolve(context, contextHandlerResolver, model,
				                                      new DependencyModel(DependencyType.Service, typeof(IBookStore).FullName,
				                                                          typeof(IBookStore), false));
			}

			public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			                       DependencyModel dependency)
			{
				return dependency.TargetType == typeof(IBookStore);
			}
		}

		[Test]
		[ExpectedException(typeof(CircularDependencyException))]
		public void UsingBadResolver()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new BadDependencyResolver(container.Kernel));
			container
				.Register(
				Component.For<IItemService>().ImplementedBy<ItemService>(),
				Component.For<IBookStore>().ImplementedBy<BookStore>()
				);
			container.Resolve<IItemService>();
		}

		[Test]
		[ExpectedException(typeof(DependencyResolverException))]
		public void UsingGoodResolver()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new GoodDependencyResolver());
			container
				.Register(
				Component.For<IItemService>().ImplementedBy<ItemService>(),
				Component.For<IBookStore>().ImplementedBy<BookStore>()
				);
			container.Resolve<IItemService>();
		}
	}
}