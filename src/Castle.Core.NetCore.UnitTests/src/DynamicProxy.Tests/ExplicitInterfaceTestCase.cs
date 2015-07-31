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

	using Castle.DynamicProxy.Tests.Explicit;
	using Castle.DynamicProxy.Tests.GenInterfaces;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using Xunit;

#if !SILVERLIGHT && !NETCORE
	public class ExplicitInterfaceTestCase : BasePEVerifyTestCase
	{
		private LogInvocationInterceptor interceptor;

		[Fact]
		public void Can_proxy_class_with_two_explicit_methods_differing_only_by_return_type()
		{
			generator.CreateClassProxy(typeof(TwoInterfacesExplicit), new[] { typeof(ISimpleInterface), typeof(IDisposable) },
				interceptor);
		}

		[Fact]
		public void ExplicitGenericInterface()
		{
			var proxy = (GenInterface<int>)generator.CreateClassProxy(typeof(GenInterfaceExplicit),
				new[] { typeof(GenInterface<int>) },
				interceptor);

			var result = proxy.DoSomething(4);

			Assert.Equal(5, result);
			Assert.Equal("DoSomething ", interceptor.LogContents);
		}

		[Fact]
		public void ExplicitGenericMethod_with_base_call()
		{
			var proxy = (IGenericInterface)generator.CreateClassProxy(typeof(GenericMethodExplicit),
				new[] { typeof(IGenericInterface) },
				interceptor);

			var result = proxy.GenericMethod<int>();

			Assert.Equal(7, result);
			Assert.Equal("GenericMethod ", interceptor.LogContents);
		}

		[Fact]
		public void ExplicitGenericMethod_without_base_call()
		{
			var proxy = (IGenericInterface)generator.CreateClassProxy(typeof(GenericMethodExplicit),
				new[] { typeof(IGenericInterface) },
				interceptor,
				new SetReturnValueInterceptor(5));

			var result = proxy.GenericMethod<int>();

			Assert.Equal(5, result);
			Assert.Equal("GenericMethod ", interceptor.LogContents);
		}

		[Fact]
		public void ExplicitInterface_AsAdditionalInterfaceToProxy_OnClassProxy_WithBaseCalls()
		{
			var proxy = (ISimpleInterface)generator.CreateClassProxy(typeof(SimpleInterfaceExplicit),
				new[] { typeof(ISimpleInterface) },
				interceptor);

			var result = proxy.Do();

			Assert.Equal(1, interceptor.Invocations.Count);
			Assert.Equal("Do", interceptor.Invocations[0]);
			Assert.Equal(5, result); // indicates that original method was called
		}

		[Fact]
		public void ExplicitInterface_AsAdditionalInterfaceToProxy_OnClassProxy_WithoutBaseCalls()
		{
			interceptor.Proceed = false;

			var proxy = (SimpleInterfaceExplicit)generator.CreateClassProxy(typeof(SimpleInterfaceExplicit),
				new[] { typeof(ISimpleInterface) },
				interceptor);

			proxy.DoVirtual();
			var result = ((ISimpleInterface)proxy).Do();
			proxy.DoVirtual();

			Assert.Equal(3, interceptor.Invocations.Count);
			Assert.Equal("DoVirtual", interceptor.Invocations[0]);
			Assert.Equal("Do", interceptor.Invocations[1]);
			Assert.Equal("DoVirtual", interceptor.Invocations[2]);

			Assert.Equal(0, result); // indicates that original method was not called
		}

		[Fact]
		public void ExplicitInterface_properties_should_be_public_class()
		{
			var proxy = generator.CreateClassProxy(typeof(ExplicitInterfaceWithPropertyImplementation),
				new[] { typeof(ISimpleInterfaceWithProperty) },
				interceptor);
			Assert.NotEmpty(proxy.GetType().GetProperties());
		}

		[Fact]
		public void ExplicitInterface_properties_should_be_public_interface()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterfaceWithProperty), interceptor);
			Assert.NotEmpty(proxy.GetType().GetProperties());
		}

		[Fact]
		public void ExplicitMethodOutArguments()
		{
			var proxy = (IWithRefOut)generator.CreateClassProxy(typeof(WithRefOutExplicit),
				new[] { typeof(IWithRefOut) },
				interceptor);

			int result;
			proxy.Do(out result);

			Assert.Equal(5, result);
			Assert.Equal("Do ", interceptor.LogContents);
		}

		[Fact]
		public void ExplicitMethodRefArguments()
		{
			var proxy = (IWithRefOut)generator.CreateClassProxy(typeof(WithRefOutExplicit),
				new[] { typeof(IWithRefOut) },
				interceptor);

			var result = 0;
			proxy.Did(ref result);

			Assert.Equal(5, result);
			Assert.Equal("Did ", interceptor.LogContents);
		}

		public ExplicitInterfaceTestCase()
		{
			interceptor = new LogInvocationInterceptor();
		}

		[Fact]
		public void NonVirtualExplicitInterfaceMethods_AreIgnored_OnClassProxy()
		{
			var instance = generator.CreateClassProxy<SimpleInterfaceExplicit>(interceptor);

			instance.DoVirtual();
			var result = ((ISimpleInterface)instance).Do();
			instance.DoVirtual();

			Assert.Equal(2, interceptor.Invocations.Count);
			Assert.Equal("DoVirtual", interceptor.Invocations[0]);
			Assert.Equal("DoVirtual", interceptor.Invocations[1]);

			Assert.Equal(5, result);
		}
	}
#endif

	public class ExplicitInterfaceWithPropertyImplementation : ISimpleInterfaceWithProperty
	{
		public int Age
		{
			get { throw new NotImplementedException(); }
		}
	}
}