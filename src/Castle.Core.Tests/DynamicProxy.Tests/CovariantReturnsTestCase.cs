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

#if NET5_0_OR_GREATER

using System;
using System.Reflection;

using NUnit.Framework;

namespace Castle.DynamicProxy.Tests
{
	[TestFixture]
	public class CovariantReturnsTestCase : BasePEVerifyTestCase
	{
		// DynamicProxy's current implementation for covariant returns support expects to see override methods
		// before the overridden methods. That is, we rely on a specific behavior of .NET Reflection, and this test
		// codifies that assumption. If it ever breaks, we'll need to adjust our implementation accordingly.
		[Test]
		public void Reflection_returns_methods_from_a_derived_class_before_methods_from_its_base_class()
		{
			var derivedType = typeof(DerivedClassWithStringInsteadOfObject);
			var baseType = typeof(BaseClassWithObject);
			Assume.That(derivedType.BaseType == baseType);

			var derivedMethod = derivedType.GetMethod("Method");
			var baseMethod = baseType.GetMethod("Method");
			Assume.That(derivedMethod != baseMethod);

			var methods = derivedType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			var derivedMethodIndex = Array.FindIndex(methods, m => m.Name == "Method" && m.DeclaringType == derivedType);
			var baseMethodIndex = Array.FindIndex(methods, m => m.Name == "Method" && m.DeclaringType == baseType);
			Assume.That(derivedMethodIndex >= 0);
			Assume.That(baseMethodIndex >= 0);

			Assert.IsTrue(derivedMethodIndex < baseMethodIndex);
		}

		[Theory]
		[TestCase(typeof(DerivedClassWithInterfaceInsteadOfObject))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfObject))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfInterface))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfGenericArg))]
		public void Can_proxy_type_having_method_with_covariant_return(Type classToProxy)
		{
			_ = generator.CreateClassProxy(classToProxy, new StandardInterceptor());
		}

		[Theory]
		[TestCase(typeof(DerivedClassWithInterfaceInsteadOfObject), typeof(IComparable))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfObject), typeof(string))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfInterface), typeof(string))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfGenericArg), typeof(string))]
		public void Proxied_method_has_correct_return_type(Type classToProxy, Type expectedMethodReturnType)
		{
			var proxy = generator.CreateClassProxy(classToProxy, new StandardInterceptor());
			var method = proxy.GetType().GetMethod("Method");
			Assert.AreEqual(expectedMethodReturnType, method.ReturnType);
		}

		[Theory]
		[TestCase(typeof(DerivedClassWithInterfaceInsteadOfObject))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfObject))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfInterface))]
		[TestCase(typeof(DerivedClassWithStringInsteadOfGenericArg))]
		public void Can_invoke_method_with_covariant_return(Type classToProxy)
		{
			var proxy = generator.CreateClassProxy(classToProxy, new StandardInterceptor());
			var method = proxy.GetType().GetMethod("Method");
			var returnValue = method.Invoke(proxy, null);
			Assert.AreEqual(expected: classToProxy.Name, returnValue);
		}

		public class BaseClassWithObject
		{
			public virtual object Method() => nameof(BaseClassWithObject);
		}

		public class DerivedClassWithInterfaceInsteadOfObject : BaseClassWithObject
		{
			public override IComparable Method() => nameof(DerivedClassWithInterfaceInsteadOfObject);
		}

		public class DerivedClassWithStringInsteadOfObject : BaseClassWithObject
		{
			public override string Method() => nameof(DerivedClassWithStringInsteadOfObject);
		}

		public class BaseClassWithInterface
		{
			public virtual IComparable Method() => nameof(BaseClassWithInterface);
		}

		public class DerivedClassWithStringInsteadOfInterface : BaseClassWithInterface
		{
			public override string Method() => nameof(DerivedClassWithStringInsteadOfInterface);
		}

		public class BaseClassWithGenericArg<T> where T : IComparable
		{
			public virtual T Method() => default(T);
		}

		public class DerivedClassWithStringInsteadOfGenericArg : BaseClassWithGenericArg<IComparable>
		{
			public override string Method() => nameof(DerivedClassWithStringInsteadOfGenericArg);
		}
	}
}

#endif
