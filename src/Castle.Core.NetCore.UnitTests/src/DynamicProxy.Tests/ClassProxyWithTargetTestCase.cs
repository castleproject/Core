// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Mixins;

	using CastleTests;

	using Xunit;

		public class ClassProxyWithTargetTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void Can_proxy_class_with_no_default_ctor()
		{
			var proxy = generator.CreateClassProxyWithTarget(typeof(VirtualClassWithNoDefaultCtor),
			                                                 new VirtualClassWithNoDefaultCtor(42),
			                                                 new object[] {12});
			var result = ((VirtualClassWithNoDefaultCtor) proxy).Method();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_virtual_class_with_protected_generic_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedGenericMethod(42));
			var result = proxy.PublicMethod<int>();
			Assert.Equal(42, result);
		}

#if !SILVERLIGHT
		[Fact]
		[Bug("DYNPROXY-170")]
		public void Can_proxy_class_with_protected_generic_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassWithProtectedGenericMethod(42));
			var result = proxy.PublicMethod<int>();
			Assert.Equal(42, result);
		}
#endif

		[Fact]
		public void Can_proxy_virtual_class_with_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedMethod(42));
			var result = proxy.PublicMethod();
			Assert.Equal(42, result);
		}
		
#if !SILVERLIGHT
		[Fact]
		[Bug("DYNPROXY-170")]
		public void Can_proxy_class_with_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassWithProtectedMethod(42));
			var result = proxy.PublicMethod();
			Assert.Equal(42, result);
		}
#endif

		[Fact]
		public void Can_proxy_class_with_two_protected_methods_differing_by_return_type()
		{
			generator.CreateClassProxyWithTarget(new HasTwoProtectedMethods());
		}

		[Fact]
		public void Can_proxy_purely_virtual_class()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithMethod());
			var result = proxy.Method();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_purely_virtual_inherited_abstract_class()
		{
			var proxy = generator.CreateClassProxyWithTarget<AbstractClassWithMethod>(new InheritsAbstractClassWithMethod());
			var result = proxy.Method();
			Assert.Equal(42, result);
		}

#if !NETCORE // TODO: Replace Assert.That...
		[Fact]
		public void Cannot_proxy_inaccessible_class()
		{
			var exception = Assert.Throws<GeneratorException>(() => generator.CreateClassProxyWithTarget<PrivateClass>(new PrivateClass()));
			Assert.That(exception.Message, Is.StringStarting("Can not create proxy for type Castle.DynamicProxy.Tests.ClassProxyWithTargetTestCase+PrivateClass because it is not accessible. Make it public, or internal"));
		}

		[Fact]
		public void Cannot_proxy_generic_class_with_inaccessible_type_argument()
		{
			var exception = Assert.Throws<GeneratorException>(() => generator.CreateClassProxyWithTarget<List<PrivateClass>>(new List<PrivateClass>()));
			Assert.That(exception.Message, Is.StringStarting("Can not create proxy for type System.Collections.Generic.List`1[[Castle.DynamicProxy.Tests.ClassProxyWithTargetTestCase+PrivateClass, Castle.Core.Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc]] because type Castle.DynamicProxy.Tests.ClassProxyWithTargetTestCase+PrivateClass is not accessible. Make it public, or internal"));
		}
#endif

		[Fact]
		public void Cannot_proxy_generic_class_with_type_argument_that_has_inaccessible_type_argument()
		{
			var expected = string.Format("Can not create proxy for type {0} because type {1} is not accessible. Make it public, or internal",
					typeof(List<List<PrivateClass>>).FullName, typeof(PrivateClass).FullName);

			var exception = Assert.Throws<GeneratorException>(() => generator.CreateClassProxyWithTarget(new List<List<PrivateClass>>(), new IInterceptor[0]));

			Assert.StartsWith(expected, exception.Message);
		}

		[Fact]
		public void Can_proxy_generic_class()
		{
			generator.CreateClassProxyWithTarget<List<object>>(new List<object>());
		}

		[Fact]
		public void Can_proxy_with_target_after_proxy_without_target_for_the_same_type()
		{
			generator.CreateClassProxy<SimpleClass>();

			generator.CreateClassProxyWithTarget(new SimpleClass());
		}

		[Fact]
		public void Hook_does_NOT_get_notified_about_autoproperty_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithAutoProperty), false);

			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithAutoProperty), Type.EmptyTypes,
			                                     new VirtualClassWithAutoProperty(), new ProxyGenerationOptions(hook),
			                                     new object[0]);

			Assert.False(hook.NonVirtualMembers.Any(m => m is FieldInfo));
		}

		[Fact]
		public void Hook_gets_notified_about_public_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithPublicField), false);
			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithPublicField), Type.EmptyTypes,
			                                     new VirtualClassWithPublicField(), new ProxyGenerationOptions(hook),
			                                     new object[0]);
			Assert.NotEmpty((ICollection) hook.NonVirtualMembers);
			var memberInfo = hook.NonVirtualMembers.Single(m => m is FieldInfo);
			Assert.Equal("field", memberInfo.Name);
#if !NETCORE
			Assert.Equal(MemberTypes.Field, memberInfo.MemberType);
#endif
		}

		[Fact]
		public void Hook_gets_notified_about_static_methods()
		{
			var hook = new LogHook(typeof(VirtualClassWithPublicField), false);
			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithPublicField), Type.EmptyTypes,
			                                     new VirtualClassWithPublicField(), new ProxyGenerationOptions(hook),
			                                     new object[0]);
			Assert.NotEmpty((ICollection) hook.NonVirtualMembers);
			var memberInfo = hook.NonVirtualMembers.Single(m => m is FieldInfo);
			Assert.Equal("field", memberInfo.Name);
#if !NETCORE
			Assert.Equal(MemberTypes.Field, memberInfo.MemberType);
#endif
		}

		[Fact]
		public void Uses_The_Provided_Options()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SimpleMixin());

			var target = new SimpleClassWithProperty();
			var proxy = generator.CreateClassProxyWithTarget(target, options);

			Assert.IsAssignableFrom(typeof(ISimpleMixin), proxy);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		[Bug("DYNPROXY-185")]
		public void Returns_proxy_target_instead_of_self()
		{
			var target = new EmptyClass();
			var proxy = generator.CreateClassProxyWithTarget(target);
			var result = (EmptyClass)((IProxyTargetAccessor)proxy).DynProxyGetTarget();
			Assert.Equal(target, result);
		}
#endif

		private class PrivateClass { }
	}
}