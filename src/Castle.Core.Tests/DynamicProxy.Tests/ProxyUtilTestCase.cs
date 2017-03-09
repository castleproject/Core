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
	using System.Reflection;

	using Castle.DynamicProxy;

	using NUnit.Framework;

	[TestFixture]
	public class ProxyUtilTestCase
	{
		[TestCaseSource("AccessibleMethods")]
		public void IsAccessible_Accessible_Method_Returns_True(string methodName)
		{
			Assert.IsTrue(ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName)));
		}

		[TestCaseSource("InaccessibleMethods")]
		public void IsAccessible_Inaccessible_Method_ReturnsFalse(string methodName)
		{
			Assert.IsFalse(ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName)));
		}

		[TestCaseSource("AccessibleMethods")]
		public void IsAccessibleWithReason_Accessible_Method_Returns_True(string methodName)
		{
			string reason;
			Assert.IsTrue(ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName), out reason));
		}

		[TestCaseSource("AccessibleMethods")]
		public void IsAccessibleWithReason_Accessible_Method_Does_Not_Populate_ReasonMethodIsNotAccessible(string methodName)
		{
			string reason;
			ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName), out reason);

			Assert.IsNull(reason);
		}

		[TestCaseSource("InaccessibleMethods")]
		public void IsAccessibleWithReason_Inaccessible_Method_ReturnsFalse(string methodName)
		{
			string reason;
			Assert.IsFalse(ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName), out reason));
		}

		[TestCaseSource("InaccessibleMethods")]
		public void IsAccessibleWithReason_Inaccessible_Method_Populates_ReasonMethodIsNotAccessible(string methodName)
		{
			string reason;
			ProxyUtil.IsAccessible(GetMethod<TestClass>(methodName), out reason);

			var expectedReason =
				"Can not create proxy for method Void APrivateMethod() " +
				"because it is not accessible. Make it public, or internal " +
				"and mark your assembly with " +
				"[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)] " +
				"attribute, because assembly Castle.Core.Tests is strong-named.";
			Assert.AreEqual(expectedReason, reason);
		}

		private static MethodInfo GetMethod<T>(string name)
		{
			return typeof(T).GetMethod(name,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		private static readonly object[] AccessibleMethods =
		{
			new object[] { "APublicMethod" },
			new object[] { "AProtectedMethod" },
			new object[] { "AnInternalMethod" }, // because our internals are visible to DynamicProxy2 
			new object[] { "AProtectedInternalMethod" }
		};

		private static readonly object[] InaccessibleMethods =
		{
			new object[] { "APrivateMethod" },
		};

		public class TestClass
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
	}
}