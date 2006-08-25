// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Test
{
	using System;
	using Castle.DynamicProxy.Test.Classes;
	using Castle.DynamicProxy.Test.Interceptors;
	
	using NUnit.Framework;

	[TestFixture]
	public class BasicClassProxyTestCase
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void ProxyForClass()
		{
			object proxy = generator.CreateClassProxy(
				typeof(ServiceClass), new ResultModifierInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));

			ServiceClass instance = (ServiceClass) proxy;

			// return value is changed by the interceptor
			Assert.AreEqual(44, instance.Sum(20, 25));
			
			// return value is changed by the interceptor
			Assert.AreEqual(true, instance.Valid);
			
			// The rest aren't changed
			Assert.AreEqual(45, instance.Sum((byte)20, (byte)25)); // byte
			Assert.AreEqual(45, instance.Sum((long)20, (long)25)); // long
			Assert.AreEqual(45, instance.Sum((short)20, (short)25)); // short
			Assert.AreEqual(45, instance.Sum((float)20, (float)25)); // float
			Assert.AreEqual(45, instance.Sum((double)20, (double)25)); // double
			Assert.AreEqual(45, instance.Sum((ushort)20, (ushort)25)); // ushort
			Assert.AreEqual(45, instance.Sum((uint)20, (uint)25)); // uint
			Assert.AreEqual(45, instance.Sum((ulong)20, (ulong)25)); // ulong
		}

		[Test]
		public void ProxyForClassWithInterfaces()
		{
			object proxy = generator.CreateClassProxy(typeof(ServiceClass), new Type[] { typeof(IDisposable) },
				new ResultModifierInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(ServiceClass).IsAssignableFrom(proxy.GetType()));
			Assert.IsTrue(typeof(IDisposable).IsAssignableFrom(proxy.GetType()));

			ServiceClass inter = (ServiceClass)proxy;

			Assert.AreEqual(44, inter.Sum(20, 25));
			Assert.AreEqual(true, inter.Valid);
		}

	}
}