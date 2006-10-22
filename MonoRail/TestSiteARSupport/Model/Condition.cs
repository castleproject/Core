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

namespace TestSiteARSupport.Model
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Condition : ActiveRecordBase
	{
		private int _id;
		private string _name;
		private ConditionType _conditionType;
		private IList _comments;

		[PrimaryKey]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[BelongsTo(Column = "ConditionTypeId")]
		public ConditionType ConditionType
		{
			get { return _conditionType; }
			set { _conditionType = value; }
		}

		[HasMany(typeof(Comment),
			ColumnKey = "ConditionId",
			RelationType = RelationType.Bag,
			Lazy = true,
			Cascade = ManyRelationCascadeEnum.All)]
		public IList Comments
		{
			get { return _comments; }
			set { _comments = value; }
		}
	}
}