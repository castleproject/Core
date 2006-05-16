// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

#if DOTNET2

namespace Castle.Windsor.Tests
{
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
    using Castle.MicroKernel.Resolvers;

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

        [Test]
        [ExpectedException(typeof(DependencyResolverException), "Type Castle.Windsor.Tests.IRepository`1[System.Int32] has a mandatory dependency on itself. Can't satisfy the dependency!")]
        public void ThrowsExceptionIfTryToResolveComponentWithDependencyOnItself()
        {
                container = new WindsorContainer(new XmlInterpreter(GetFilePath("RecursiveDecoratorConfig.xml")));
                container.Resolve<IRepository<int>>();
        }

        [Test]
        public void GetGenericServiceWithDecorator_GenericDecoratorOnTop()
        {
            container = new WindsorContainer(new XmlInterpreter(GetFilePath("DecoratorConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>();
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>),
                repos);

            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>), repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>), ((LoggingRepositoryDecorator<int>)repos).inner);

            DemoRepository<int> inner = ((LoggingRepositoryDecorator<int>)repos).inner as DemoRepository<int>;

            Assert.AreEqual("second", inner.Name);
        }
    }

    public interface IRepository<T>
    {
        T Get(int id);
    }

    public class DemoRepository<T> : IRepository<T>
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public T Get(int id)
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class LoggingRepositoryDecorator<T> : IRepository<T>
    {
        public IRepository<T> inner;

        public LoggingRepositoryDecorator()
        { }
        
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