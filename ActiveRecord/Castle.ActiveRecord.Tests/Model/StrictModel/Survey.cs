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

namespace Castle.ActiveRecord.Tests.Model.StrictModel
{
	using System;

	using Iesi.Collections;

	[ActiveRecord(DiscriminatorValue="2")]
	public class Survey : QuestionContainer
	{
		public Survey()
		{
		}

		#region static methods

		public new static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Survey) );
		}

		public static Survey[] FindAll()
		{
			return (Survey[]) ActiveRecordBase.FindAll( typeof(Survey) );
		}

		public new static Survey Find(int id)
		{
			return (Survey) ActiveRecordBase.FindByPrimaryKey( typeof(Survey), id );
		}

		#endregion
	}
}
