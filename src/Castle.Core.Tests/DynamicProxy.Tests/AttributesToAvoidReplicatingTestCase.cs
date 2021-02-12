﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Security.Permissions;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class AttributesToAvoidReplicatingTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void NonInheritableAttribute_should_be_replicated_as_it_is_not_inherited()
		{
			var proxy = generator.CreateClassProxy<AttributedClass_NonInheritable>();
			Assert.AreEqual(1, AttributeCount<NonInheritableAttribute>(proxy));
		}

		[Test]
		public void NonInheritableAttribute_can_be_suppressed_via_AttributesToAvoidReplicating()
		{
			var options = new ProxyGenerationOptions();
			options.AttributesToAvoidReplicating.Add(typeof(NonInheritableAttribute));
			var proxy = generator.CreateClassProxy<AttributedClass_NonInheritable>(options);
			Assert.AreEqual(0, AttributeCount<NonInheritableAttribute>(proxy));
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

		private int AttributeCount<TAttribute>(object proxy)
			where TAttribute : Attribute
		{
			return proxy.GetType().GetCustomAttributes(typeof(TAttribute), false).Count();
		}
	}
}