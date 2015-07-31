// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;

	using Castle.DynamicProxy.Tests.BugsReported;

	using Interceptors;

	using InterClasses;

	using Xunit;

	public class BugsReportedTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void InterfaceInheritance()
		{
			ICameraService proxy = (ICameraService)
				generator.CreateInterfaceProxyWithTarget(typeof(ICameraService),
					new CameraService(),
					new StandardInterceptor());

			Assert.NotNull(proxy);

			proxy.Add("", "");
			proxy.Record(null);
		}

		[Fact]
		public void ProxyInterfaceWithSetterOnly()
		{
			IHaveOnlySetter proxy = (IHaveOnlySetter)
				generator.CreateInterfaceProxyWithTarget(typeof(IHaveOnlySetter),
					new HaveOnlySetter(),
					new DoNothingInterceptor());

			Assert.NotNull(proxy);

			proxy.Foo = "bar";
		}

		[Fact]
		public void CallingProceedWithInterceptorOnAbstractMethodShouldThrowException()
		{
			var proxy = generator.CreateClassProxy<AbstractClass>(ProxyGenerationOptions.Default, new StandardInterceptor());
			Assert.NotNull(proxy);

			Action callProxyMethod = () =>
				proxy.Foo();

			var message =
				"This is a DynamicProxy2 error: The interceptor attempted to 'Proceed' for method 'System.String Foo()' which is abstract. " +
				"When calling an abstract method there is no implementation to 'proceed' to " +
				"and it is the responsibility of the interceptor to mimic the implementation (set return value, out arguments etc)";
			var exception =
				Assert.Throws(typeof(NotImplementedException), callProxyMethod);
			Assert.Equal(message, exception.Message);
		}

		[Fact]
		public void CallingProceedWithoutInterceptorOnAbstractMethodShouldThrowException()
		{
			var proxy = generator.CreateClassProxy<AbstractClass>();
			Assert.NotNull(proxy);

			Action callProxyMethod = () =>
				proxy.Foo();

			var message =
				"This is a DynamicProxy2 error: There are no interceptors specified for method 'System.String Foo()' which is abstract. " +
				"When calling an abstract method there is no implementation to 'proceed' to " +
				"and it is the responsibility of the interceptor to mimic the implementation (set return value, out arguments etc)";
			var exception =
				Assert.Throws(typeof(NotImplementedException), callProxyMethod);
			Assert.Equal(message, exception.Message);
		}

		[Fact]
		public void ProxyTypeThatInheritFromGenericType()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IUserRepository>(new DoNothingInterceptor());
			Assert.NotNull(proxy);
		}

		[Fact]
		public void DYNPROXY_51_GenericMarkerInterface()
		{
			WithMixin p =
				(WithMixin)generator.CreateClassProxy(typeof(WithMixin), new Type[] { typeof(Marker<int>) }, new IInterceptor[0]);
			p.Method();
		}

		[Fact]
		public void DYNPROXY_99_ClassProxyHasNamespace()
		{
			Type type = generator.CreateClassProxy(typeof(ServiceImpl)).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithTargetHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl()).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithTargetInterfaceHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl()).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithoutTargetHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithoutTarget(typeof(IService)).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}
	}

	public interface IRepository<TEntity, TKey>
	{
		TEntity GetById(TKey key);
	}

	public class User
	{
	}

	public interface IUserRepository : IRepository<User, string>
	{
	}

	public abstract class AbstractClass
	{
		public abstract string Foo();
	}

	public interface IHaveOnlySetter
	{
		string Foo { set; }
	}

	public class HaveOnlySetter : IHaveOnlySetter
	{
		public string Foo
		{
			set { throw new Exception("The method or operation is not implemented."); }
		}
	}

	public interface Marker<T>
	{
	}

	public class WithMixin
	{
		public virtual void Method()
		{
		}
	}
}