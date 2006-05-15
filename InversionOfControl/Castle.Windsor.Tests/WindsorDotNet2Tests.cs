#if DOTNET2

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Model.Resource;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class WindsorDotNet2Tests
    {
        private IWindsorContainer container;
        private const string TestConfig = @"
<configuration>
    <components>
      <component id='calc'
           service='Castle.Windsor.Tests.Components.ICalcService, Castle.Windsor.Tests' 
           type='Castle.Windsor.Tests.Components.CalculatorService, Castle.Windsor.Tests' />
        
        <component id='int.repos.generic' 
           service='Castle.Windsor.Tests.IRepository`1[[System.Int32]], Castle.Windsor.Tests' 
           type='Castle.Windsor.Tests.DemoRepository`1[[System.Int32]], Castle.Windsor.Tests' />
    <component id='int.repos' 
           service='Castle.Windsor.Tests.IRepository`1[[System.Int32]], Castle.Windsor.Tests' 
           type='Castle.Windsor.Tests.LoggingRepositoryDecorator`1[[System.Int32]], Castle.Windsor.Tests'>
                <parameters>
                    <inner>${int.repos.generic}</inner>
                </parameters>
        </component>
    </components>
</configuration>";

        [SetUp]
        public void Init()
        {
            container = new WindsorContainer(new XmlInterpreter(new StaticContentResource(TestConfig)));
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

        [Test]
        [Ignore("Cause StackOverFlow because it trys to match int.repos to itself because it is on the top.")]
        public void GetGenericServiceWithDecorator_GenericDecoratorOnTop()
        {
            string Bad_TestConfig = @"
<configuration>
    <components>
         <component id='int.repos' 
           service='Castle.Windsor.Tests.IRepository`1[[System.Int32]], Castle.Windsor.Tests' 
           type='Castle.Windsor.Tests.LoggingRepositoryDecorator`1[[System.Int32]], Castle.Windsor.Tests'>
                <inner>${int.repos.generic}</inner>
        </component>
        <component id='int.repos.generic' 
           service='Castle.Windsor.Tests.IRepository`1[[System.Int32]], Castle.Windsor.Tests' 
           type='Castle.Windsor.Tests.DemoRepository`1[[System.Int32]], Castle.Windsor.Tests' />
    </components>
</configuration>";

            container = new WindsorContainer(new XmlInterpreter(new StaticContentResource(Bad_TestConfig)));
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