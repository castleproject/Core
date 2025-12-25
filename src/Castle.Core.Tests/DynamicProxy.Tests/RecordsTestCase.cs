// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Records;

	using NUnit.Framework;

	[TestFixture]
	public class RecordsTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Can_proxy_empty_record()
		{
			_ = generator.CreateClassProxy<EmptyRecord>(new StandardInterceptor());
		}

		[Test]
		public void Can_proxy_record_derived_from_empty_record()
		{
			_ = generator.CreateClassProxy<DerivedFromEmptyRecord>(new StandardInterceptor());
		}

		[Test]
		public void Can_proxy_empty_generic_record()
		{
			_ = generator.CreateClassProxy<EmptyGenericRecord<object>>(new StandardInterceptor());
		}

		[Test]
		public void Can_proxy_record_derived_from_empty_generic_record()
		{
			_ = generator.CreateClassProxy<DerivedFromEmptyGenericRecord>(new StandardInterceptor());
		}

		[Test]
		public void Cloning_proxied_record_preserves_proxy_type()
		{
			var proxy = generator.CreateClassProxy<EmptyRecord>();
			var clonedProxy = proxy with { };
			Assert.True(ProxyUtil.IsProxy(clonedProxy));
			Assert.AreSame(proxy.GetType(), clonedProxy.GetType());
		}

		[Test]
		public void Cloning_proxied_record_preserves_proxy_type_even_when_not_intercepting()
		{
			var proxy = generator.CreateClassProxy<EmptyRecord>(new ProxyGenerationOptions(new InterceptNothingHook()));
			var clonedProxy = proxy with { };
			Assert.True(ProxyUtil.IsProxy(clonedProxy));
			Assert.AreSame(proxy.GetType(), clonedProxy.GetType());
		}

		[Test]
		public void Can_intercept_clone_method()
		{
			var expectedClone = new DerivedFromEmptyRecord();
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				if (invocation.Method.Name == "<Clone>$")
				{
					invocation.ReturnValue = expectedClone;
				}
			});
			var proxy = generator.CreateClassProxy<EmptyRecord>(interceptor);
			var actualClone = proxy with { };
			Assert.AreSame(expectedClone, actualClone);
		}

		[Test]
		public void Can_proceed_for_intercepted_clone_method()
		{
			var interceptor = new WithCallbackInterceptor(invocation =>
			{
				if (invocation.Method.Name == "<Clone>$")
				{
					invocation.Proceed();
				}
			});
			var proxy = generator.CreateClassProxy<EmptyRecord>(interceptor);
			var clonedProxy = proxy with { };
			Assert.AreSame(proxy.GetType(), clonedProxy.GetType());
		}

		[Serializable]
		public record class InterceptNothingHook : IProxyGenerationHook
		{
			public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo) => false;

			void IProxyGenerationHook.MethodsInspected() { }
			void IProxyGenerationHook.NonProxyableMemberNotification(Type type, MemberInfo memberInfo) { }
		}
	}
}
