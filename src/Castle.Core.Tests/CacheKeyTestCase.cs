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
	using System.Reflection;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.Classes;
	using NUnit.Framework;

	[TestFixture]
	public class CacheKeyTestCase
	{
		[Test]
		public void InstanceEquivalence()
		{
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass), null, ProxyGenerationOptions.Default);

			Assert.AreEqual(key1, key2);

			key1 = new CacheKey(typeof (NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof (NonPublicConstructorClass), null, new ProxyGenerationOptions());

			Assert.AreEqual(key1, key2);
		}

		[Test]
		public void InstanceEquivalence_WithInterfaces()
		{
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);

			Assert.AreEqual(key1, key2);

			key1 =
				new CacheKey(typeof (NonPublicConstructorClass), new Type[] {typeof (IDisposable)}, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof (NonPublicConstructorClass), new Type[] {typeof (IDisposable)}, ProxyGenerationOptions.Default);

			Assert.AreEqual(key1, key2);
		}

		[Test]
		public void DifferentKeys()
		{
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicMethodsClass), null, ProxyGenerationOptions.Default);

			Assert.AreNotEqual(key1, key2);

			key1 =
				new CacheKey(typeof (NonPublicConstructorClass), new Type[] {typeof (IDisposable)}, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof (NonPublicConstructorClass), new Type[] {typeof (IConvertible)}, ProxyGenerationOptions.Default);

			Assert.AreNotEqual(key1, key2);

			key1 =
				new CacheKey(typeof (NonPublicConstructorClass), new Type[] {typeof (IDisposable)}, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof (NonPublicMethodsClass), new Type[] {typeof (IDisposable)}, ProxyGenerationOptions.Default);

			Assert.AreNotEqual(key1, key2);
		}

		[Test]
		public void DifferentOptions()
		{
			ProxyGenerationOptions options1 = new ProxyGenerationOptions();
			ProxyGenerationOptions options2 = new ProxyGenerationOptions();
			options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass), null, options1);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass), null, options2);

			Assert.AreNotEqual(key1, key2);

			options1 = new ProxyGenerationOptions();
			options2 = new ProxyGenerationOptions();
			options2.Selector = new AllInterceptorSelector();
			key1 = new CacheKey(typeof (NonPublicConstructorClass), null, options1);
			key2 = new CacheKey(typeof (NonPublicConstructorClass), null, options2);

			Assert.AreNotEqual(key1, key2);
		}

		[Test]
		public void EquivalentOptions()
		{
			ProxyGenerationOptions options1 = new ProxyGenerationOptions();
			ProxyGenerationOptions options2 = new ProxyGenerationOptions();

			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass), null, options1);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass), null, options2);

			Assert.AreEqual(key1, key2);
		}

		[Test]
		public void EqualWithProxyForType()
		{
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);

			Assert.AreEqual(key1, key2);

			CacheKey key3 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.AreNotEqual(key1, key3);
			Assert.AreNotEqual(key3, key1);

			CacheKey key4 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.AreEqual(key4, key3);
			Assert.AreEqual(key3, key4);
		}

		[Test]
		public void EqualNullAndEmptyInterfaces()
		{
			CacheKey key1 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			CacheKey key2 = new CacheKey(typeof (NonPublicConstructorClass).GetTypeInfo(), null, Type.EmptyTypes,
			                             ProxyGenerationOptions.Default);

			Assert.AreEqual(key1, key2);
			Assert.AreEqual(key2, key1);
		}
	}
}