// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

#if !DOTNET35
namespace Castle.DynamicProxy.Tests.Classes
{
	using System;

	using Castle.DynamicProxy.Tests.Interfaces;

	public class ClassWithMethodsWithAllKindsOfOptionalParameters : InterfaceWithMethodsWithAllKindsOfOptionalParameters
	{
		public virtual void MethodWithOptionalByteParameter(byte b = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultByteParameter(byte b = 3)
		{
		}

		public virtual void MethodWithOptionalNullableByteParameter(byte? b = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableByteParameter(byte? b = 3)
		{
		}

		public virtual void MethodWithOptionalSignedByteParameter(sbyte b = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultSignedByteParameter(sbyte b = 3)
		{
		}

		public virtual void MethodWithOptionalNullableSignedByteParameter(sbyte? b = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableSignedByteParameter(sbyte? b = 3)
		{
		}

		public virtual void MethodWithOptionalShortParameter(short s = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultShortParameter(short s = 3)
		{
		}

		public virtual void MethodWithOptionalNullableShortParameter(short? s = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableShortParameter(short? s = 3)
		{
		}

		public virtual void MethodWithOptionalUnsignedShortParameter(ushort s = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultUnsignedShortParameter(ushort s = 3)
		{
		}

		public virtual void MethodWithOptionalNullableUnsignedShortParameter(ushort? s = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableUnsignedShortParameter(ushort? s = 3)
		{
		}

		public virtual void MethodWithOptionalIntParameter(int i = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultIntParameter(int i = 3)
		{
		}

		public virtual void MethodWithOptionalNullableIntParameter(int? i = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableIntParameter(int? i = 3)
		{
		}

		public virtual void MethodWithOptionalUnsignedIntParameter(uint i = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultUnsignedIntParameter(uint i = 3)
		{
		}

		public virtual void MethodWithOptionalNullableUnsignedIntParameter(uint? i = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableUnsignedIntParameter(uint? i = 3)
		{
		}

		public virtual void MethodWithOptionalLongParameter(long l = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultLongParameter(long l = 3L)
		{
		}

		public virtual void MethodWithOptionalNullableLongParameter(long? l = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableLongParameter(long? l = 3L)
		{
		}

		public virtual void MethodWithOptionalUnsignedLongParameter(ulong l = 0)
		{
		}

		public virtual void MethodWithOptionalNonDefaultUnsignedLongParameter(ulong l = 3L)
		{
		}

		public virtual void MethodWithOptionalNullableUnsignedLongParameter(ulong? l = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableUnsignedLongParameter(ulong? l = 3L)
		{
		}

		public virtual void MethodWithOptionalFloatParameter(float f = 0f)
		{
		}

		public virtual void MethodWithOptionalNonDefaultFloatParameter(float f = 3f)
		{
		}

		public virtual void MethodWithOptionalNullableFloatParameter(float? f = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableFloatParameter(float? f = 3f)
		{
		}

		public virtual void MethodWithOptionalDoubleParameter(double d = 0d)
		{
		}

		public virtual void MethodWithOptionalNonDefaultDoubleParameter(double d = 3d)
		{
		}

		public virtual void MethodWithOptionalNullableDoubleParameter(double? d = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableDoubleParameter(double? d = 3d)
		{
		}

		public virtual void MethodWithOptionalCharParameter(char c = '\0')
		{
		}

		public virtual void MethodWithOptionalNonDefaultCharParameter(char c = 'A')
		{
		}

		public virtual void MethodWithOptionalNullableCharParameter(char? c = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableCharParameter(char? c = 'A')
		{
		}

		public virtual void MethodWithOptionalBoolParameter(bool b = false)
		{
		}

		public virtual void MethodWithOptionalNonDefaultBoolParameter(bool b = true)
		{
		}

		public virtual void MethodWithOptionalNullableBoolParameter(bool? b = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableBoolParameter(bool? b = true)
		{
		}

		public virtual void MethodWithOptionalObjectParameter(object o = null)
		{
		}

		public virtual void MethodWithOptionalNullStringParameter(string s = null)
		{
		}

		public virtual void MethodWithOptionalStringParameter(string s = "")
		{
		}

		public virtual void MethodWithOptionalDecimalParameter(decimal d = 0m)
		{
		}

		public virtual void MethodWithOptionalNonDefaultDecimalParameter(decimal d = 3m)
		{
		}

		public virtual void MethodWithOptionalNullableDecimalarameter(decimal? d = null)
		{
		}

		public virtual void MethodWithOptionalNonDefaultNullableDecimalParameter(decimal? d = 3m)
		{
		}
		public void MethodWithOptionalNullableEnumParameter(ConsoleColor? c = null)
		{
		}
		public void MethodWithOptionalNonDefaultNullableEnumParameter(ConsoleColor? c = ConsoleColor.Cyan)
		{
		}
	}
}
#endif