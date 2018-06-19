// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Tests
{
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	public interface IGenericInterface<T>
	{
		void GenericMethod(T toto);
	}

	public class Class1 : IGenericInterface<IInterface1>
	{
		public void GenericMethod(IInterface1 toto)
		{
		}
	}

	public class Class2 : IGenericInterface<IInterface2>
	{
		public void GenericMethod(IInterface2 toto)
		{
		}
	}

	public class Class3 : IInterface3, IInterface2
	{
	}

	public class Class4 : IInterface3, IInterface1
	{
	}

	public interface IInterface1
	{
	}

	public interface IInterface2
	{
	}

	public interface IInterface3
	{
	}

	[TestFixture]
	public class MultipleSavedAssembliesTestCase
	{
		[Test]
		[Bug("DYNPROXY-179")]
		public void LoadAssemblyIntoCache_InvalidCacheAfterTwoLoadAssemblyIntoCacheThatContainsSameClass()
		{
			//
			// Step 1 - Save an assembly with 1 class proxy
			//
			var proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "5", "ProxyCache5.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "5", "ProxyCache5.dll");
			var proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			var generator = new ProxyGenerator(proxyBuilder);
			generator.CreateClassProxy(typeof(EmptyClass), new[] { typeof(IInterface1) }, new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 2 - Save another assembly with 1 class proxy
			// note : to reproduce the problem, must load previously saved assembly (in cache) before saving this assembly.
			//
			proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "6", "ProxyCache6.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "6", "ProxyCache6.dll");
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			var proxyAssembly = Assembly.LoadFrom("ProxyCache5.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			generator.CreateClassProxy(typeof(EmptyClass), new[] { typeof(IInterface2) }, new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 3 - Load the last proxy assembly and try to create the first class proxy (created in step 1)
			// note : Creating proxy from step 2 works.
			// issue : returns the wrong proxy (the one from step 2)
			//
			proxyGeneratorModuleScope = new ModuleScope(true);
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			proxyAssembly = Assembly.LoadFrom("ProxyCache6.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			var invalidProxy = generator.CreateClassProxy(typeof(EmptyClass), new[] { typeof(IInterface1) }, new DoNothingInterceptor());
			if (invalidProxy as IInterface1 == null)
			{
				Assert.Fail();
			}
		}

		[Test]
		[Bug("DYNPROXY-179")]
		public void LoadAssemblyIntoCache_InvalidCacheAfterTwoLoadAssemblyIntoCacheThatContainsSameGeneric()
		{
			//
			// Step 1 - Save an assembly with 1 generic proxy
			//
			var proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "1", "ProxyCache1.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "1", "ProxyCache1.dll");
			var proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			var generator = new ProxyGenerator(proxyBuilder);
			generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface1>), new Class1(), new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 2 - Save another assembly with 1 generic proxy
			// note : to reproduce the problem, must load previously saved assembly (in cache) before saving this assembly.
			//
			proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "2", "ProxyCache2.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "2", "ProxyCache2.dll");
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			var proxyAssembly = Assembly.LoadFrom("ProxyCache1.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface2>), new Class2(), new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 3 - Load the last proxy assembly and try to create the first generic proxy (created in step 1)
			// note : Creating proxy from step 2 works.
			// exception : Missing method exception, it returns the wrong proxy and the constructor used doesn't match the arguments passed.
			//
			proxyGeneratorModuleScope = new ModuleScope(true);
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			proxyAssembly = Assembly.LoadFrom("ProxyCache2.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface1>), new Class1(), new DoNothingInterceptor());
		}

		[Test]
		[Bug("DYNPROXY-179")]
		public void LoadAssemblyIntoCache_InvalidCacheAfterTwoLoadAssemblyIntoCacheThatContainsSameInterface()
		{
			//
			// Step 1 - Save an assembly with 1 interface proxy
			//
			var proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "3", "ProxyCache3.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "3", "ProxyCache3.dll");
			var proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			var generator = new ProxyGenerator(proxyBuilder);
			generator.CreateInterfaceProxyWithTargetInterface(typeof(IInterface3), new[] { typeof(IInterface2) }, new Class3(), new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 2 - Save another assembly with 1 interface proxy
			// note : to reproduce the problem, must load previously saved assembly (in cache) before saving this assembly.
			//
			proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "4", "ProxyCache4.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "4", "ProxyCache4.dll");
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			var proxyAssembly = Assembly.LoadFrom("ProxyCache3.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			generator.CreateInterfaceProxyWithTargetInterface(typeof(IInterface3), new[] { typeof(IInterface1) }, new Class4(), new DoNothingInterceptor());
			proxyGeneratorModuleScope.SaveAssembly();

			//
			// Step 3 - Load the last proxy assembly and try to create the first interface proxy (created in step 1)
			// note : Creating proxy from step 2 works.
			// issue : returns the wrong proxy (the one from step 2)
			//
			proxyGeneratorModuleScope = new ModuleScope(true);
			proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
			generator = new ProxyGenerator(proxyBuilder);

			proxyAssembly = Assembly.LoadFrom("ProxyCache4.dll");
			proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

			var invalidProxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IInterface3), new[] { typeof(IInterface2) }, new Class3(), new DoNothingInterceptor());
			if (invalidProxy as IInterface2 == null)
			{
				Assert.Fail();
			}
		}
	}
}

#endif