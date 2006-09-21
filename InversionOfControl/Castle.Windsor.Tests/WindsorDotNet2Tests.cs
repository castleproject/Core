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
    using System.IO;

    using Castle.Windsor.Tests.Components;
    using Castle.Windsor.Configuration.Interpreters;
    using Castle.MicroKernel.Resolvers;

    using NUnit.Framework;

    [TestFixture]
    public class WindsorDotNet2Tests
    {
        [Test]
        public void ResolveGeneric()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            ICalcService svr = container.Resolve<ICalcService>();
            Assert.IsTrue(typeof(CalculatorService).IsInstanceOfType(svr));
        }

        [Test]
        public void ResolveGenericWithId()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            ICalcService svr = container.Resolve<ICalcService>("calc");
            Assert.IsTrue(typeof(CalculatorService).IsInstanceOfType(svr));
        }

        [Test]
        public void GetGenericService()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos.generic");
            Assert.IsNotNull(repos);
            Assert.IsTrue(typeof(DemoRepository<int>).IsInstanceOfType(repos));
        }

        [Test]
        public void GetGenericServiceWithDecorator()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericsConfig.xml")));
            IRepository<int> repos = container.Resolve<IRepository<int>>("int.repos");
            Assert.IsNotNull(repos);
            Assert.IsTrue(typeof(LoggingRepositoryDecorator<int>).IsInstanceOfType(repos));
            Assert.IsTrue(typeof(DemoRepository<int>).IsInstanceOfType(((LoggingRepositoryDecorator<int>)repos).inner));
        }

        [Test]
		[ExpectedException(typeof(DependencyResolverException), @"Cycle detected in configuration.
Component int.repos has a dependency on Castle.Windsor.Tests.IRepository`1[System.Int32], but it doesn't provide an override.
You must provide an override if a component has a dependency on a service that it - itself - provides")]
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
            Assert.IsTrue(typeof(LoggingRepositoryDecorator<int>).IsInstanceOfType(repos));

            Assert.IsTrue(typeof(LoggingRepositoryDecorator<int>).IsInstanceOfType(repos));
            Assert.IsTrue(typeof(DemoRepository<int>).IsInstanceOfType(((LoggingRepositoryDecorator<int>)repos).inner));

            DemoRepository<int> inner = ((LoggingRepositoryDecorator<int>)repos).inner as DemoRepository<int>;

            Assert.AreEqual("second", inner.Name);
        }

        [Test]
        public void InferGenericArgumentForComponentFromPassedType()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

            IRepository<string> repos = container.Resolve<IRepository<string>>();
            Assert.IsTrue(typeof(LoggingRepositoryDecorator<string>).IsInstanceOfType(repos));

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
            IRepository<IEmployee> empRepost = container[typeof(IRepository<IEmployee>)] as IRepository<IEmployee>;
            Assert.IsNotNull(empRepost);
            Assert.IsTrue(typeof(LoggingRepositoryDecorator<IEmployee>).IsInstanceOfType(empRepost));
            LoggingRepositoryDecorator<IEmployee> log = empRepost as LoggingRepositoryDecorator<IEmployee>;
            IRepository<IEmployee> inner = log.inner;
            Assert.IsNotNull(inner);
            Assert.IsTrue(typeof(DemoRepository<IEmployee>).IsInstanceOfType(inner));
            DemoRepository<IEmployee> demoEmpRepost = inner as DemoRepository<IEmployee>;
            Assert.AreEqual("Generic Repostiory", demoEmpRepost.Name);
            Assert.IsNotNull(demoEmpRepost.Cache);
            Assert.IsTrue(typeof(HashTableCache<IEmployee>).IsInstanceOfType(demoEmpRepost.Cache));
        }

        [Test]
        public void ComplexGenericConfiguration_GetReviewRepository_BoundAtConfiguration()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IReviewer> rev = container.Resolve<IRepository<IReviewer>>();

            Assert.IsTrue(typeof(ReviewerRepository).IsInstanceOfType(rev));
            ReviewerRepository repos = rev as ReviewerRepository;
            Assert.AreEqual("Reviewer Repository", repos.Name);
            Assert.IsNotNull(repos.Cache);
            Assert.IsTrue(typeof(HashTableCache<IReviewer>).IsInstanceOfType(repos.Cache));
        }

        [Test]
        public void ComplexGenericConfiguration_GetReviewableRepostiory_ShouldResolveToDemoRepostiroyInsideLoggingRepositoryWithNoCaching()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IReviewableEmployee> empRepost = container.Resolve<IRepository<IReviewableEmployee>>();
            Assert.IsNotNull(empRepost);
            Assert.IsTrue(typeof(LoggingRepositoryDecorator<IReviewableEmployee>).IsInstanceOfType(empRepost));
            LoggingRepositoryDecorator<IReviewableEmployee> log = empRepost as LoggingRepositoryDecorator<IReviewableEmployee>;
            IRepository<IReviewableEmployee> inner = log.inner;
            Assert.IsNotNull(inner);
            Assert.IsTrue(typeof(DemoRepository<IReviewableEmployee>).IsInstanceOfType(inner));
            DemoRepository<IReviewableEmployee> demoEmpRepost = inner as DemoRepository<IReviewableEmployee>;
            Assert.AreEqual("Generic Repostiory With No Cache", demoEmpRepost.Name);
            Assert.IsNotNull(demoEmpRepost.Cache);
            Assert.IsTrue(typeof(NullCache<IReviewableEmployee>).IsInstanceOfType(demoEmpRepost.Cache));
        }

        [Test]
        public void TestGenericSpecialization()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
            IRepository<IReviewer> repository = container.Resolve<IRepository<IReviewer>>();
            Assert.IsTrue(typeof(ReviewerRepository).IsInstanceOfType(repository), "Not ReviewerRepository!");
        }

		[Test]
		public void TestComponentLifestylePerGenericType()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("comp", typeof(IRepository<>), typeof(TransientRepository<>));

			object o1 = container.Resolve<IRepository<Employee>>();
			object o2 = container.Resolve<IRepository<Employee>>();
			object o3 = container.Resolve<IRepository<Reviewer>>();
			object o4 = container.Resolve<IRepository<Reviewer>>();

			Assert.IsFalse( Object.ReferenceEquals(o1, o2) );
			Assert.IsFalse( Object.ReferenceEquals(o1, o3) );
			Assert.IsFalse( Object.ReferenceEquals(o1, o4) );
		}

    	[Test]
    	public void CanCreateANormalTypeWithCtorDependencyOnGenericType()
    	{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("comp", typeof(NeedsGenericType));
			container.AddComponent("cache", typeof(ICache<>), typeof(NullCache<>));

    		NeedsGenericType needsGenericType = container.Resolve<NeedsGenericType>();
    		
			Assert.IsNotNull(needsGenericType);
    	}
	
    	public string GetFilePath(string fileName)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
							   "../Castle.Windsor.Tests/DotNet2Config/" + fileName);
		}
	}
}

#endif