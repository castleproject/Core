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
	using Castle.DynamicProxy.Tests.Classes;

	using Xunit;

	using System.Reflection;

	public class CacheKeyTestCase
	{
		[Fact]
		public void InstanceEquivalence()
		{
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);

			key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof(NonPublicConstructorClass), null, new ProxyGenerationOptions());

			Assert.Equal(key1, key2);
		}

		[Fact]
		public void InstanceEquivalence_WithInterfaces()
		{
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new Type[] { typeof(IDisposable) }, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof(NonPublicConstructorClass), new Type[] { typeof(IDisposable) }, ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);
		}

		[Fact]
		public void DifferentKeys()
		{
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof(NonPublicMethodsClass), null, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new Type[] { typeof(IDisposable) }, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof(NonPublicConstructorClass), new Type[] { typeof(IConvertible) }, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new Type[] { typeof(IDisposable) }, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof(NonPublicMethodsClass), new Type[] { typeof(IDisposable) }, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);
		}

		[Fact]
		public void DifferentOptions()
		{
			ProxyGenerationOptions options1 = new ProxyGenerationOptions();
			ProxyGenerationOptions options2 = new ProxyGenerationOptions();
			options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.NotEqual(key1, key2);

			options1 = new ProxyGenerationOptions();
			options2 = new ProxyGenerationOptions();
			options2.Selector = new AllInterceptorSelector();
			key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.NotEqual(key1, key2);
		}

		[Fact]
		public void EquivalentOptions()
		{
			ProxyGenerationOptions options1 = new ProxyGenerationOptions();
			ProxyGenerationOptions options2 = new ProxyGenerationOptions();

			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.Equal(key1, key2);
		}

		[Fact]
		public void EqualWithProxyForType()
		{
#if NETCORE
            CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
#else
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), null, null, ProxyGenerationOptions.Default);
#endif

			Assert.Equal(key1, key2);

			CacheKey key3 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.NotEqual(key1, key3);
			Assert.NotEqual(key3, key1);

			CacheKey key4 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.Equal(key4, key3);
			Assert.Equal(key3, key4);
		}

		[Fact]
		public void EqualNullAndEmptyInterfaces()
		{
#if NETCORE
            CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, Type.EmptyTypes, ProxyGenerationOptions.Default);
#else
			CacheKey key1 = new CacheKey(typeof(NonPublicConstructorClass), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof(NonPublicConstructorClass), null, Type.EmptyTypes, ProxyGenerationOptions.Default);
#endif

			Assert.Equal(key1, key2);
			Assert.Equal(key2, key1);
		}
	}
}