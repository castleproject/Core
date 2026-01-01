// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class ProxyTypeCachingWithDifferentHooksTestCase : BasePEVerifyTestCase
	{
#if FEATURE_SERIALIZATION
		[Serializable]
#endif
		public class CustomHook : AllMethodsHook { }

#if FEATURE_SERIALIZATION
		[Serializable]
#endif
		public record class RecordClassHook : IProxyGenerationHook
		{
			public RecordClassHook(string id)
			{
				Id = id;
			}

			public string Id { get; }

			public void MethodsInspected() { }
			public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) { }
			public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo) => false;
		}

		[Test]
		public void Proxies_with_same_hook_will_use_cached_proxy_type()
		{
			var first = CreateProxyWithHook<CustomHook>();
			var second = CreateProxyWithHook<CustomHook>();
			Assert.AreEqual(first.GetType(), second.GetType());
		}

		[Test]
		public void Proxies_with_different_hooks_will_use_different_proxy_types()
		{
			var first = CreateProxyWithHook<AllMethodsHook>();
			var second = CreateProxyWithHook<CustomHook>();
			Assert.AreNotEqual(first.GetType(), second.GetType());
		}

		[Test]
		public void Proxies_with_different_but_equal_record_class_hooks_will_use_cached_proxy_type()
		{
			var firstHook = new RecordClassHook("1");
			var secondHook = new RecordClassHook("1");
			Assume.That(firstHook, Is.Not.SameAs(secondHook));
			Assume.That(firstHook, Is.EqualTo(secondHook));

			var first = CreateProxyWithHook(firstHook);
			var second = CreateProxyWithHook(secondHook);

			Assert.AreEqual(first.GetType(), second.GetType());
		}

		[Test]
		public void Proxies_with_different_and_unequal_record_class_hooks_will_use_different_proxy_types()
		{
			var firstHook = new RecordClassHook("1");
			var secondHook = new RecordClassHook("2");
			Assume.That(firstHook, Is.Not.SameAs(secondHook));
			Assume.That(firstHook, Is.Not.EqualTo(secondHook));

			var first = CreateProxyWithHook(firstHook);
			var second = CreateProxyWithHook(secondHook);

			Assert.AreNotEqual(first.GetType(), second.GetType());
		}

		private object CreateProxyWithHook<THook>() where THook : IProxyGenerationHook, new()
		{
			return CreateProxyWithHook(new THook());
		}

		private object CreateProxyWithHook(IProxyGenerationHook hook)
		{
			return generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), new ProxyGenerationOptions(hook), new DoNothingInterceptor());
		}
	}
}