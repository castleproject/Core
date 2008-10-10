using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Exceptions;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
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

            public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
            {
                return kernel.Resolve<IBookStore>();
            }

            public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
            {
                return dependency.TargetType == typeof (IBookStore);
            }
        }

        public class GoodDependencyResolver : ISubDependencyResolver
        {

            public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
            {
                return parentResolver.Resolve(context, parentResolver, model,
                    new DependencyModel(DependencyType.Service, typeof(IBookStore).FullName, typeof(IBookStore), false));
            }

            public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
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