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

#if !MONO
#if !SILVERLIGHT // we do not support xml config on SL

namespace Castle.Windsor.Tests
{
	using System;
	using System.IO;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor.Configuration.Interpreters;
	using Components;
	using Core;
	using NUnit.Framework;

	[TestFixture]
	public class WindsorDotNet2Tests
	{
		[Test]
		public void UsingResolveGenericMethodOverload()
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
		public void WillUseDefaultCtorOnGenericComponentIfTryingToResolveOnSameComponent()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("RecursiveDecoratorConfig.xml")));
			LoggingRepositoryDecorator<int> resolve =
				(LoggingRepositoryDecorator<int>)container.Resolve<IRepository<int>>();
			Assert.IsNull(resolve.inner);
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
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

			IRepository<string> repos = container.Resolve<IRepository<string>>();
			Assert.IsTrue(typeof(LoggingRepositoryDecorator<string>).IsInstanceOfType(repos));

			DemoRepository<string> inner = ((LoggingRepositoryDecorator<string>)repos).inner as DemoRepository<string>;

			Assert.AreEqual("second", inner.Name);
		}

		[Test]
		public void GetSameInstanceFromGenericType()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

			IRepository<string> repos1 = container.Resolve<IRepository<string>>();
			IRepository<string> repos2 = container.Resolve<IRepository<string>>();

			Assert.AreSame(repos1, repos2);
		}

		[Test]
		public void GetSameInstanceOfGenericFromTwoDifferentGenericTypes()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("GenericDecoratorConfig.xml")));

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
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
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
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
			IRepository<IReviewer> rev = container.Resolve<IRepository<IReviewer>>();

			Assert.IsTrue(typeof(ReviewerRepository).IsInstanceOfType(rev));
			ReviewerRepository repos = rev as ReviewerRepository;
			Assert.AreEqual("Reviewer Repository", repos.Name);
			Assert.IsNotNull(repos.Cache);
			Assert.IsTrue(typeof(HashTableCache<IReviewer>).IsInstanceOfType(repos.Cache));
		}

		[Test]
		public void
			ComplexGenericConfiguration_GetReviewableRepostiory_ShouldResolveToDemoRepostiroyInsideLoggingRepositoryWithNoCaching
			()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
			IRepository<IReviewableEmployee> empRepost = container.Resolve<IRepository<IReviewableEmployee>>();
			Assert.IsNotNull(empRepost);
			Assert.IsTrue(typeof(LoggingRepositoryDecorator<IReviewableEmployee>).IsInstanceOfType(empRepost));
			LoggingRepositoryDecorator<IReviewableEmployee> log =
				empRepost as LoggingRepositoryDecorator<IReviewableEmployee>;
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
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
			IRepository<IReviewer> repository = container.Resolve<IRepository<IReviewer>>();
			Assert.IsTrue(typeof(ReviewerRepository).IsInstanceOfType(repository), "Not ReviewerRepository!");
		}

		[Test]
		public void ComplexGenericConfiguration_GetRepositoryByIdAndType()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("ComplexGenericConfig.xml")));
			IRepository<IReviewer> repository = container.Resolve<IRepository<IReviewer>>("generic.repository");
			Assert.IsTrue(typeof(DemoRepository<IReviewer>).IsInstanceOfType(repository), "Not DemoRepository!");
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

			Assert.IsFalse(ReferenceEquals(o1, o2));
			Assert.IsFalse(ReferenceEquals(o1, o3));
			Assert.IsFalse(ReferenceEquals(o1, o4));
		}

        [Test]
        public void TestComponentLifestylePerGenericTypeNotMarkedAsTransient()
        {
            IWindsorContainer container = new WindsorContainer();

            container.AddComponentLifeStyle("comp", typeof(IRepository<>), 
                typeof(RepositoryNotMarkedAsTransient<>), LifestyleType.Transient);

            object o1 = container.Resolve<IRepository<Employee>>();
            object o2 = container.Resolve<IRepository<Employee>>();
            object o3 = container.Resolve<IRepository<Reviewer>>();
            object o4 = container.Resolve<IRepository<Reviewer>>();

            Assert.IsFalse(Object.ReferenceEquals(o1, o2));
            Assert.IsFalse(Object.ReferenceEquals(o1, o3));
            Assert.IsFalse(Object.ReferenceEquals(o1, o4));
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

		[Test]
		public void InterceptGeneric1()
		{
			WindsorContainer container = new WindsorContainer();

			container.AddFacility("interceptor-facility", new MyInterceptorGreedyFacility());
			container.AddComponent("interceptor", typeof(StandardInterceptor));
			container.AddComponent("key", typeof(IRepository<Employee>), typeof(DemoRepository<Employee>));

			IRepository<Employee> store = container.Resolve<IRepository<Employee>>();

			Assert.IsFalse(typeof(DemoRepository<Employee>).IsInstanceOfType(store), "This should have been a proxy");
		}

		[Test]
		public void InterceptGeneric2()
		{
			WindsorContainer container = new WindsorContainer();

			container.AddFacility("interceptor-facility", new MyInterceptorGreedyFacility2());
			container.AddComponent("interceptor", typeof(StandardInterceptor));
			container.AddComponent("key", typeof(IRepository<>), typeof(DemoRepository<>));

			IRepository<Employee> store = container.Resolve<IRepository<Employee>>();

			Assert.IsFalse(typeof(DemoRepository<Employee>).IsInstanceOfType(store), "This should have been a proxy");
		}

		[Test]
		public void ChainOfResponsability()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("chainOfRespnsability.config")));
			IResultFinder<int> resolve = container.Resolve<IResultFinder<int>>();
			Assert.IsTrue(resolve.Finder is DatabaseResultFinder<int>);
			Assert.IsTrue(resolve.Finder.Finder is WebServiceResultFinder<int>);
			Assert.IsTrue(resolve.Finder.Finder.Finder is FailedResultFinder<int>);
			Assert.IsTrue(resolve.Finder.Finder.Finder.Finder == null);
		}

		[Test, Ignore]
		public void ChainOfResponsability_Smart()
		{
			IWindsorContainer container =
				new WindsorContainer(new XmlInterpreter(GetFilePath("chainOfRespnsability_smart.config")));
			IResultFinder<int> resolve = container.Resolve<IResultFinder<int>>();
			Assert.IsTrue(resolve is CacheResultFinder<int>);
			Assert.IsTrue(resolve.Finder is DatabaseResultFinder<int>);
			Assert.IsTrue(resolve.Finder.Finder is WebServiceResultFinder<int>);
			Assert.IsNull(resolve.Finder.Finder.Finder);

			IResultFinder<String> resolve2 = container.Resolve<IResultFinder<String>>();
			Assert.IsTrue(resolve2 is ResultFinderStringDecorator);
			Assert.IsNotNull(resolve2.Finder);
		}

		[Test]
		public void LifestyleIsInheritsFromGenericType()
		{
			WindsorContainer container = new WindsorContainer();

			container.AddFacility("interceptor-facility", new MyInterceptorGreedyFacility2());
			container.AddComponent("interceptor", typeof(StandardInterceptor));
			container.AddComponentLifeStyle("key", typeof(IRepository<>), typeof(DemoRepository<>), LifestyleType.Transient);

			IRepository<Employee> store = container.Resolve<IRepository<Employee>>();
			IRepository<Employee> anotherStore = container.Resolve<IRepository<Employee>>();

			Assert.IsFalse(typeof(DemoRepository<Employee>).IsInstanceOfType(store), "This should have been a proxy");
			Assert.AreNotSame(store, anotherStore, "This should be two different instances");
		}

		[Test]
		public void InterceptorInheritFromGenericType()
		{
			WindsorContainer container = new WindsorContainer();

			container.AddComponent("interceptor", typeof(MyInterceptor));
			container.AddComponent("key", typeof(IRepository<>), typeof(DemoRepository<>));
			container.Kernel.GetHandler(typeof(IRepository<>)).ComponentModel.Interceptors.Add(
				new InterceptorReference(typeof(MyInterceptor)));

			IRepository<object> demoRepository = container.Resolve<IRepository<object>>();
			demoRepository.Get(12);

			Assert.AreEqual(12, MyInterceptor.InterceptedId , "invocation should have been intercepted by MyInterceptor");
		}

		[Test]
		public void ParentResolverIntercetorShouldNotAffectGenericComponentInterceptor()
		{
			WindsorContainer container = new WindsorContainer();
			container.AddComponent<MyInterceptor>();

			container.Register(
				Component.For<ISpecification>()
					.ImplementedBy<MySpecification>()
					.Interceptors(new InterceptorReference(typeof(MyInterceptor)))
					.Anywhere
				);
			container.AddComponent("repos", typeof(IRepository<>), typeof(TransientRepository<>));

			ISpecification specification = container.Resolve<ISpecification>();
			bool isProxy = specification.Repository.GetType().FullName.Contains("Proxy");
			Assert.IsFalse(isProxy);
		}


		public string GetFilePath(string fileName)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
								ConfigHelper.ResolveConfigPath("DotNet2Config/" + fileName));
		}
	}

	public class MyInterceptor : IInterceptor
	{
		public static int InterceptedId = 0;

		public void Intercept(IInvocation invocation)
		{
			invocation.Proceed();
			if (invocation.Arguments.Length > 0)
				InterceptedId = (int)invocation.Arguments[0];
		}
	}

	public interface IResultFinder<T>
	{
		T Process(ISpecification specification);
		IResultFinder<T> Finder { get; }
	}

	public class CacheResultFinder<T> : IResultFinder<T>
	{
		private IResultFinder<T> finder;

		public IResultFinder<T> Finder
		{
			get { return finder; }
		}

		public CacheResultFinder()
		{
		}

		public CacheResultFinder(IResultFinder<T> finder)
		{
			this.finder = finder;
		}

		public T Process(ISpecification specification)
		{
			return default(T);
		}
	}

	public class DatabaseResultFinder<T> : IResultFinder<T>
	{
		private IResultFinder<T> finder;

		public IResultFinder<T> Finder
		{
			get { return finder; }
		}

		public DatabaseResultFinder()
		{
		}

		public DatabaseResultFinder(IResultFinder<T> finder)
		{
			this.finder = finder;
		}

		public T Process(ISpecification specification)
		{
			return default(T);
		}
	}

	public class WebServiceResultFinder<T> : IResultFinder<T>
	{
		private IResultFinder<T> finder;

		public IResultFinder<T> Finder
		{
			get { return finder; }
		}

		public WebServiceResultFinder()
		{
		}

		public WebServiceResultFinder(IResultFinder<T> finder)
		{
			this.finder = finder;
		}

		public T Process(ISpecification specification)
		{
			return default(T);
		}
	}

	public class FailedResultFinder<T> : IResultFinder<T>
	{
		public IResultFinder<T> Finder
		{
			get { return null; }
		}

		public T Process(ISpecification specification)
		{
			return default(T);
		}
	}

	public class ResultFinderStringDecorator : IResultFinder<string>
	{
		private IResultFinder<string> finder;

		public ResultFinderStringDecorator(IResultFinder<string> finder)
		{
			this.finder = finder;
		}

		public IResultFinder<string> Finder
		{
			get { return finder; }
		}

		public String Process(ISpecification specification)
		{
			return String.Empty;
		}
	}

	public interface ISpecification
	{
		IRepository<int> Repository { get; }
	}

	public class MySpecification : ISpecification
	{
		private readonly IRepository<int> repository;

		public IRepository<int> Repository
		{
			get { return repository; }
		}

		public MySpecification(IRepository<int> repository)
		{
			this.repository = repository;
		}
	}
}

#endif
#endif