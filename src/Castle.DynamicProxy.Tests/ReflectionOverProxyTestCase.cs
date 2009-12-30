// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.ComponentModel;

	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class ReflectionOverProxyTestCase:BasePEVerifyTestCase
	{
		[Test]
		public void Proxy_should_have_TypeDescriptionProviderAttribute()
		{
			var type = generator.CreateClassProxy(typeof(SimpleClass)).GetType();
			Assert.IsTrue(Attribute.IsDefined(type, typeof(TypeDescriptionProviderAttribute)));

			type = generator.CreateInterfaceProxyWithoutTarget(typeof(IOne)).GetType();
			Assert.IsTrue(Attribute.IsDefined(type, typeof(TypeDescriptionProviderAttribute)));

			type = generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne),new One()).GetType();
			Assert.IsTrue(Attribute.IsDefined(type, typeof(TypeDescriptionProviderAttribute)));

			type = generator.CreateInterfaceProxyWithTarget(typeof(IOne),new One()).GetType();
			Assert.IsTrue(Attribute.IsDefined(type, typeof(TypeDescriptionProviderAttribute)));
		}

		[Test]
		public void Proxy_with_explicitly_implemented_interface_should_return_short_names_via_descriptor()
		{
			var type = generator.CreateClassProxy(typeof(HasPropertyBar), new[] { typeof(IHasProperty) }).GetType();
			var properties = TypeDescriptor.GetProperties(type);
			Assert.IsNotEmpty(properties);
			foreach (PropertyDescriptor property in properties)
			{
				Assert.That(property.Name.IndexOf(".") == -1, "Name should have no dots: {0}", property.Name);
			}
		}

		[Test]
		public void Should_be_able_to_set_get_interface_property_with_short_name_via_TypeDescriptor()
		{
			var proxy = generator.CreateClassProxy(typeof(HasPropertyBar), new[] { typeof(IHasProperty) }) as IHasProperty;
			var properties = TypeDescriptor.GetProperties(proxy);
			var property = properties.Find("Prop", true);
			property.SetValue(proxy, 7);
			Assert.AreEqual(7, proxy.Prop);
		}

	}
}