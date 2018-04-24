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
namespace Castle.DynamicProxy.Tests.Interfaces
{
	using System;
	public interface InterfaceWithMethodsWithAllKindsOfOptionalParameters
	{
		void MethodWithOptionalByteParameter(byte b = 0);

		void MethodWithOptionalNonDefaultByteParameter(byte b = 3);

		void MethodWithOptionalNullableByteParameter(byte? b = null);

		void MethodWithOptionalNonDefaultNullableByteParameter(byte? b = 3);

		void MethodWithOptionalSignedByteParameter(sbyte b = 0);

		void MethodWithOptionalNonDefaultSignedByteParameter(sbyte b = 3);

		void MethodWithOptionalNullableSignedByteParameter(sbyte? b = null);

		void MethodWithOptionalNonDefaultNullableSignedByteParameter(sbyte? b = 3);

		void MethodWithOptionalShortParameter(short s = 0);

		void MethodWithOptionalNonDefaultShortParameter(short s = 3);

		void MethodWithOptionalNullableShortParameter(short? s = null);

		void MethodWithOptionalNonDefaultNullableShortParameter(short? s = 3);

		void MethodWithOptionalUnsignedShortParameter(ushort s = 0);

		void MethodWithOptionalNonDefaultUnsignedShortParameter(ushort s = 3);

		void MethodWithOptionalNullableUnsignedShortParameter(ushort? s = null);

		void MethodWithOptionalNonDefaultNullableUnsignedShortParameter(ushort? s = 3);

		void MethodWithOptionalIntParameter(int i = 0);

		void MethodWithOptionalNonDefaultIntParameter(int i = 3);

		void MethodWithOptionalNullableIntParameter(int? i = null);

		void MethodWithOptionalNonDefaultNullableIntParameter(int? i = 3);

		void MethodWithOptionalUnsignedIntParameter(uint i = 0);

		void MethodWithOptionalNonDefaultUnsignedIntParameter(uint i = 3);

		void MethodWithOptionalNullableUnsignedIntParameter(uint? i = null);

		void MethodWithOptionalNonDefaultNullableUnsignedIntParameter(uint? i = 3);

		void MethodWithOptionalLongParameter(long l = 0);

		void MethodWithOptionalNonDefaultLongParameter(long l = 3L);

		void MethodWithOptionalNullableLongParameter(long? l = null);

		void MethodWithOptionalNonDefaultNullableLongParameter(long? l = 3L);

		void MethodWithOptionalUnsignedLongParameter(ulong l = 0);

		void MethodWithOptionalNonDefaultUnsignedLongParameter(ulong l = 3L);

		void MethodWithOptionalNullableUnsignedLongParameter(ulong? l = null);

		void MethodWithOptionalNonDefaultNullableUnsignedLongParameter(ulong? l = 3L);

		void MethodWithOptionalFloatParameter(float f = 0f);

		void MethodWithOptionalNonDefaultFloatParameter(float f = 3f);

		void MethodWithOptionalNullableFloatParameter(float? f = null);

		void MethodWithOptionalNonDefaultNullableFloatParameter(float? f = 3f);

		void MethodWithOptionalDoubleParameter(double d = 0d);

		void MethodWithOptionalNonDefaultDoubleParameter(double d = 3d);

		void MethodWithOptionalNullableDoubleParameter(double? d = null);

		void MethodWithOptionalNonDefaultNullableDoubleParameter(double? d = 3d);

		void MethodWithOptionalCharParameter(char c = '\0');

		void MethodWithOptionalNonDefaultCharParameter(char c = 'A');

		void MethodWithOptionalNullableCharParameter(char? c = null);

		void MethodWithOptionalNonDefaultNullableCharParameter(char? c = 'A');

		void MethodWithOptionalBoolParameter(bool b = false);

		void MethodWithOptionalNonDefaultBoolParameter(bool b = true);

		void MethodWithOptionalNullableBoolParameter(bool? b = null);

		void MethodWithOptionalNonDefaultNullableBoolParameter(bool? b = true);

		void MethodWithOptionalObjectParameter(object o = null);

		void MethodWithOptionalNullStringParameter(string s = null);

		void MethodWithOptionalStringParameter(string s = "");

		void MethodWithOptionalDecimalParameter(decimal d = 0m);

		void MethodWithOptionalNonDefaultDecimalParameter(decimal d = 3m);

		void MethodWithOptionalNullableDecimalarameter(decimal? d = null);

		void MethodWithOptionalNonDefaultNullableDecimalParameter(decimal? d = 3m);
		void MethodWithOptionalNullableEnumParameter(ConsoleColor? c = null);
		void MethodWithOptionalNonDefaultNullableEnumParameter(ConsoleColor? c = ConsoleColor.Cyan);
	}
}
#endif