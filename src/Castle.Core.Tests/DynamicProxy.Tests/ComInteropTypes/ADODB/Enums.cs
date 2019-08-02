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
	using System.Runtime.InteropServices;

	[Guid("00000543-0000-0010-8000-00aa006d2ea4")]
	public enum AffectEnum
	{
		adAffectCurrent = 1,
		adAffectGroup,
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adAffectAll,
		adAffectAllChapters
	}

	[Guid("0000052e-0000-0010-8000-00aa006d2ea4")]
	public enum CommandTypeEnum
	{
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adCmdUnspecified = -1,
		adCmdUnknown = 8,
		adCmdText = 1,
		adCmdTable = 2,
		adCmdStoredProc = 4,
		adCmdFile = 0x100,
		adCmdTableDirect = 0x200
	}

	[Guid("00000545-0000-0010-8000-00aa006d2ea4")]
	public enum CompareEnum
	{
		adCompareLessThan,
		adCompareEqual,
		adCompareGreaterThan,
		adCompareNotEqual,
		adCompareNotComparable
	}

	[Guid("0000052f-0000-0010-8000-00aa006d2ea4")]
	public enum CursorLocationEnum
	{
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adUseNone = 1,
		adUseServer = 2,
		adUseClient = 3,
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adUseClientBatch = 3
	}

	[Guid("0000051c-0000-0010-8000-00aa006d2ea4")]
	public enum CursorOptionEnum
	{
		adHoldRecords = 0x100,
		adMovePrevious = 0x200,
		adAddNew = 16778240,
		adDelete = 16779264,
		adUpdate = 16809984,
		adBookmark = 0x2000,
		adApproxPosition = 0x4000,
		adUpdateBatch = 0x10000,
		adResync = 0x20000,
		adNotify = 0x40000,
		adFind = 0x80000,
		adSeek = 0x400000,
		adIndex = 0x800000
	}

	[Guid("0000051b-0000-0010-8000-00aa006d2ea4")]
	public enum CursorTypeEnum
	{
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adOpenUnspecified = -1,
		adOpenForwardOnly,
		adOpenKeyset,
		adOpenDynamic,
		adOpenStatic
	}

	[Guid("0000051f-0000-0010-8000-00aa006d2ea4")]
	public enum DataTypeEnum
	{
		adEmpty = 0,
		adTinyInt = 0x10,
		adSmallInt = 2,
		adInteger = 3,
		adBigInt = 20,
		adUnsignedTinyInt = 17,
		adUnsignedSmallInt = 18,
		adUnsignedInt = 19,
		adUnsignedBigInt = 21,
		adSingle = 4,
		adDouble = 5,
		adCurrency = 6,
		adDecimal = 14,
		adNumeric = 131,
		adBoolean = 11,
		adError = 10,
		adUserDefined = 132,
		adVariant = 12,
		adIDispatch = 9,
		adIUnknown = 13,
		adGUID = 72,
		adDate = 7,
		adDBDate = 133,
		adDBTime = 134,
		adDBTimeStamp = 135,
		adBSTR = 8,
		adChar = 129,
		adVarChar = 200,
		adLongVarChar = 201,
		adWChar = 130,
		adVarWChar = 202,
		adLongVarWChar = 203,
		adBinary = 0x80,
		adVarBinary = 204,
		adLongVarBinary = 205,
		adChapter = 136,
		adFileTime = 0x40,
		adPropVariant = 138,
		adVarNumeric = 139,
		adArray = 0x2000
	}

	[Guid("00000526-0000-0010-8000-00aa006d2ea4")]
	public enum EditModeEnum
	{
		adEditNone = 0,
		adEditInProgress = 1,
		adEditAdd = 2,
		adEditDelete = 4
	}

	[Guid("0000051d-0000-0010-8000-00aa006d2ea4")]
	public enum LockTypeEnum
	{
		[TypeLibVar(TypeLibVarFlags.FHidden)]
		adLockUnspecified = -1,
		adLockReadOnly = 1,
		adLockPessimistic = 2,
		adLockOptimistic = 3,
		adLockBatchOptimistic = 4
	}

	[Guid("00000540-0000-0010-8000-00aa006d2ea4")]
	public enum MarshalOptionsEnum
	{
		adMarshalAll,
		adMarshalModifiedOnly
	}

	[Guid("0000052c-0000-0010-8000-00aa006d2ea4")]
	public enum ParameterDirectionEnum
	{
		adParamUnknown,
		adParamInput,
		adParamOutput,
		adParamInputOutput,
		adParamReturnValue
	}

	[Guid("00000548-0000-0010-8000-00aa006d2ea4")]
	public enum PersistFormatEnum
	{
		adPersistADTG,
		adPersistXML
	}

	[Guid("00000528-0000-0010-8000-00aa006d2ea4")]
	public enum PositionEnum
	{
		adPosUnknown = -1,
		adPosBOF = -2,
		adPosEOF = -3
	}

	[Guid("00000544-0000-0010-8000-00aa006d2ea4")]
	public enum ResyncEnum
	{
		adResyncUnderlyingValues = 1,
		adResyncAllValues
	}

	[Guid("00000547-0000-0010-8000-00aa006d2ea4")]
	public enum SearchDirectionEnum
	{
		adSearchForward = 1,
		adSearchBackward = -1
	}

	[Guid("00000552-0000-0010-8000-00aa006d2ea4")]
	public enum SeekEnum
	{
		adSeekFirstEQ = 1,
		adSeekLastEQ = 2,
		adSeekAfterEQ = 4,
		adSeekAfter = 8,
		adSeekBeforeEQ = 0x10,
		adSeekBefore = 0x20
	}

	[Guid("00000549-0000-0010-8000-00aa006d2ea4")]
	public enum StringFormatEnum
	{
		adClipString = 2
	}
}

#endif
