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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.Interceptors;

	using Xunit;

	using System.Reflection;

	public class DelegateProxyTestCasE : BasePEVerifyTestCase
	{
		private Type GenerateProxyType<TDelegate>()
		{
			var scope = generator.ProxyBuilder.ModuleScope;
			var proxyGenerator = new DelegateProxyGenerator(scope, typeof(TDelegate))
			{
				Logger = generator.ProxyBuilder.Logger
			};
			return proxyGenerator.GetProxyType();
		}

		private T GetProxyInstance<T>(T func, params IInterceptor[] interceptors)
		{
			var type = GenerateProxyType<T>();
			var instance = Activator.CreateInstance(type, func, interceptors);
#if NETCORE
            var method = instance.GetType().GetMethod("Invoke");
            return (T)(object)method.CreateDelegate(typeof(T), instance);
#else
			return (T)(object)Delegate.CreateDelegate(typeof(T), instance, "Invoke");
#endif
		}

		[Fact]
		public void Can_create_Delegate_type_proxy()
		{
			var type = GenerateProxyType<Func<int>>();
			Assert.NotNull(type);
		}

		[Fact]
		public void Can_intercept_call_to_delegate()
		{
			var proxy = GetProxyInstance<Func<int>>(() =>
			{
				Assert.True(false, "Shouldn't have gone that far");
				return 5;
			}, new SetReturnValueInterceptor(3));
			var result = proxy.Invoke();
			Assert.Equal(3, result);
		}

		[Fact]
		public void Can_intercept_call_to_delegate_no_target()
		{
			var proxy = GetProxyInstance<Func<int>>(null, new SetReturnValueInterceptor(3));
			var result = proxy.Invoke();
			Assert.Equal(3, result);
		}

		[Fact]
		public void Can_proxy_delegate_with_no_target()
		{
			var proxy = GetProxyInstance<Func<int>>(() => 5);
			var result = proxy.Invoke();
			Assert.Equal(5, result);
		}
	}
}