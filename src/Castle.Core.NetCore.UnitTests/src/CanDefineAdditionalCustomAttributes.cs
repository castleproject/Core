// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Internal;

	using CastleTests;

	using Xunit;

	using System.Reflection;

	public class CanDefineAdditionalCustomAttributes : BasePEVerifyTestCase
	{
		[Fact]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_ints()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithIntArray));
		}

		[Fact]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_types()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithTypeArray));
		}

		[Fact]
		public void On_class()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateBuilder<__Protect>());

			var proxy = generator.CreateClassProxy(typeof(CanDefineAdditionalCustomAttributes), options);

			Assert.True(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}

		[Fact]
		public void On_interfaces()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateBuilder<__Protect>());

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IDisposable), new Type[0], options);

			Assert.True(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class AttributeWithTypeArrayArgument : Attribute
	{
		public AttributeWithTypeArrayArgument(params Type[] attributeTypes)
		{
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class AttributeWithIntArrayArgument : Attribute
	{
		public AttributeWithIntArrayArgument(params int[] ints)
		{
		}
	}

	[AttributeWithTypeArrayArgument(typeof(string))]
	public interface IHasAttributeWithTypeArray
	{
	}

	[AttributeWithIntArrayArgument(1, 2, 3)]
	public interface IHasAttributeWithIntArray
	{
	}

	public class __Protect : Attribute
	{
	}
}