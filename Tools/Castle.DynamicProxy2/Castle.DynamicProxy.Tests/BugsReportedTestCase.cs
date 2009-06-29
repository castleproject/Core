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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.BugsReported;
	using InterClasses;
	using NUnit.Framework;

	[TestFixture]
	public class BugsReportedTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void InterfaceInheritance()
		{
			ICameraService proxy = (ICameraService)
			                       generator.CreateInterfaceProxyWithTarget(typeof (ICameraService),
			                                                                new CameraService(),
			                                                                new StandardInterceptor());

			Assert.IsNotNull(proxy);

			proxy.Add("", "");
			proxy.Record(null);
		}

		[Test]
		public void ProxyInterfaceWithSetterOnly()
		{
			IHaveOnlySetter proxy = (IHaveOnlySetter)
			                        generator.CreateInterfaceProxyWithTarget(typeof (IHaveOnlySetter),
			                                                                 new HaveOnlySetter(),
			                                                                 new SkipCallingMethodInterceptor());

			Assert.IsNotNull(proxy);

			proxy.Foo = "bar";
		}

		[Test]
		[ExpectedException(typeof (NotImplementedException),
			ExpectedMessage =
				"This is a DynamicProxy2 error: the interceptor attempted to 'Proceed' for a method without a target, for example, an interface method or an abstract method"
			)]
		public void CallingProceedOnAbstractMethodShouldThrowException()
		{
			AbstractClass proxy = (AbstractClass)
			                      generator.CreateClassProxy(typeof (AbstractClass), ProxyGenerationOptions.Default,
			                                                 new StandardInterceptor());

			Assert.IsNotNull(proxy);

			proxy.Foo();
		}

		[Test]
		public void ProxyTypeThatInheritFromGenericType()
		{
			IUserRepository proxy = (IUserRepository)
			                        generator.CreateInterfaceProxyWithoutTarget(typeof (IUserRepository),
			                                                                    new SkipCallingMethodInterceptor());

			Assert.IsNotNull(proxy);
		}

		[Test]
		public void DYNPROXY_51_GenericMarkerInterface()
		{
			WithMixin p =
				(WithMixin) generator.CreateClassProxy(typeof (WithMixin), new Type[] {typeof (Marker<int>)}, new IInterceptor[0]);
			p.Method();
		}

		[Test]
		public void DYNPROXY_99_ClassProxyHasNamespace()
		{
			Type type = generator.CreateClassProxy(typeof(ServiceImpl)).GetType();
			Assert.IsNotNull(type.Namespace);
			Assert.AreEqual("Castle.Proxies", type.Namespace);
		}

		[Test]
		public void DYNPROXY_99_InterfaceProxyWithTargetHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithTarget(typeof(IService),new ServiceImpl()).GetType();
			Assert.IsNotNull(type.Namespace);
			Assert.AreEqual("Castle.Proxies", type.Namespace);
		}

		[Test]
		public void DYNPROXY_99_InterfaceProxyWithTargetInterfaceHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl()).GetType();
			Assert.IsNotNull(type.Namespace);
			Assert.AreEqual("Castle.Proxies", type.Namespace);
		}
		[Test]
		public void DYNPROXY_99_InterfaceProxyWithoutTargetHasNamespace()
		{
			Type type = generator.CreateInterfaceProxyWithoutTarget(typeof(IService)).GetType();
			Assert.IsNotNull(type.Namespace);
			Assert.AreEqual("Castle.Proxies", type.Namespace);
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

	public class SkipCallingMethodInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
		}
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