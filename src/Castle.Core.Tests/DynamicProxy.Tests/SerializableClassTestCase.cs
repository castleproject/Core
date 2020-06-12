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
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.InterClasses;

	using NUnit.Framework;

	[TestFixture]
	public class SerializableClassTestCase : BasePEVerifyTestCase
	{
		[Test(Description = "DYNPROXY-133")]
		public void Can_proxy_class_with_explicit_GetObjectData()
		{
			generator.CreateClassProxy<SerializableExplicitImpl>();
		}

		[Test]
		public void Proxy_class_is_not_serializable_even_when_proxied_class_is()
		{
			// DynamicProxy 4 used to mark proxy classes as serializable.
			// This test is here to explicitly document that this no longer happens.

			var proxy = generator.CreateClassProxy<SerializableClass>(new StandardInterceptor());

			Assert.IsFalse(proxy.GetType().IsSerializable);
		}

		[Test]
		public void Proxy_class_does_not_implement_ISerializable()
		{
			// DynamicProxy 4 used to auto-implement `ISerializable`.
			// This test is here to explicitly document that this no longer happens.

			var proxy = generator.CreateClassProxy<SerializableClass>(new StandardInterceptor());

			Assert.IsFalse(proxy is ISerializable);
		}
	}
}
