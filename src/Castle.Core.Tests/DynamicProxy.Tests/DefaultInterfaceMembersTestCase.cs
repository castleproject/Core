// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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

#if NETCOREAPP3_0_OR_GREATER

using System.Reflection;

using Castle.DynamicProxy.Tests.Interceptors;
using Castle.DynamicProxy.Tests.Interfaces;

using NUnit.Framework;

namespace Castle.DynamicProxy.Tests
{
	[TestFixture]
	public class DefaultInterfaceMembersTestCase : BasePEVerifyTestCase
	{
		#region Methods with default implementation

		[Test]
		public void Can_proxy_class_that_inherits_method_with_default_implementation_from_interface()
		{
			_ = generator.CreateClassProxy<InheritsMethodWithDefaultImplementation>();
		}

		[Test]
		public void Can_proxy_interface_with_method_with_default_implementation()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithDefaultImplementation>();
		}

		[Test]
		public void Default_implementation_gets_called_when_method_not_intercepted_in_proxied_class()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateClassProxy<InheritsMethodWithDefaultImplementation>(options);
			var expected = "default implementation";
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Default_implementation_gets_called_when_method_not_intercepted_in_proxied_interface()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithDefaultImplementation>(options);
			var expected = "default implementation";
			var actual = proxy.MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_method_with_default_implementation_in_proxied_class()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateClassProxy<InheritsMethodWithDefaultImplementation>(interceptor);
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_method_with_default_implementation_in_proxied_interface()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithDefaultImplementation>(interceptor);
			var actual = proxy.MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_method_default_implementation_in_proxied_class()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateClassProxy<InheritsMethodWithDefaultImplementation>(interceptor);
			var expected = "default implementation";
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_method_default_implementation_in_proxied_interface()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithDefaultImplementation>(interceptor);
			var expected = "default implementation";
			var actual = proxy.MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Generic methods with default implementation

		[Test]
		public void Can_proceed_to_generic_method_default_implementation_in_proxied_interface()
		{
			// (This test targets the code regarding generics inside `InterfaceProxyWithoutTargetContributor.CreateCallbackMethod`.)

			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveGenericMethodWithDefaultImplementation>(interceptor);
			var expected = "default implementation for Int32";
			var actual = proxy.GenericMethodWithDefaultImplementation<int>();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Methods with "overridden" default implementation

		[Test]
		public void Can_intercept_method_with_overridden_default_implementation_in_proxied_class()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateClassProxy<OverridesMethodWithDefaultImplementation>(interceptor);
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Siblings of methods with default implementation

		[Test]
		public void Can_intercept_method_that_is_sibling_of_method_with_default_implementation_in_proxied_class()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateClassProxy<InheritsMethodWithDefaultImplementationAmongOtherThings>(interceptor);
			var actual = proxy.Method();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_method_that_is_sibling_of_method_with_default_implementation_in_proxied_interface()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveMethodWithDefaultImplementationAmongOtherThings>(interceptor);
			var actual = proxy.Method();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_base_implementation_of_method_that_is_sibling_of_method_with_default_implementation_in_proxied_class()
		{
			var expected = "class implementation";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateClassProxy<InheritsMethodWithDefaultImplementationAmongOtherThings>(interceptor);
			var actual = proxy.Method();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Properties with default implementation

		[Test]
		public void Can_proxy_class_that_inherits_property_with_default_implementation_from_interface()
		{
			_ = generator.CreateClassProxy<InheritsPropertyWithDefaultImplementation>();
		}

		[Test]
		public void Can_proxy_interface_with_property_with_default_implementation()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHavePropertyWithDefaultImplementation>();
		}

		[Test]
		public void Default_implementation_gets_called_when_property_not_intercepted_in_proxied_class()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateClassProxy<InheritsPropertyWithDefaultImplementation>(options);
			var expected = "default implementation";
			var actual = ((IHavePropertyWithDefaultImplementation)proxy).PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Default_implementation_gets_called_when_property_not_intercepted_in_proxied_interface()
		{
			var options = new ProxyGenerationOptions(new ProxyNothingHook());
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHavePropertyWithDefaultImplementation>(options);
			var expected = "default implementation";
			var actual = proxy.PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_property_with_default_implementation_in_proxied_class()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateClassProxy<InheritsPropertyWithDefaultImplementation>(interceptor);
			var actual = ((IHavePropertyWithDefaultImplementation)proxy).PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_property_with_default_implementation_in_proxied_interface()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHavePropertyWithDefaultImplementation>(interceptor);
			var actual = proxy.PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_property_default_implementation_in_proxied_class()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateClassProxy<InheritsPropertyWithDefaultImplementation>(interceptor);
			var expected = "default implementation";
			var actual = ((IHavePropertyWithDefaultImplementation)proxy).PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_property_default_implementation_in_proxied_interface()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHavePropertyWithDefaultImplementation>(interceptor);
			var expected = "default implementation";
			var actual = proxy.PropertyWithDefaultImplementation;
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Methods with default implementation in additional interfaces

		[Test]
		public void Can_proxy_class_that_inherits_method_with_default_implementation_from_additional_interface()
		{
			_ = generator.CreateClassProxy(typeof(object), new[] { typeof(IHaveMethodWithDefaultImplementation) });
		}

		[Test]
		public void Can_proxy_interface_with_method_with_default_implementation_from_additional_interface()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), new[] { typeof(IHaveMethodWithDefaultImplementation) });
		}

		[Test]
		public void Can_intercept_method_with_default_implementation_from_additional_interface_in_proxied_class()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateClassProxy(typeof(object), new[] { typeof(IHaveMethodWithDefaultImplementation) },interceptor);
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_intercept_method_with_default_implementation_from_additional_interface_in_proxied_interface()
		{
			var expected = "intercepted";
			var interceptor = new WithCallbackInterceptor(invocation => invocation.ReturnValue = expected);
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), new[] { typeof(IHaveMethodWithDefaultImplementation) },interceptor);
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_method_default_implementation_from_additional_interface_in_proxied_class()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateClassProxy(typeof(object), new[] { typeof(IHaveMethodWithDefaultImplementation) }, interceptor);
			var expected = "default implementation";
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Can_proceed_to_method_default_implementation_from_additional_interface_in_proxied_interface()
		{
			var interceptor = new WithCallbackInterceptor(invocation => invocation.Proceed());
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), new[] { typeof(IHaveMethodWithDefaultImplementation) }, interceptor);
			var expected = "default implementation";
			var actual = ((IHaveMethodWithDefaultImplementation)proxy).MethodWithDefaultImplementation();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Sealed members

		[Test]
		public void Can_proxy_interface_with_sealed_method()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHaveSealedMethod>();
		}

		[Test]
		public void Can_invoke_sealed_method_in_proxied_interface()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHaveSealedMethod>();
			var expected = "default implementation";
			var actual = proxy.SealedMethod();
			Assert.AreEqual(expected, actual);
		}

		#endregion

		#region Static members

		[Test]
		public void Can_proxy_interface_with_static_method()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHaveStaticMethod>();
		}

