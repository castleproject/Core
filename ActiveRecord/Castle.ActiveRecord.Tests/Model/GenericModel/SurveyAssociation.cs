// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

#if DOTNET2

namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System;

	[ActiveRecord("SurveyAssociation")]
	public class SurveyAssociation : ActiveRecordBase
	{
		private Int32 _surveyAssociationId;
		private Int32 _surveyId;
		private Int32? _companyId;
		private Int32? _departmentId;
		private Int32? _userGroupId;
		private Int32? _userProfileId;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int SurveyAssociationId
		{
			get { return this._surveyAssociationId; }
			set { this._surveyAssociationId = value; }
		}

		/// <summary>
		/// Get/Set the surveyId
		/// </summary>
		[Property()]
		public Int32 SurveyId
		{
			get { return this._surveyId; }
			set { this._surveyId = value; }
		}

		/// <summary>
		/// Get/Set the companyId
		/// </summary>
		[Property()]
		public Int32? CompanyId
		{
			get { return this._companyId; }
			set { this._companyId = value; }
		}

		/// <summary>
		/// Get/Set the departmentId
		/// </summary>
		[Property()]
		public Int32? DepartmentId
		{
			get { return this._departmentId; }
			set { this._departmentId = value; }
		}

		/// <summary>
		/// Get/Set the userGroupId
		/// </summary>
		[Property()]
		public Int32? UserGroupId
		{
			get { return this._userGroupId; }
			set { this._userGroupId = value; }
		}

		/// <summary>
		/// Get/Set the userProfileId
		/// </summary>
		[Property()]
		public Int32? UserProfileId
		{
			get { return this._userProfileId; }
			set { this._userProfileId = value; }
		}

		public static SurveyAssociation[] FindAll()
		{
			return ((SurveyAssociation[]) (ActiveRecordBase.FindAll(typeof (SurveyAssociation))));
		}
	}
}

#endif