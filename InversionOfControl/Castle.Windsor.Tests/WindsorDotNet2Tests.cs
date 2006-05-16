#if DOTNET2

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Castle.Windsor.Tests.Configuration2;
using NUnit.Framework;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Model.Resource;
using Castle.MicroKernel.Handlers;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class WindsorDotNet2Tests
    {
        private IWindsorContainer container;
        public string GetFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                               "../Castle.Windsor.Tests/DotNet2Config/" + fileName);
        }

        [SetUp]
        public void Init()
        {
            container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
        }

        [Test]
        public void ResovleGeneric()
        {
            ICalcService svr = container.Resolve<ICalcService>();
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }

        [Test]
        public void ResovleGenericWithId()
        {
            ICalcService svr = container.Resolve<ICalcService>("calc");
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }

        [Test]
        public void GetGenericService()
        {
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos.generic");
            Assert.IsNotNull(repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>),repos);
        }

        [Test]
        public void GetGenericServiceWithDecorator()
        {
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos");
            Assert.IsNotNull(repos);
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>), repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>), ((LoggingRepositoryDecorator<int>)repos).inner);
        }

        const string ExpectedExceptionMessage = @"Can't create component 'int.repos' as it has dependencies to be satisfied. 
int.repos is waiting for the following dependencies: 

Services: 
- Castle.Windsor.Tests.IRepository`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]. A dependency cannot be satisfied by itself, did you forget to add a parameter name to differentiate between the two dependencies? 
";
        [Test]
        [ExpectedException(typeof(HandlerException),ExpectedExceptionMessage)]
        public void ThrowsExceptionIfTryToResolveComponentWithDependencyOnItself()
        {
                container = new WindsorContainer(new XmlInterpreter(GetFilePath("RecursiveDecoratorConfig.xml")));
                container.Resolve<IRepository<int>>();
        }

        [Test]
        [Ignore("Doesn't work, find out why")]
        public void GetGenericServiceWithDecorator_GenericDecoratorOnTop()
        {
            container = new WindsorContainer(new XmlInterpreter(GetFilePath("DecoratorConfig.xml")));
            container.Resolve<IRepository<int>>();
        }
    }

    public interface IRepository<T>
    {
        T Get(int id);
    }

    public class DemoRepository<T> : IRepository<T>
    {
        public T Get(int id)
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class LoggingRepositoryDecorator<T> : IRepository<T>
    {
        public IRepository<T> inner;

        public LoggingRepositoryDecorator(IRepository<T> inner)
        {
            this.inner = inner;
        }

        public T Get(int id)
        {
            Console.WriteLine("Getting {0}", id);
            return inner.Get(id);
        }
    }
}
#endif