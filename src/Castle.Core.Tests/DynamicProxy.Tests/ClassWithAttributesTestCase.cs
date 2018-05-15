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
	using System.IO;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class ClassWithAttributesTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods()
		{
			HasNonInheritableAttribute instance = (HasNonInheritableAttribute)
										generator.CreateClassProxy(typeof (HasNonInheritableAttribute), new StandardInterceptor());

			object[] attributes = instance.GetType().GetTypeInfo().GetCustomAttributes(typeof (NonInheritableAttribute), false).ToArray();
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOf(typeof (NonInheritableAttribute), attributes[0]);

			attributes = instance.GetType().GetMethod("OnMethod").GetCustomAttributes(typeof (NonInheritableAttribute), false).ToArray();
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOf(typeof (NonInheritableAttribute), attributes[0]);
		}

		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods_ComplexAttributes()
		{
			AttributedClass2 instance = (AttributedClass2)
										generator.CreateClassProxy(typeof (AttributedClass2), new StandardInterceptor());

			object[] attributes = instance.GetType().GetTypeInfo().GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false).ToArray();
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOf(typeof (ComplexNonInheritableAttribute), attributes[0]);
			ComplexNonInheritableAttribute att = (ComplexNonInheritableAttribute) attributes[0];
			// (1, 2, true, "class", FileAccess.Write)
			Assert.AreEqual(1, att.Id);
			Assert.AreEqual(2, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("class", att.Name);
			Assert.AreEqual(FileAccess.Write, att.Access);

			attributes = instance.GetType().GetMethod("Do1").GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false).ToArray();
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOf(typeof (ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (2, 3, "Do1", Access = FileAccess.ReadWrite)
			Assert.AreEqual(2, att.Id);
			Assert.AreEqual(3, att.Num);
			Assert.AreEqual(false, att.IsSomething);
			Assert.AreEqual("Do1", att.Name);
			Assert.AreEqual(FileAccess.ReadWrite, att.Access);

			attributes = instance.GetType().GetMethod("Do2").GetCustomAttributes(typeof (ComplexNonInheritableAttribute), false).ToArray();
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOf(typeof (ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (3, 4, "Do2", IsSomething=true)
			Assert.AreEqual(3, att.Id);
			Assert.AreEqual(4, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("Do2", att.Name);
		}

		[Test]
		public void EnsureProxyHasAttributesOnProperties()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			var nameProperty = proxy.GetType().GetProperty("OnProperty");
			Assert.IsTrue(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}

		[Test, Ignore("Not supported. Is it possible? There seems to be no API to allow that.")]
		public void EnsureProxyHasAttributesOnOnReturn()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			var nameProperty = proxy.GetType().GetMethod("OnReturn").ReturnParameter;
			Assert.IsTrue(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}

		[Test]
		public void EnsureProxyHasAttributesOnParameter()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			ParameterInfo nameProperty = proxy.GetType().GetMethod("OnParameter").GetParameters().Single();
			Assert.IsTrue(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}

		[Test]
		[ExcludeOnFramework(Framework.Mono, "Mono does not currently emit custom attributes on generic type parameters of methods. See https://github.com/mono/mono/issues/8512.")]
		public void EnsureProxyHasAttributesOnGenericArgument()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			var nameProperty = proxy.GetType().GetMethod("OnGenericArgument").GetGenericArguments().Single();
			Assert.IsTrue(nameProperty.GetTypeInfo().IsDefined(typeof(NonInheritableAttribute), false));
		}

		[Test]
		public void Can_proxy_type_with_non_inheritable_attribute_depending_on_array_of_something_via_property()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHasNonInheritableAttributeWithArray>();
			var attribute = proxy.GetType().GetTypeInfo()
				.GetCustomAttributes(typeof(NonInheritableWithArrayAttribute), false)
				.Cast<NonInheritableWithArrayAttribute>().Single();
			CollectionAssert.AreEqual(attribute.Values, new[] {"1", "2", "3"});
		}

		[Test]
		public void Can_proxy_type_with_non_inheritable_attribute_depending_on_array_of_something_via_field()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHasNonInheritableAttributeWithArray2>();
			var attribute = proxy.GetType().GetTypeInfo()
				.GetCustomAttributes(typeof(NonInheritableWithArray2Attribute), false)
				.Cast<NonInheritableWithArray2Attribute>().Single();
			CollectionAssert.AreEqual(attribute.Values, new[] { "1", "2", "3" });
		}
	}
}