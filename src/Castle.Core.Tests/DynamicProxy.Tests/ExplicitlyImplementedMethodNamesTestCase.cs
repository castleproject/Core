// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

	using NUnit.Framework;

	using ISharedNameFromA = Interfaces.A.ISharedName;
	using ISharedNameFromB = Interfaces.B.ISharedName;
	using ISharedNameFromC = Interfaces.C.ISharedName;

	[TestFixture]
	public class ExplicitlyImplementedMethodNamesTestCase
	{
		// This test does not target DynamicProxy. Rather, it codifies our assumption about
		// how the C# compiler names explicitly implemented methods. We need to verify it
		// because we want DynamicProxy to follow the same naming convention.
		[Test]
		public void Csharp_compiler_includes_namespace_and_type_name_in_names_of_explicitly_implemented_methods()
		{
			var b = typeof(ISharedNameFromB);
			var c = typeof(ISharedNameFromC);

			var implementingType = typeof(TripleSharedName);

			AssertNamingSchemeOfExplicitlyImplementedMethods(b, c, implementingType);
		}

		[Test]
		public void DynamicProxy_includes_namespace_and_type_name_in_names_of_explicitly_implemented_methods()
		{
			var a = typeof(ISharedNameFromA);
			var b = typeof(ISharedNameFromB);
			var c = typeof(ISharedNameFromC);

			var proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(
				interfaceToProxy: a,
				additionalInterfacesToProxy: new[] { b, c },
				interceptors: new StandardInterceptor());

			var implementingType = proxy.GetType();

			AssertNamingSchemeOfExplicitlyImplementedMethods(b, c, implementingType);
		}

		private void AssertNamingSchemeOfExplicitlyImplementedMethods(Type b, Type c, Type implementingType)
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			// The assertions at the end of this method only make sense if certain preconditions
			// are met. We verify those using NUnit assumptions:

			// We require two interface types that have the same name and a method named `M` each:
			Assume.That(b.GetTypeInfo().IsInterface);
			Assume.That(c.GetTypeInfo().IsInterface);
			Assume.That(b.Name == c.Name);
			Assume.That(b.GetMethod("M") != null);
			Assume.That(c.GetMethod("M") != null);

			// We also need a type that implements the above interfaces:
			Assume.That(b.IsAssignableFrom(implementingType));
			Assume.That(c.IsAssignableFrom(implementingType));

			// If all of the above conditions are met, we expect the methods from the interfaces
			// to be implemented explicitly. For our purposes, this means that they follow the
			// naming scheme `<namespace>.<type>.M`:
			Assert.NotNull(implementingType.GetMethod($"{b.Namespace}.{b.Name}.M", bindingFlags));
			Assert.NotNull(implementingType.GetMethod($"{c.Namespace}.{c.Name}.M", bindingFlags));
		}

		// Verification of the BCL's representation of "no namespace" supports our implementation
		// of the above naming scheme:
		[Test]
		public void Namespace_of_types_without_namespace_equals_null()
		{
			Assert.Null(typeof(global::IHaveNoNamespace).Namespace);
		}

		private sealed class TripleSharedName : ISharedNameFromA, ISharedNameFromB, ISharedNameFromC
		{
			void ISharedNameFromA.M() { }
			void ISharedNameFromB.M() { }
			void ISharedNameFromC.M() { }
		}
	}
}

internal interface IHaveNoNamespace { }
