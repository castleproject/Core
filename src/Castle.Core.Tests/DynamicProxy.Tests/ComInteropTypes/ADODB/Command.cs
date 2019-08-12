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
	using System;
	using System.Reflection;
	using System.Runtime.InteropServices;

	[ComImport]
	[CoClass(typeof(CommandClass))]
	[Guid("b08400bd-f9d1-4d02-b856-71d5dba123e9")]
	public interface Command : _Command
	{
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("00000507-0000-0010-8000-00aa006d2ea4")]
	[TypeLibType(TypeLibTypeFlags.FCanCreate | TypeLibTypeFlags.FLicensed)]
	//[DefaultMember("Parameters")]
	public class CommandClass // : (implemented interfaces omitted)
	{
	}

	[ComImport]
	[DefaultMember("Parameters")]
	[Guid("b08400bd-f9d1-4d02-b856-71d5dba123e9")]
	[TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FNonExtensible)]
	public interface _Command // : (inherited interfaces omitted)
	{
		[DispId(500)]
		Properties Properties
		{
			[DispId(500)]
			//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.PropertiesToInternalPropertiesMarshaler")]
			get;
		}

		[DispId(1)]
		Connection ActiveConnection
		{
			[DispId(1)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;

			[DispId(1)]
			[param: In]
			[param: MarshalAs(UnmanagedType.Interface)]
			set;
		}

		[DispId(1)]
		void let_ActiveConnection(
			[In] [MarshalAs(UnmanagedType.Struct)] object ppvObject);

		[DispId(2)]
		string CommandText
		{
			[DispId(2)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;

			[DispId(2)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		[DispId(3)]
		int CommandTimeout
		{
			[DispId(3)]
			get;

			[DispId(3)]
			[param: In]
			set;
		}

		[DispId(4)]
		bool Prepared
		{
			[DispId(4)]
			get;

			[DispId(4)]
			[param: In]
			set;
		}

		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		Recordset Execute(
			[Optional] [MarshalAs(UnmanagedType.Struct)] out object RecordsAffected,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] ref object Parameters,
			[In] int Options = -1);

		[DispId(6)]
		//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.ParameterToInternalParameterMarshaler")]
		Parameter CreateParameter(
			[In] [MarshalAs(UnmanagedType.BStr)] string Name = "",
			[In] DataTypeEnum Type = DataTypeEnum.adEmpty,
			[In] ParameterDirectionEnum Direction = ParameterDirectionEnum.adParamInput,
			[In] [ComAliasName("ADODB.ADO_LONGPTR")] int Size = 0,
			[In] [MarshalAs(UnmanagedType.Struct)] object Value = null /* default value added */);

		[DispId(0)]
		Parameters Parameters
		{
			[DispId(0)]
			//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.ParametersToInternalParametersMarshaler")]
			get;
		}

		[DispId(7)]
		CommandTypeEnum CommandType
		{
			[DispId(7)]
			[param: In]
			set;

			[DispId(7)]
			get;
		}

		[DispId(8)]
		string Name
		{
			[DispId(8)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;

			[DispId(8)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		[DispId(9)]
		int State
		{
			[DispId(9)]
			get;
		}

		[DispId(10)]
		void Cancel();

		[DispId(11)]
		object CommandStream
		{
			[DispId(11)]
			[param: In]
			[param: MarshalAs(UnmanagedType.IUnknown)]
			set;

			[DispId(11)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;
		}

		[DispId(12)]
		string Dialect
		{
			[DispId(12)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;

			[DispId(12)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		[DispId(13)]
		bool NamedParameters
		{
			[DispId(13)]
			[param: In]
			set;

			[DispId(13)]
			get;
		}
	}
}

#endif
