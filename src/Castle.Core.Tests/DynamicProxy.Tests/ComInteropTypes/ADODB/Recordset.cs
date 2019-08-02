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
	[Guid("00000556-0000-0010-8000-00aa006d2ea4")]
	[CoClass(typeof(RecordsetClass))]
	public interface Recordset : _Recordset //, (inherited event interface omitted)
	{
	}

	[ComImport]
	[ClassInterface(ClassInterfaceType.None)]
	[TypeLibType(TypeLibTypeFlags.FCanCreate | TypeLibTypeFlags.FLicensed)]
	[Guid("00000535-0000-0010-8000-00aa006d2ea4")]
	//[ComSourceInterfaces("ADODB.RecordsetEvents")]
	//[DefaultMember("Fields")]
	public class RecordsetClass // : (implemented interfaces omitted)
	{
	}

	[ComImport]
	[TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FNonExtensible)]
	[DefaultMember("Fields")]
	[Guid("00000556-0000-0010-8000-00aa006d2ea4")]
	public interface _Recordset // : (inherited interfaces omitted)
	{
		[DispId(500)]
		Properties Properties
		{
			[DispId(500)]
			//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.PropertiesToInternalPropertiesMarshaler")]
			get;
		}

		[DispId(1000)]
		PositionEnum AbsolutePosition
		{
			[DispId(1000)]
			get;

			[DispId(1000)]
			[param: In]
			set;
		}

		[DispId(1001)]
		object set_ActiveConnection(
			[In] [MarshalAs(UnmanagedType.IDispatch)] object value);

		[DispId(1001)]
		void let_ActiveConnection(
			[In] [MarshalAs(UnmanagedType.Struct)] object pvar);

		[DispId(1001)]
		[return: MarshalAs(UnmanagedType.Struct)]
		object get_ActiveConnection();

		[DispId(1002)]
		bool BOF
		{
			[DispId(1002)]
			get;
		}

		[DispId(1003)]
		object Bookmark
		{
			[DispId(1003)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;

			[DispId(1003)]
			[param: In]
			[param: MarshalAs(UnmanagedType.Struct)]
			set;
		}

		[DispId(1004)]
		int CacheSize
		{
			[DispId(1004)]
			get;

			[DispId(1004)]
			[param: In]
			set;
		}

		[DispId(1005)]
		CursorTypeEnum CursorType
		{
			[DispId(1005)]
			get;

			[DispId(1005)]
			[param: In]
			set;
		}

		[DispId(1006)]
		bool EOF
		{
			[DispId(1006)]
			get;
		}

		[DispId(0)]
		Fields Fields
		{
			[DispId(0)]
			//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "ADODB.FieldsToInternalFieldsMarshaler")]
			get;
		}

		[DispId(1008)]
		LockTypeEnum LockType
		{
			[DispId(1008)]
			get;

			[DispId(1008)]
			[param: In]
			set;
		}

		[ComAliasName("ADODB.ADO_LONGPTR")]
		[DispId(1009)]
		int MaxRecords
		{
			[DispId(1009)]
			[return: ComAliasName("ADODB.ADO_LONGPTR")]
			get;

			[DispId(1009)]
			[param: In]
			[param: ComAliasName("ADODB.ADO_LONGPTR")]
			set;
		}

		[ComAliasName("ADODB.ADO_LONGPTR")]
		[DispId(1010)]
		int RecordCount
		{
			[DispId(1010)]
			[return: ComAliasName("ADODB.ADO_LONGPTR")]
			get;
		}

		[DispId(1011)]
		void set_Source(
			[In] [MarshalAs(UnmanagedType.IDispatch)] object value);

		[DispId(1011)]
		void let_Source(
			[In] [MarshalAs(UnmanagedType.BStr)] string pvSource);

		[DispId(1011)]
		[return: MarshalAs(UnmanagedType.Struct)]
		object get_Source();

		[DispId(1012)]
		void AddNew(
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object FieldList,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Values);

		[DispId(1013)]
		void CancelUpdate();

		[DispId(1014)]
		void Close();

		[DispId(1015)]
		void Delete(
			[In] AffectEnum AffectRecords = AffectEnum.adAffectCurrent);

		[DispId(1016)]
		[return: MarshalAs(UnmanagedType.Struct)]
		object GetRows(
			[In] int Rows /* = -1 */,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Start,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Fields);

		[DispId(1017)]
		void Move(
			[In] [ComAliasName("ADODB.ADO_LONGPTR")] int NumRecords,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Start);

		[DispId(1018)]
		void MoveNext();

		[DispId(1019)]
		void MovePrevious();

		[DispId(1020)]
		void MoveFirst();

		[DispId(1021)]
		void MoveLast();

		[DispId(1022)]
		void Open(
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Source,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object ActiveConnection,
			[In] CursorTypeEnum CursorType = CursorTypeEnum.adOpenUnspecified,
			[In] LockTypeEnum LockType = LockTypeEnum.adLockUnspecified,
			[In] int Options = -1);

		[DispId(1023)]
		void Requery([In] int Options = -1);

		[DispId(1610809378)]
		[TypeLibFunc(TypeLibFuncFlags.FHidden)]
		void _xResync([In] AffectEnum AffectRecords = AffectEnum.adAffectAll);

		[DispId(1025)]
		void Update(
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Fields,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Values);

		[DispId(1047)]
		PositionEnum AbsolutePage
		{
			[DispId(1047)]
			get;

			[DispId(1047)]
			[param: In]
			set;
		}

		[DispId(1026)]
		EditModeEnum EditMode
		{
			[DispId(1026)]
			get;
		}

		[DispId(1030)]
		object Filter
		{
			[DispId(1030)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;

			[DispId(1030)]
			[param: In]
			[param: MarshalAs(UnmanagedType.Struct)]
			set;
		}

		[DispId(1050)]
		[ComAliasName("ADODB.ADO_LONGPTR")]
		int PageCount
		{
			[DispId(1050)]
			[return: ComAliasName("ADODB.ADO_LONGPTR")]
			get;
		}

		[DispId(1048)]
		int PageSize
		{
			[DispId(1048)]
			get;

			[DispId(1048)]
			[param: In]
			set;
		}

		[DispId(1031)]
		string Sort
		{
			[DispId(1031)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;

			[DispId(1031)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		[DispId(1029)]
		int Status
		{
			[DispId(1029)]
			get;
		}

		[DispId(1054)]
		int State
		{
			[DispId(1054)]
			get;
		}

		[DispId(1610809392)]
		[TypeLibFunc(TypeLibFuncFlags.FHidden)]
		[return: MarshalAs(UnmanagedType.Interface)]
		Recordset _xClone();

		[DispId(1035)]
		void UpdateBatch(
			[In] AffectEnum AffectRecords = AffectEnum.adAffectAll);

		[DispId(1049)]
		void CancelBatch(
			[In] AffectEnum AffectRecords = AffectEnum.adAffectAll);

		[DispId(1051)]
		CursorLocationEnum CursorLocation
		{
			[DispId(1051)]
			get;

			[DispId(1051)]
			[param: In]
			set;
		}

		[DispId(1052)]
		[return: MarshalAs(UnmanagedType.Interface)]
		Recordset NextRecordset(
			[Optional] [MarshalAs(UnmanagedType.Struct)] out object RecordsAffected);

		[DispId(1036)]
		bool Supports(
			[In] CursorOptionEnum CursorOptions);

		[DispId(-8)]
		object Collect
		{
			[DispId(-8)]
			[TypeLibFunc(64)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;

			[DispId(-8)]
			[TypeLibFunc(64)]
			[param: In]
			[param: MarshalAs(UnmanagedType.Struct)]
			set;
		}

		[DispId(1053)]
		MarshalOptionsEnum MarshalOptions
		{
			[DispId(1053)]
			get;

			[DispId(1053)]
			[param: In]
			set;
		}

		[DispId(1058)]
		void Find(
			[In] [MarshalAs(UnmanagedType.BStr)] string Criteria,
			[In] [ComAliasName("ADODB.ADO_LONGPTR")] int SkipRecords /* = 0 */,
			[In] SearchDirectionEnum SearchDirection /* = SearchDirectionEnum.adSearchForward */,
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Start);

		[DispId(1055)]
		void Cancel();

		[DispId(1056)]
		object DataSource
		{
			[DispId(1056)]
			[return: MarshalAs(UnmanagedType.IUnknown)]
			get;

			[DispId(1056)]
			[param: In]
			[param: MarshalAs(UnmanagedType.IUnknown)]
			set;
		}

		[TypeLibFunc(64)]
		[DispId(1610874883)]
		void _xSave(
			[In] [MarshalAs(UnmanagedType.BStr)] string FileName = "",
			[In] PersistFormatEnum PersistFormat = PersistFormatEnum.adPersistADTG);

		[DispId(1061)]
		object ActiveCommand
		{
			[DispId(1061)]
			[return: MarshalAs(UnmanagedType.IDispatch)]
			get;
		}

		[DispId(1063)]
		bool StayInSync
		{
			[DispId(1063)]
			[param: In]
			set;

			[DispId(1063)]
			get;
		}

		[DispId(1062)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GetString(
			[In] StringFormatEnum StringFormat = StringFormatEnum.adClipString,
			[In] int NumRows = -1,
			[In] [MarshalAs(UnmanagedType.BStr)] string ColumnDelimeter = "",
			[In] [MarshalAs(UnmanagedType.BStr)] string RowDelimeter = "",
			[In] [MarshalAs(UnmanagedType.BStr)] string NullExpr = "");

		[DispId(1064)]
		string DataMember
		{
			[DispId(1064)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;

			[DispId(1064)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		[DispId(1065)]
		CompareEnum CompareBookmarks(
			[In] [MarshalAs(UnmanagedType.Struct)] object Bookmark1,
			[In] [MarshalAs(UnmanagedType.Struct)] object Bookmark2);

		[DispId(1034)]
		[return: MarshalAs(UnmanagedType.Interface)]
		Recordset Clone(
			[In] LockTypeEnum LockType = LockTypeEnum.adLockUnspecified);

		[DispId(1024)]
		void Resync(
			[In] AffectEnum AffectRecords = AffectEnum.adAffectAll,
			[In] ResyncEnum ResyncValues = ResyncEnum.adResyncAllValues);

		[DispId(1066)]
		void Seek(
			[In] [MarshalAs(UnmanagedType.Struct)] object KeyValues,
			[In] SeekEnum SeekOption = SeekEnum.adSeekFirstEQ);

		[DispId(1067)]
		string Index
		{
			[DispId(1067)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;

			[DispId(1067)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		[DispId(1057)]
		void Save(
			[Optional] [In] [MarshalAs(UnmanagedType.Struct)] object Destination,
			[In] PersistFormatEnum PersistFormat = PersistFormatEnum.adPersistADTG);
	}
}

#endif