#if NET7_0_OR_GREATER
		[Test]
		[Ignore("Support for static abstract interface members has not yet been implemented.")]
		public void Can_proxy_interface_with_static_abstract_method()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IHaveStaticAbstractMethod>();
		}
#endif

		#endregion

		public interface IHaveGenericMethodWithDefaultImplementation
		{
			string GenericMethodWithDefaultImplementation<T>()
			{
				return "default implementation for " + typeof(T).Name;
			}
		}

		public interface IHaveMethodWithDefaultImplementation
		{
			string MethodWithDefaultImplementation()
			{
				return "default implementation";
			}
		}

		public interface IHaveMethodWithDefaultImplementationAmongOtherThings : IHaveMethodWithDefaultImplementation
		{
			string Method();
		}

		public interface IHavePropertyWithDefaultImplementation
		{
			string PropertyWithDefaultImplementation
			{
				get
				{
					return "default implementation";
				}
			}
		}

		public interface IHaveSealedMethod
		{
			sealed string SealedMethod()
			{
				return "default implementation";
			}
		}

		public interface IHaveStaticMethod
		{
			static string StaticMethod()
			{
				return "default implementation";
			}
		}

#if NET7_0_OR_GREATER
		public interface IHaveStaticAbstractMethod
		{
			static abstract string StaticAbstractMethod();
		}
#endif

		public class InheritsMethodWithDefaultImplementation : IHaveMethodWithDefaultImplementation { }

		public class InheritsMethodWithDefaultImplementationAmongOtherThings : IHaveMethodWithDefaultImplementationAmongOtherThings
		{
			public virtual string Method()
			{
				return "class implementation";
			}
		}

		public class InheritsPropertyWithDefaultImplementation : IHavePropertyWithDefaultImplementation { }

		public class OverridesMethodWithDefaultImplementation : IHaveMethodWithDefaultImplementation
		{
			public virtual string MethodWithDefaultImplementation()
			{
				return "class implementation";
			}
		}
	}
}

#endif
