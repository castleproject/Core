// Copyright 2017-2017 Castle Project - http://www.castleproject.org/ 
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

	using Castle.DynamicProxy;

	using NUnit.Framework;

	[TestFixture]
	public class ProxyUtilTestCase
	{
		[Test]
		public void CreateDelegateToMixin_when_given_null_for_proxy_throws_ArgumentNullException()
		{
			var _ = typeof(Action);
			Assert.Throws<ArgumentNullException>(() => ProxyUtil.CreateDelegateToMixin(null, _));
		}

		[Test]
		public void CreateDelegateToMixin_when_given_null_for_delegateType_throws_ArgumentNullException()
		{
			var _ = new object();
			Assert.Throws<ArgumentNullException>(() => ProxyUtil.CreateDelegateToMixin(_, null));
		}

		[Test]
		public void CreateDelegateToMixin_when_given_non_delegate_type_throws_ArgumentException()
		{
			var _ = new object();
			Assert.Throws<ArgumentException>(() => ProxyUtil.CreateDelegateToMixin(_, typeof(Exception)));
		}

		[Test]
		public void CreateDelegateToMixin_when_given_valid_arguments_succeeds()
		{
			var proxy = new FakeProxyWithInvokeMethods();
			Assert.NotNull(ProxyUtil.CreateDelegateToMixin(proxy, typeof(Action)));
		}

		[Test]
		public void CreateDelegateToMixin_throws_MissingMethodException_if_no_suitable_Invoke_method_found()
		{
			var proxy = new FakeProxyWithInvokeMethods();
			Assert.Throws<MissingMethodException>(() => ProxyUtil.CreateDelegateToMixin<Action<bool>>(proxy));
		}

		[Test]
		public void CreateDelegateToMixin_returns_invokable_delegate()
		{
			var proxy = new FakeProxyWithInvokeMethods();
			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			action.Invoke();
		}

		[Test]
		public void CreateDelegateToMixin_can_deal_with_multiple_Invoke_overloads()
		{
			var proxy = new FakeProxyWithInvokeMethods();

			var action = ProxyUtil.CreateDelegateToMixin<Action>(proxy);
			action.Invoke();
			Assert.AreEqual("Invoke()", proxy.LastInvocation);

			var intAction = ProxyUtil.CreateDelegateToMixin<Action<int>>(proxy);
			intAction.Invoke(42);
			Assert.AreEqual("Invoke(42)", proxy.LastInvocation);
		}

		[TestCaseSource(nameof(AccessibleMethods))]
		public void IsAccessible_Accessible_Method_Returns_True(MethodBase method)
		{
			Assert.IsTrue(ProxyUtil.IsAccessible(method));
		}

		[TestCaseSource(nameof(InaccessibleMethods))]
		public void IsAccessible_Inaccessible_Method_Returns_False(MethodBase method)
		{
			Assert.IsFalse(ProxyUtil.IsAccessible(method));
		}

		[TestCaseSource(nameof(AccessibleMethods))]
		public void IsAccessibleWithReason_Accessible_Method_Returns_True(MethodBase method)
		{
			string reason;
			Assert.IsTrue(ProxyUtil.IsAccessible(method, out reason));
		}

		[TestCaseSource(nameof(AccessibleMethods))]
		public void IsAccessibleWithReason_Accessible_Method_Does_Not_Populate_ReasonMethodIsNotAccessible(MethodBase method)
		{
			string reason;
			ProxyUtil.IsAccessible(method, out reason);

			Assert.IsNull(reason);
		}

		[TestCaseSource(nameof(InaccessibleMethods))]
		public void IsAccessibleWithReason_Inaccessible_Method_Returns_False(MethodBase method)
		{
			string reason;
			Assert.IsFalse(ProxyUtil.IsAccessible(method, out reason));
		}

		[TestCaseSource(nameof(InaccessibleMethods))]
		public void IsAccessibleWithReason_Inaccessible_Method_Populates_ReasonMethodIsNotAccessible(MethodBase method)
		{
			string reason;
			ProxyUtil.IsAccessible(method, out reason);

#if FEATURE_GET_REFERENCED_ASSEMBLIES
			var expectedReason = "Can not create proxy for method Void " + method.Name + "() because it or its declaring type is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)] attribute, because assembly Castle.Core.Tests is strong-named.";
#else
			var expectedReason = "Can not create proxy for method Void " + method.Name + "() because it or its declaring type is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7\")] attribute, because assembly Castle.Core.Tests is strong-named.";
#endif

			Assert.AreEqual(expectedReason, reason);
		}

		private static MethodInfo GetMethod(Type declaringType, string name)
		{
			return declaringType.GetMethod(name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static readonly MethodBase[] AccessibleMethods =
		{
			GetMethod(typeof(PublicTestClass), "APublicMethod"),
			GetMethod(typeof(PublicTestClass), "AProtectedMethod"),
			GetMethod(typeof(PublicTestClass), "AnInternalMethod"), // because our internals are visible to DynamicProxy2
			GetMethod(typeof(PublicTestClass), "AProtectedInternalMethod"),
		};

		public static readonly MethodBase[] InaccessibleMethods =
		{
			GetMethod(typeof(PublicTestClass), "APrivateMethod"),
			GetMethod(typeof(PrivateTestClass), "APublicMethod"),
		};

		public class PublicTestClass
		{
			public void APublicMethod()
			{
			}

			protected void AProtectedMethod()
			{
			}

			internal void AnInternalMethod()
			{
			}

			protected internal void AProtectedInternalMethod()
			{
			}

			private void APrivateMethod()
			{
			}
		}

		private class PrivateTestClass
		{
			public void APublicMethod()
			{
			}
		}

		private sealed class FakeProxyWithInvokeMethods
		{
			public string LastInvocation { get; set; }

			public void Invoke()
			{
				LastInvocation = "Invoke()";
			}

			public void Invoke(int arg)
			{
				LastInvocation = $"Invoke({arg})";
			}
		}
	}
}
