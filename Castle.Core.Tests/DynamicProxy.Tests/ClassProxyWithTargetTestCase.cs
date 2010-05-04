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
	using System.Collections;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyWithTargetTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Can_proxy_purely_virtual_class()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithMethod());
			var result = proxy.Method();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void Can_proxy_class_with_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedMethod(42));
			var result = proxy.PublicMethod();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void Can_proxy_class_with_protected_generic_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedGenericMethod(42));
			var result = proxy.PublicMethod<int>();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void Can_proxy_class_with_no_default_ctor()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithNoDefaultCtor(42), new object[] { 12 });
			var result = proxy.Method();
			Assert.AreEqual(42, result);
		}

		[Test]
		public void Hook_gets_notified_about_public_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithPublicField), false);
			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithPublicField), Type.EmptyTypes,
			                                     new VirtualClassWithPublicField(), new ProxyGenerationOptions(hook),
			                                     new object[0]);
			Assert.IsNotEmpty((ICollection)hook.NonVirtualMembers);
			var memberInfo = hook.NonVirtualMembers.Single(m => m is FieldInfo);
			Assert.AreEqual("field", memberInfo.Name);
			Assert.AreEqual(MemberTypes.Field, memberInfo.MemberType);
		}

		[Test]
		public void Hook_does_NOT_get_notified_about_autoproperty_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithAutoProperty), false);

			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithAutoProperty), Type.EmptyTypes,
												 new VirtualClassWithAutoProperty(), new ProxyGenerationOptions(hook),
												 new object[0]);

			Assert.False(hook.NonVirtualMembers.Any(m => m is FieldInfo));
		}
	}
}