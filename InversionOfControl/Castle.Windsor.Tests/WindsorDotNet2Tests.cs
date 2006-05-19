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
    using Castle.Model.Internal;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    
	using Castle.Windsor.Tests.Configuration2;
	using Castle.Windsor.Tests.Components;
    using Castle.Windsor.Configuration.Interpreters;
    using Castle.Model.Resource;
    using Castle.MicroKernel.Handlers;
    using Castle.MicroKernel.Resolvers;

	using NUnit.Framework;

    [TestFixture]
    public class WindsorDotNet2Tests
    {
        public string GetFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                               "../Castle.Windsor.Tests/DotNet2Config/" + fileName);
        }

        [Test]
        public void ResovleGeneric()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            ICalcService svr = container.Resolve<ICalcService>();
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }

        [Test]
        public void ResovleGenericWithId()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            ICalcService svr = container.Resolve<ICalcService>("calc");
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }

        [Test]
        public void GetGenericService()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos.generic");
            Assert.IsNotNull(repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>), repos);
        }

        [Test]
        public void GetGenericServiceWithDecorator()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos");
            Assert.IsNotNull(repos);
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>), repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>), ((LoggingRepositoryDecorator<int>)repos).inner);
        }

        [Test]
        [ExpectedException(typeof(DependencyResolverException), @"Cycle detected in cofiguration.
Component int.repos has a dependency Castle.Windsor.Tests.IRepository`1[System.Int32], but it doesn't provide an override.
You must provide an override if a component has a dependency on a service that it registers.")]
        public void ThrowsExceptionIfTryToResolveComponentWithDependencyOnItself()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("RecursiveDecoratorConfig.xml")));
            container.Resolve<IRepository<int>>();
        }

        [Test]
        public void GetGenericServiceWithDecorator_GenericDecoratorOnTop()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("DecoratorConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>();
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>),
                repos);

            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<int>), repos);
            Assert.IsInstanceOfType(typeof(DemoRepository<int>), ((LoggingRepositoryDecorator<int>)repos).inner);

            DemoRepository<int> inner = ((LoggingRepositoryDecorator<int>)repos).inner as DemoRepository<int>;

            Assert.AreEqual("second", inner.Name);
        }

        [Test]
        public void InferGenericArgumentForComponentFromPassedType()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

            IRepository<string> repos = container.Resolve<IRepository<string>>();
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<string>), repos);

            DemoRepository<string> inner = ((LoggingRepositoryDecorator<string>)repos).inner as DemoRepository<string>;

            Assert.AreEqual("second", inner.Name);

        }

        [Test]
        public void GetSameInstanceFromGenericType()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

            IRepository<string> repos1 = container.Resolve<IRepository<string>>();
            IRepository<string> repos2 = container.Resolve<IRepository<string>>();
            
            Assert.AreSame(repos1, repos2);
         
        }

        [Test]
        public void GetSameInstanceOfGenericFromTwoDifferentGenericTypes()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

            IRepository<string> repos1 = container.Resolve<IRepository<string>>();
            IRepository<string> repos2 = container.Resolve<IRepository<string>>();

            Assert.AreSame(repos1, repos2);

            IRepository<int> repos3 = container.Resolve<IRepository<int>>();
            IRepository<int> repos4 = container.Resolve<IRepository<int>>();

            Assert.AreSame(repos3, repos4);
        }

        [Test]
        public void ComplexGenericConfiguration_GetGenericRepostiory()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IEmployee> empRepost = container[typeof (IRepository<IEmployee>)] as IRepository<IEmployee>;
            Assert.IsNotNull(empRepost);
            Assert.IsInstanceOfType(typeof (LoggingRepositoryDecorator<IEmployee>), empRepost);
            LoggingRepositoryDecorator<IEmployee> log = empRepost as LoggingRepositoryDecorator<IEmployee>;
            IRepository<IEmployee> inner = log.inner;
            Assert.IsNotNull(inner);
            Assert.IsInstanceOfType(typeof (DemoRepository<IEmployee>), inner);
            DemoRepository<IEmployee> demoEmpRepost = inner as DemoRepository<IEmployee>;
            Assert.AreEqual("Generic Repostiory", demoEmpRepost.Name);
            Assert.IsNotNull(demoEmpRepost.Cache);
            Assert.IsInstanceOfType(typeof (HashTableCache<IEmployee>), demoEmpRepost.Cache);
        }

        [Test]
        public void ComplexGenericConfiguration_GetReviewRepository_BoundAtConfiguration()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IReviewer> rev = container.Resolve<IRepository<IReviewer>>();

            Assert.IsInstanceOfType(typeof(ReviewerRepository), rev);
            ReviewerRepository repos = rev as  ReviewerRepository;
            Assert.AreEqual("Reviewer Repository", repos.Name);
            Assert.IsNotNull(repos.Cache);
            Assert.IsInstanceOfType(typeof(HashTableCache<IReviewer>), repos.Cache);
        }

        [Test]
        public void ComplexGenericConfiguration_GetReviewableRepostiory_ShouldResolveToDemoRepostiroyInsideLoggingRepositoryWithNoCaching()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IReviewableEmployee> empRepost = container.Resolve<IRepository<IReviewableEmployee>>();
            Assert.IsNotNull(empRepost);
            Assert.IsInstanceOfType(typeof(LoggingRepositoryDecorator<IReviewableEmployee>), empRepost);
            LoggingRepositoryDecorator<IReviewableEmployee> log = empRepost as LoggingRepositoryDecorator<IReviewableEmployee>;
            IRepository<IReviewableEmployee> inner = log.inner;
            Assert.IsNotNull(inner);
            Assert.IsInstanceOfType(typeof(DemoRepository<IReviewableEmployee>), inner);
            DemoRepository<IReviewableEmployee> demoEmpRepost = inner as DemoRepository<IReviewableEmployee>;
            Assert.AreEqual("Generic Repostiory With No Cache", demoEmpRepost.Name);                
            Assert.IsNotNull(demoEmpRepost.Cache);
            Assert.IsInstanceOfType(typeof(NullCache<IReviewableEmployee>), demoEmpRepost.Cache);
        }
        //more tests:
        //  cache generic types
        //  cache properties of generic types
        //  cache ctors of generic typesd
    }
}
#endif
