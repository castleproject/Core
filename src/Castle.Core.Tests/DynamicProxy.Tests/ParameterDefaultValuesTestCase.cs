// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class ParameterDefaultValuesTestCase : BasePEVerifyTestCase
	{
		[TestCase(typeof(ClassWithMethodsWithAllKindsOfOptionalParameters))]
		[TestCase(typeof(HasDefaultValues))]
		public void Proxying_class_with_all_kinds_of_default_parameter_values_succeeds(Type classType)
		{
			generator.CreateClassProxy(classType, new DoNothingInterceptor());
		}

		[TestCase(typeof(InterfaceWithMethodsWithAllKindsOfOptionalParameters))]
		public void Proxying_interface_with_all_kinds_of_default_parameter_values_succeeds(Type interfaceType)
		{
			generator.CreateInterfaceProxyWithoutTarget(interfaceType, new DoNothingInterceptor());
		}

		[Test]
		public void Fully_supported_No_default()
		{
			var originalParameter = GetOriginalParameter(typeof(HasNoDefaultValues), nameof(HasNoDefaultValues.No_default));
			Assert.True(originalParameter.IsOptional);
			Assert.False(originalParameter.HasDefaultValue);
			Assert.IsAssignableFrom<Missing>(originalParameter.DefaultValue);

			var proxiedParameter = GetProxiedParameter(typeof(HasNoDefaultValues), nameof(HasNoDefaultValues.No_default));
			Assert.True(proxiedParameter.IsOptional);
			Assert.False(proxiedParameter.HasDefaultValue);
			Assert.IsAssignableFrom<Missing>(proxiedParameter.DefaultValue);
		}

		[Test]
		public void Fully_supported_Not_optional()
		{
			// On .NET Core 1.1, we have two different types called `DBNull`: one from the NuGet package
			// System.Data.Common, and another (shadowed / non-exposed) from mscorlib. We need the latter.
			// The `Type.GetType` detour can be dropped after upgrading the test project to .NET Core 2.0.
			var coreLibDBNullType = Type.GetType("System.DBNull");

			var originalParameter = GetOriginalParameter(typeof(HasNoDefaultValues), nameof(HasNoDefaultValues.Not_optional));
			Assert.False(originalParameter.IsOptional);
			Assert.False(originalParameter.HasDefaultValue);
			Assert.IsAssignableFrom(coreLibDBNullType, originalParameter.DefaultValue);

			var proxiedParameter = GetProxiedParameter(typeof(HasNoDefaultValues), nameof(HasNoDefaultValues.Not_optional));
			Assert.False(proxiedParameter.IsOptional);
			Assert.False(proxiedParameter.HasDefaultValue);
			Assert.IsAssignableFrom(coreLibDBNullType, proxiedParameter.DefaultValue);
		}

		[TestCase(nameof(HasDefaultValues.Bool_default))]
		[TestCase(nameof(HasDefaultValues.Bool_non_default))]
		[TestCase(nameof(HasDefaultValues.Bool_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Bool_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Bool_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Byte_default))]
		[TestCase(nameof(HasDefaultValues.Byte_non_default))]
		[TestCase(nameof(HasDefaultValues.Byte_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Byte_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Byte_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Char_default))]
		[TestCase(nameof(HasDefaultValues.Char_non_default))]
		[TestCase(nameof(HasDefaultValues.Char_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Char_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Char_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.DateTime_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.DateTime_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.DateTime_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Decimal_default))]
		[TestCase(nameof(HasDefaultValues.Decimal_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Decimal_non_default))]
		[TestCase(nameof(HasDefaultValues.Decimal_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Decimal_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Double_default))]
		[TestCase(nameof(HasDefaultValues.Double_non_default))]
		[TestCase(nameof(HasDefaultValues.Double_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Double_non_default_from_attribute_wrong_type))]
		[TestCase(nameof(HasDefaultValues.Double_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Double_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Double_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Double_nullable_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Double_nullable_non_default_from_attribute_wrong_type))]
		[TestCase(nameof(HasDefaultValues.Float_default))]
		[TestCase(nameof(HasDefaultValues.Float_non_default))]
		[TestCase(nameof(HasDefaultValues.Float_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Float_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Float_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Int_default))]
		[TestCase(nameof(HasDefaultValues.Int_non_default))]
		[TestCase(nameof(HasDefaultValues.Int_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Int_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Int_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Long_default))]
		[TestCase(nameof(HasDefaultValues.Long_non_default))]
		[TestCase(nameof(HasDefaultValues.Long_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Long_non_default_from_attribute_wrong_type))]
		[TestCase(nameof(HasDefaultValues.Long_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Long_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Long_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Long_nullable_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Long_nullable_non_default_from_attribute_wrong_type))]
		[TestCase(nameof(HasDefaultValues.Object_default))]
		[TestCase(nameof(HasDefaultValues.Object_null))]
		[TestCase(nameof(HasDefaultValues.Object_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.SByte_default))]
		[TestCase(nameof(HasDefaultValues.SByte_non_default))]
		[TestCase(nameof(HasDefaultValues.SByte_nullable_null))]
		[TestCase(nameof(HasDefaultValues.SByte_nullable_default))]
		[TestCase(nameof(HasDefaultValues.SByte_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Short_default))]
		[TestCase(nameof(HasDefaultValues.Short_non_default))]
		[TestCase(nameof(HasDefaultValues.Short_nullable_null))]
		[TestCase(nameof(HasDefaultValues.Short_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Short_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.String_default))]
		[TestCase(nameof(HasDefaultValues.String_non_default))]
		[TestCase(nameof(HasDefaultValues.String_null))]
		[TestCase(nameof(HasDefaultValues.UInt_default))]
		[TestCase(nameof(HasDefaultValues.UInt_non_default))]
		[TestCase(nameof(HasDefaultValues.UInt_nullable_null))]
		[TestCase(nameof(HasDefaultValues.UInt_nullable_default))]
		[TestCase(nameof(HasDefaultValues.UInt_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.ULong_default))]
		[TestCase(nameof(HasDefaultValues.ULong_non_default))]
		[TestCase(nameof(HasDefaultValues.ULong_nullable_null))]
		[TestCase(nameof(HasDefaultValues.ULong_nullable_default))]
		[TestCase(nameof(HasDefaultValues.ULong_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedClass_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedClass_null))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_non_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_nullable_null))]
		[TestCase(nameof(HasDefaultValues.UserDefinedStruct_nullable_null))]
		[TestCase(nameof(HasDefaultValues.UShort_default))]
		[TestCase(nameof(HasDefaultValues.UShort_non_default))]
		[TestCase(nameof(HasDefaultValues.UShort_nullable_null))]
		[TestCase(nameof(HasDefaultValues.UShort_nullable_default))]
		[TestCase(nameof(HasDefaultValues.UShort_nullable_non_default))]
		public void Fully_supported(string methodName)
		{
			AssertParameter(typeof(HasDefaultValues), methodName);
		}

		[TestCase(typeof(bool?))]
		[TestCase(typeof(decimal?))]
		[TestCase(typeof(double?))]
		[TestCase(typeof(float?))]
		[TestCase(typeof(DateTime?))]
		[TestCase(typeof(DateTime?))]
		[TestCase(typeof(int?))]
		[TestCase(typeof(object))]
		[TestCase(typeof(string))]
		[TestCase(typeof(UserDefinedClass))]
		[TestCase(typeof(UserDefinedEnum?))]
		[TestCase(typeof(UserDefinedStruct?))]
		public void Fully_supported_Generics(Type parameterType)
		{
			AssertParameter(typeof(HasDefaultValue<>).MakeGenericType(parameterType), nameof(HasDefaultValue<object>.Method));
		}

		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "ParameterBuilder.SetConstant does not accept a null default value for value type parameters. See https://github.com/dotnet/corefx/issues/26164.")]
		[TestCase(typeof(bool))]
		[TestCase(typeof(decimal))]
		[TestCase(typeof(double))]
		[TestCase(typeof(float))]
		[TestCase(typeof(DateTime))]
		[TestCase(typeof(int))]
		[TestCase(typeof(UserDefinedEnum))]
		[TestCase(typeof(UserDefinedStruct))]
		public void Not_supported_on_the_CLR_Generics(Type parameterType)
		{
			AssertParameter(typeof(HasDefaultValue<>).MakeGenericType(parameterType), nameof(HasDefaultValue<object>.Method));
		}

		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "ParameterBuilder.SetConstant does not accept a null default value for value type parameters. See https://github.com/dotnet/corefx/issues/26164.")]
		[TestCase(nameof(HasDefaultValues.DateTime_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedStruct_default))]
		public void Not_supported_on_the_CLR_Struct_default(string methodName)
		{
			AssertParameter(typeof(HasDefaultValues), methodName);
		}

		[ExcludeOnFramework(Framework.NetCore | Framework.NetFramework, "ParameterBuilder.SetConstant does not accept non-null default values for nullable enum parameters. See https://github.com/dotnet/coreclr/issues/17893.")]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_nullable_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_nullable_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.UserDefinedEnum_nullable_non_default_from_attribute))]
		public void Not_supported_on_the_CLR_UserDefinedEnum_nullable_non_null(string methodName)
		{
			AssertParameter(typeof(HasDefaultValues), methodName);
		}

		[ExcludeOnFramework(Framework.Mono, "ParameterInfo.DefaultValue does not report the correct default value for non-null optional parameters of type `DateTime?` and `decimal?`. See https://github.com/mono/mono/issues/11303.")]
		[TestCase(nameof(HasDefaultValues.DateTime_nullable_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.DateTime_nullable_non_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Decimal_nullable_default))]
		[TestCase(nameof(HasDefaultValues.Decimal_nullable_default_from_attribute))]
		[TestCase(nameof(HasDefaultValues.Decimal_nullable_non_default))]
		[TestCase(nameof(HasDefaultValues.Decimal_nullable_non_default_from_attribute))]
		public void Not_supported_on_Mono_DateTime_and_Decimal_not_null(string methodName)
		{
			AssertParameter(typeof(HasDefaultValues), methodName);
		}

		private void AssertParameter(Type type, string methodName)
		{
			var method = type.GetMethod(methodName);
			var expectedDefaultValue = method.Invoke(Activator.CreateInstance(type), new object[1]);

			var proxiedParameter = GetProxiedParameter(type, methodName);
#if DOTNET45
			Assert.True(proxiedParameter.HasDefaultValue);
#endif
			Assert.AreEqual(expectedDefaultValue, proxiedParameter.DefaultValue);
		}

		private static ParameterInfo GetOriginalParameter(Type type, string methodName = "Method")
		{
			return type.GetMethod(methodName).GetParameters()[0];
		}

		private ParameterInfo GetProxiedParameter(Type type, string methodName = "Method")
		{
			var proxy = generator.CreateClassProxy(type, new DoNothingInterceptor());
			return proxy.GetType().GetMethod(methodName).GetParameters()[0];
		}

		public class HasDefaultValues
		{
			// This class has methods with optional parameters having all kinds of default values.
			//
			// Method name nomenclature used: `{parameter type}[_nullable]_{kind of default value}`
			//
			// The methods must return the default value that reflection is expected to report for the
			// parameter. Typically this matches the declared default value, but there are exceptions!
			//
			// This method list is intended to be exhaustive, i.e. it should cover *all* possible
			// kinds of default values. If you find a case that is missing, please add it.
			//
			// (Some lines are commented out. These are not valid C#. We've left them here to show
			// that the cases have at least been considered.)

			public virtual object Bool_default(bool arg = default) => default(bool);
			public virtual object Bool_non_default(bool arg = true) => true;
			public virtual object Bool_nullable_null(bool? arg = null) => null;
			public virtual object Bool_nullable_default(bool? arg = default(bool)) => default(bool);
			public virtual object Bool_nullable_non_default(bool? arg = true) => true;

			public virtual object Byte_default(byte arg = default) => default(byte);
			public virtual object Byte_non_default(byte arg = (byte)1) => (byte)1;
			public virtual object Byte_nullable_null(byte? arg = null) => null;
			public virtual object Byte_nullable_default(byte? arg = default(byte)) => default(byte);
			public virtual object Byte_nullable_non_default(byte? arg = (byte)1) => (byte)1;

			public virtual object Char_default(char arg = default) => default(char);
			public virtual object Char_non_default(char arg = '1') => '1';
			public virtual object Char_nullable_null(char? arg = null) => null;
			public virtual object Char_nullable_default(char? arg = default(char)) => default(char);
			public virtual object Char_nullable_non_default(char? arg = '1') => '1';

			public virtual object DateTime_default(DateTime arg = default) => null;
			public virtual object DateTime_default_from_attribute([Optional, DateTimeConstant(0L)] DateTime arg) => new DateTime(0L);
			public virtual object DateTime_non_default_from_attribute([Optional, DateTimeConstant(1L)] DateTime arg) => new DateTime(1L);
			public virtual object DateTime_nullable_null(DateTime? arg = null) => null;
			//public virtual object DateTime_nullable_default(DateTime? arg = default(DateTime)) => default(DateTime);
			public virtual object DateTime_nullable_default_from_attribute([Optional, DateTimeConstant(0L)] DateTime? arg) => new DateTime(0L);
			public virtual object DateTime_nullable_non_default_from_attribute([Optional, DateTimeConstant(1L)] DateTime? arg) => new DateTime(1L);

			public virtual object Decimal_default(decimal arg = default) => default(decimal);
			public virtual object Decimal_default_from_attribute([Optional, DecimalConstant(0, 0, 0, 0, 0)] decimal arg) => new decimal(0, 0, 0, false, 0);
			public virtual object Decimal_non_default(decimal arg = 1.00m) => 1.00m;
			public virtual object Decimal_non_default_from_attribute([Optional, DecimalConstant(0, 0, 0, 0, 1)] decimal arg) => new decimal(1, 0, 0, false, 0);
			public virtual object Decimal_nullable_null(decimal? arg = null) => null;
			public virtual object Decimal_nullable_default(decimal? arg = default(decimal)) => default(decimal);
			public virtual object Decimal_nullable_default_from_attribute([Optional, DecimalConstant(0, 0, 0, 0, 0)] decimal? arg) => new decimal(0, 0, 0, false, 0);
			public virtual object Decimal_nullable_non_default(decimal? arg = 1.00m) => 1.00m;
			public virtual object Decimal_nullable_non_default_from_attribute([Optional, DecimalConstant(0, 0, 0, 0, 1)] decimal? arg) => new decimal(1, 0, 0, false, 0);

			public virtual object Double_default(double arg = default) => default(double);
			public virtual object Double_non_default(double arg = 1.0) => 1.0;
			public virtual object Double_non_default_from_attribute([Optional, DefaultParameterValue(1.0)] double arg) => 1.0;
			public virtual object Double_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1.0f)] double arg) => 1.0;
			public virtual object Double_nullable_null(double? arg = null) => null;
			public virtual object Double_nullable_default(double? arg = default(double)) => default(double);
			public virtual object Double_nullable_non_default(double? arg = 1.0) => 1.0;
			public virtual object Double_nullable_non_default_from_attribute([Optional, DefaultParameterValue(1.0)] double? arg) => 1.0;
			public virtual object Double_nullable_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1.0f)] double? arg) => 1.0;

			public virtual object Float_default(float arg = default) => default(float);
			public virtual object Float_non_default(float arg = 1.0f) => 1.0f;
			public virtual object Float_nullable_null(float? arg = null) => null;
			public virtual object Float_nullable_default(float? arg = default(float)) => default(float);
			public virtual object Float_nullable_non_default(float? arg = 1.0f) => 1.0f;

			public virtual object Int_default(int arg = default) => default(int);
			public virtual object Int_non_default(int arg = 1) => 1;
			public virtual object Int_nullable_null(int? arg = null) => null;
			public virtual object Int_nullable_default(int? arg = default(int)) => default(int);
			public virtual object Int_nullable_non_default(int? arg = 1) => 1;

			public virtual object Long_default(long arg = default) => default(long);
			public virtual object Long_non_default(long arg = 1L) => 1L;
			public virtual object Long_non_default_from_attribute([Optional, DefaultParameterValue(1L)] long arg) => 1L;
			public virtual object Long_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1)] long arg) => 1L;
			public virtual object Long_nullable_null(long? arg = null) => null;
			//public virtual object Long_nullable_null_from_attribute([Optional, DefaultParameterValue(null)] long? arg) => null;
			public virtual object Long_nullable_default(long? arg = default(long)) => default(long);
			public virtual object Long_nullable_non_default(long? arg = 1L) => 1L;
			public virtual object Long_nullable_non_default_from_attribute([Optional, DefaultParameterValue(1L)] long? arg) => 1L;
			public virtual object Long_nullable_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1)] long? arg) => 1L;

			public virtual object Object_null(object arg = null) => null;
			public virtual object Object_default(object arg = default) => default(object);
			//public virtual object Object_non_default(object arg = new object()) => new object();
			public virtual object Object_non_default_from_attribute([Optional, DefaultParameterValue("1")]object arg) => "1";

			public virtual object SByte_default(sbyte arg = default) => default(sbyte);
			public virtual object SByte_non_default(sbyte arg = (sbyte)1) => (sbyte)1;
			public virtual object SByte_nullable_null(sbyte? arg = null) => null;
			public virtual object SByte_nullable_default(sbyte? arg = default(sbyte)) => default(sbyte);
			public virtual object SByte_nullable_non_default(sbyte? arg = (sbyte)1) => (sbyte)1;

			public virtual object Short_default(short arg = default) => default(short);
			public virtual object Short_non_default(short arg = (short)1) => (short)1;
			public virtual object Short_nullable_null(short? arg = null) => null;
			public virtual object Short_nullable_default(short? arg = default(short)) => default(short);
			public virtual object Short_nullable_non_default(short? arg = (short)1) => (short)1;

			public virtual object String_null(string arg = null) => null;
			public virtual object String_default(string arg = default) => default(string);
			public virtual object String_non_default(string arg = "1") => "1";

			public virtual object UInt_default(uint arg = default) => default(uint);
			public virtual object UInt_non_default(uint arg = 1u) => 1u;
			public virtual object UInt_nullable_null(uint? arg = null) => null;
			public virtual object UInt_nullable_default(uint? arg = default(uint)) => default(uint);
			public virtual object UInt_nullable_non_default(uint? arg = 1u) => 1u;

			public virtual object ULong_default(ulong arg = default) => default(ulong);
			public virtual object ULong_non_default(ulong arg = 1ul) => 1ul;
			public virtual object ULong_nullable_null(ulong? arg = null) => null;
			public virtual object ULong_nullable_default(ulong? arg = default(ulong)) => default(ulong);
			public virtual object ULong_nullable_non_default(ulong? arg = 1ul) => 1ul;

			public virtual object UserDefinedClass_null(UserDefinedClass arg = null) => null;
			public virtual object UserDefinedClass_default(UserDefinedClass arg = default) => default(UserDefinedClass);
			//public virtual object UserDefinedClass_non_default(UserDefinedClass arg = new UserDefinedClass(...)) => new UserDefinedClass(...);

			public virtual object UserDefinedEnum_default(UserDefinedEnum arg = default) => default(UserDefinedEnum);
			public virtual object UserDefinedEnum_non_default(UserDefinedEnum arg = (UserDefinedEnum)1) => (UserDefinedEnum)1;
			public virtual object UserDefinedEnum_non_default_from_attribute([Optional, DefaultParameterValue((UserDefinedEnum)1)] UserDefinedEnum arg) => (UserDefinedEnum)1;
			//public virtual object UserDefinedEnum_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1)] UserDefinedEnum arg) => (UserDefinedEnum)1;
			public virtual object UserDefinedEnum_nullable_null(UserDefinedEnum? arg = null) => null;
			//public virtual object UserDefinedEnum_nullable_null_from_attribute([Optional, DefaultParameterValue(null)] UserDefinedEnum? arg) => null;
			public virtual object UserDefinedEnum_nullable_default(UserDefinedEnum? arg = default(UserDefinedEnum)) => (int)default(UserDefinedEnum);
			public virtual object UserDefinedEnum_nullable_default_from_attribute([Optional, DefaultParameterValue(default(UserDefinedEnum))]UserDefinedEnum? arg) => (int)default(UserDefinedEnum);
			//public virtual object UserDefinedEnum_nullable_default_from_attribute_wrong_type([Optional, DefaultParameterValue(0)]UserDefinedEnum? arg) => (int)default(UserDefinedEnum);
			public virtual object UserDefinedEnum_nullable_non_default(UserDefinedEnum? arg = (UserDefinedEnum)1) => 1;
			public virtual object UserDefinedEnum_nullable_non_default_from_attribute([Optional, DefaultParameterValue((UserDefinedEnum)1)] UserDefinedEnum? arg) => 1;
			//public virtual object UserDefinedEnum_nullable_non_default_from_attribute_wrong_type([Optional, DefaultParameterValue(1)] UserDefinedEnum? arg) => 1;

			public virtual object UserDefinedStruct_default(UserDefinedStruct arg = default) => null;
			//public virtual object UserDefinedStruct_default_from_attribute([Optional, DefaultParameterValue(default(UserDefinedStruct))] UserDefinedStruct arg) => null;
			//public virtual object UserDefinedStruct_non_default(UserDefinedStruct arg = new UserDefinedStruct(...)) => new UserDefinedStruct(...);
			public virtual object UserDefinedStruct_nullable_null(UserDefinedStruct? arg = null) => null;
			//public virtual object UserDefinedStruct_nullable_null_from_attribute([Optional, DefaultParameterValue(null)] UserDefinedStruct? arg) => null;
			//public virtual object UserDefinedStruct_nullable_default(UserDefinedStruct? arg = default(UserDefinedStruct)) => default(UserDefinedStruct);
			//public virtual object UserDefinedStruct_nullable_non_default(UserDefinedStruct? arg = new UserDefinedStruct(...)) => new UserDefinedStruct(...);

			public virtual object UShort_default(ushort arg = default) => default(ushort);
			public virtual object UShort_non_default(ushort arg = (ushort)1) => (ushort)1;
			public virtual object UShort_nullable_null(ushort? arg = null) => null;
			public virtual object UShort_nullable_default(ushort? arg = default(ushort)) => default(ushort);
			public virtual object UShort_nullable_non_default(ushort? arg = (ushort)1) => (ushort)1;
		}

		public class HasDefaultValue<TParameter>
		{
			// This class has methods with optional parameters of a generic type
			// whose default value is the default value of the generic type parameter.
			// Note that in these cases, reflection is always expected to report a default
			// value of `null`!

			public virtual object Method(TParameter arg = default) => null;
		}

		public class HasNoDefaultValues
		{
			// This class has methods with parameters having no default values.
			// This is here mostly to demonstrate what the special values `DBNull.Value` and
			// `Missing.Value` mean that reflection sometimes reports.

			public virtual object Not_optional(object arg) => DBNull.Value;

			public virtual object No_default([Optional] object arg) => Missing.Value;
		}
	}

	public class UserDefinedClass
	{
	}

	public enum UserDefinedEnum
	{
	}

	public struct UserDefinedStruct
	{
	}
}
