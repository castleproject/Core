// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

#if FEATURE_TEST_COM

namespace Castle.DynamicProxy.Tests.ComInteropTypes.ADODB
{
	using System.Reflection;
	using System.Runtime.InteropServices;

	[ComImport]
	[CoClass(typeof(ParameterClass))]
	[Guid("0000050c-0000-0010-8000-00aa006d2ea4")]
	public interface Parameter : _Parameter
	{
	}

	[ComImport]
	[TypeLibType(TypeLibTypeFlags.FCanCreate | TypeLibTypeFlags.FLicensed)]
	[DefaultMember("Value")]
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("0000050b-0000-0010-8000-00aa006d2ea4")]
	public class ParameterClass // : (list of implemented interfaces omitted)
	{
	}

	[ComImport]
	[DefaultMember("Value")]
	[Guid("0000050c-0000-0010-8000-00aa006d2ea4")]
	[TypeLibType(4288)]
	public interface _Parameter // : (list of inherited interfaces omitted)
	{
		[DispId(500)]
		Properties Properties
		{
			[DispId(500)]
			//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.PropertiesToInternalPropertiesMarshaler")]
			get;
		}

		[DispId(1)]
		string Name
		{
			[DispId(1)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;

			[DispId(1)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		[DispId(0)]
		object Value
		{
			[DispId(0)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;

			[DispId(0)]
			[param: In]
			[param: MarshalAs(UnmanagedType.Struct)]
			set;
		}

		[DispId(2)]
		DataTypeEnum Type
		{
			[DispId(2)]
			get;

			[DispId(2)]
			[param: In]
			set;
		}

		[DispId(3)]
		ParameterDirectionEnum Direction
		{
			[DispId(3)]
			[param: In]
			set;

			[DispId(3)]
			get;
		}

		[DispId(4)]
		byte Precision
		{
			[DispId(4)]
			[param: In]
			set;

			[DispId(4)]
			get;
		}

		[DispId(5)]
		byte NumericScale
		{
			[DispId(5)]
			[param: In]
			set;

			[DispId(5)]
			get;
		}

		[ComAliasName("ADODB.ADO_LONGPTR")]
		[DispId(6)]
		int Size
		{
			[DispId(6)]
			[param: In]
			[param: ComAliasName("ADODB.ADO_LONGPTR")]
			set;

			[DispId(6)]
			[return: ComAliasName("ADODB.ADO_LONGPTR")]
			get;
		}

		[DispId(7)]
		void AppendChunk(
			[In] [MarshalAs(UnmanagedType.Struct)] object Val);

		[DispId(8)]
		int Attributes
		{
			[DispId(8)]
			get;

			[DispId(8)]
			[param: In]
			set;
		}
	}
}

#endif