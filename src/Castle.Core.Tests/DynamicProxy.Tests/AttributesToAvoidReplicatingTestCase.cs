// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
#if FEATURE_SECURITY_PERMISSIONS
	using System.Security.Permissions;
#endif

	using NUnit.Framework;

	[TestFixture]
	public class AttributesToAvoidReplicatingTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void DisplayNameAttribute_should_be_replicated_as_it_is_not_inherited()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_DisplayName>();
			Assert.AreEqual(1, proxy.GetType().GetTypeInfo().GetCustomAttributes<DisplayNameAttribute>().Count());
		}

		[DisplayName("Test")]
		public class AttributedClass_DisplayName
		{
		}

#if FEATURE_SECURITY_PERMISSIONS
		[Test]
		public void SecurityPermissionAttribute_should_not_be_replicated_as_it_is_part_of_cas()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_SecurityPermission>();
			Assert.IsEmpty(proxy.GetType().GetTypeInfo().GetCustomAttributes<SecurityPermissionAttribute>());
		}

		[SecurityPermission(SecurityAction.Demand)]
		public class AttributedClass_SecurityPermission
		{
		}

		[Test]
		public void ReflectionPermissionAttribute_should_not_be_replicated_as_it_is_part_of_cas()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_ReflectionPermission>();
			Assert.IsEmpty(proxy.GetType().GetTypeInfo().GetCustomAttributes<ReflectionPermissionAttribute>());
		}

		[ReflectionPermission(SecurityAction.Demand)]
		public class AttributedClass_ReflectionPermission
		{
		}
#endif
	}
}