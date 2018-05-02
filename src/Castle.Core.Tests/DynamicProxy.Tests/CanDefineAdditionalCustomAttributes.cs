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
	using System.Reflection;

	using Castle.DynamicProxy.Internal;

	using NUnit.Framework;

	[TestFixture]
	public class CanDefineAdditionalCustomAttributes : BasePEVerifyTestCase
	{
		[Test]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_ints()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithIntArray));
		}

		[Test]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_types()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithTypeArray));
		}

		[Test]
		public void Can_clone_attributes_with_array_enums()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithEnumArray));
		}

		[Test]
		public void On_class()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateInfo<__Protect>());

			var proxy = generator.CreateClassProxy(typeof(CanDefineAdditionalCustomAttributes), options);

			Assert.IsTrue(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}

		[Test]
		public void On_interfaces()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateInfo<__Protect>());

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IDisposable), new Type[0], options);

			Assert.IsTrue(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class AttributeWithTypeArrayArgument : Attribute
	{
		public AttributeWithTypeArrayArgument(params Type[] attributeTypes)
		{
		}
	}

	public enum SomeByteEnumForAttributeWithEnumArrayArgument : byte
	{
		Default,
		Special
	}

	public enum SomeSbyteEnumForAttributeWithEnumArrayArgument : sbyte
	{
		Default,
		Special
	}

	public enum SomeShortEnumForAttributeWithEnumArrayArgument : short
	{
		Default,
		Special
	}

	public enum SomeUshortEnumForAttributeWithEnumArrayArgument : ushort
	{
		Default,
		Special
	}

	public enum SomeIntEnumForAttributeWithEnumArrayArgument : int
	{
		Default,
		Special
	}

	public enum SomeUintEnumForAttributeWithEnumArrayArgument : uint
	{
		Default,
		Special
	}

	public enum SomeLongEnumForAttributeWithEnumArrayArgument : long
	{
		Default,
		Special
	}

	public enum SomeUlongEnumForAttributeWithEnumArrayArgument : ulong
	{
		Default,
		Special
	}

	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class AttributeWithEnumArrayArgument : Attribute
	{
		public AttributeWithEnumArrayArgument(params SomeByteEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeSbyteEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeShortEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeUshortEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeIntEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeUintEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeLongEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
		public AttributeWithEnumArrayArgument(params SomeUlongEnumForAttributeWithEnumArrayArgument[] attributeEnums)
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

	[AttributeWithEnumArrayArgument(SomeByteEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeSbyteEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeShortEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeUshortEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeIntEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeUintEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeLongEnumForAttributeWithEnumArrayArgument.Special)]
	[AttributeWithEnumArrayArgument(SomeUlongEnumForAttributeWithEnumArrayArgument.Special)]
	public interface IHasAttributeWithEnumArray
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