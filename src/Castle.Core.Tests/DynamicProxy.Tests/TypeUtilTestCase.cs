// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;

	using Castle.DynamicProxy.Internal;

	using NUnit.Framework;

	[TestFixture]
	public class TypeUtilTestCase
	{
		[TestCase(typeof(object), "System.Object")]
		[TestCase(typeof(List<>), "System.Collections.Generic.List`1")]
		[TestCase(typeof(List<object>), "System.Collections.Generic.List`1[System.Object]")]
		[TestCase(typeof(List<object>.Enumerator), "System.Collections.Generic.List`1+Enumerator[System.Object]")]
		[TestCase(typeof(Dictionary<,>), "System.Collections.Generic.Dictionary`2")]
		[TestCase(typeof(Dictionary<object, bool>), "System.Collections.Generic.Dictionary`2[System.Object,System.Boolean]")]
		public void AppendNamespaceQualifiedNameOf(Type type, string expected)
		{
			var builder = new StringBuilder();

			builder.AppendNamespaceQualifiedNameOf(type);

			var actual = builder.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetAllInstanceMethods_GetsPublicAndNonPublicMethods()
		{
			MethodInfo[] methods =
				typeof(object).GetAllInstanceMethods();
			MethodInfo[] realMethods =
				typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			CollectionAssert.AreEquivalent(realMethods, methods);
		}

		// The test above suggests that `TypeUtil.GetAllInstanceMethods` and `Type.GetMethods`
		// can be used interchangeably, but this is not always the case. See the test(s) below and
		// `GenericInterfaceProxyTestCase.GetAllInstanceMethodsIsStable` for cases where the two methods
		// may produce different results.

#if NET5_0_OR_GREATER

		[Test]
		public void GetAllInstanceMethods_NoDuplicatesForMethodWithCovariantReturnType()
		{
			MethodInfo[] methods =
				typeof(Derived).GetAllInstanceMethods();
			MethodInfo[] realMethods =
				typeof(Derived).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			CollectionAssert.AreNotEquivalent(realMethods, methods);
			CollectionAssert.IsSubsetOf(subset: methods, superset: realMethods);
			Assert.AreEqual(2, realMethods.Where(m => m.Name == nameof(Derived.Method)).Count());
			Assert.AreEqual(1, methods.Where(m => m.Name == nameof(Derived.Method)).Count());
		}

		public abstract class Base
		{
			public abstract Base Method();
		}

		public abstract class Derived : Base
		{
			public override abstract Derived Method();
		}

#endif
	}
}
