// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;

	using NUnit.Framework;

	[TestFixture]
	public class BasicClassProxyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ProxyForBaseTypeFromUnsignedAssembly()
		{
			Type t = typeof(Class);
			Assert.False(StrongNameUtil.IsAssemblySigned(t.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t, new StandardInterceptor());
			Assert.False(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

		[Test]
		public void ProxyForBaseTypeAndInterfaceFromUnsignedAssembly()
		{
			Type t1 = typeof(Class);
			Type t2 = typeof(IInterface);
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

		[Test]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies1()
		{
			Type t1 = typeof(Class);
			Type t2 = typeof(IServiceProvider);
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

		[Test]
		public void ProxyForBaseTypeAndInterfaceFromSignedAndUnsignedAssemblies2()
		{
			Type t1 = typeof(List<int>);
			Type t2 = typeof(IInterface);
			Assert.IsTrue(StrongNameUtil.IsAssemblySigned(t1.GetTypeInfo().Assembly));
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t2.GetTypeInfo().Assembly));
			object proxy = generator.CreateClassProxy(t1, new Type[] { t2 }, new StandardInterceptor());
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(proxy.GetType().GetTypeInfo().Assembly));
		}

		public abstract class Class
		{
			public abstract void ClassMethod();
			public abstract void Method();
		}

		public interface IInterface
		{
			void InterfaceMethod();
			void Method();
		}
	}
}
