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
	using System;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
#if FEATURE_SECURITY_PERMISSIONS
	using System.Security.Permissions;
#endif

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class AttributesToAvoidReplicatingTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void After_adding_attribute_must_be_listed_as_contained()
		{
			AttributesToAvoidReplicating.Add<string>();
			bool contains = AttributesToAvoidReplicating.Contains(typeof(string));
			Assert.IsTrue(contains);
		}

		[Test]
		public void After_adding_attribute_must_still_contain_original_attributes()
		{
			AttributesToAvoidReplicating.Add<string>();
			bool contains = AttributesToAvoidReplicating.Contains(typeof(System.Runtime.InteropServices.ComImportAttribute));
			Assert.IsTrue(contains);
		}

		[Test]
		public void NonInheritableAttribute_should_be_replicated_as_it_is_not_inherited()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_NonInheritable>();
			Assert.AreEqual(1, AttributeCount<NonInheritableAttribute>(proxy));
		}

		[NonInheritable]
		public class AttributedClass_NonInheritable
		{
		}

		[Test]
		public void InheritableAttribute_should_not_be_replicated_as_it_is_inherited_by_the_runtime()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_Inheritable>();
			Assert.AreEqual(0, AttributeCount<InheritableAttribute>(proxy));
		}

		[Inheritable]
		public class AttributedClass_Inheritable
		{
		}

#if FEATURE_SECURITY_PERMISSIONS
		[Test]
		public void SecurityPermissionAttribute_should_not_be_replicated_as_it_is_part_of_cas()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_SecurityPermission>();
			Assert.AreEqual(0, AttributeCount<SecurityPermissionAttribute>(proxy));
		}

		[SecurityPermission(SecurityAction.Demand)]
		public class AttributedClass_SecurityPermission
		{
		}

		[Test]
		public void ReflectionPermissionAttribute_should_not_be_replicated_as_it_is_part_of_cas()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_ReflectionPermission>();
			Assert.AreEqual(0, AttributeCount<ReflectionPermissionAttribute>(proxy));
		}

		[ReflectionPermission(SecurityAction.Demand)]
		public class AttributedClass_ReflectionPermission
		{
		}
#endif

		private int AttributeCount<TAttribute>(object proxy)
			where TAttribute : Attribute
		{
			return proxy.GetType().GetTypeInfo().GetCustomAttributes(typeof(TAttribute), false).Count();
		}
	}
}