// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System.Collections;

	[ActiveRecord]
	public class NotFoundBehaviourClass : ActiveRecordBase
	{
		private int id;
		private IList subclasses;
		
		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasMany(typeof(RelationalFoobar), "keycol", "RelationalFoobarTable", NotFoundBehaviour = NotFoundBehaviour.Ignore)]
		public IList SubClasses
		{
			get { return subclasses; }
			set { subclasses = value; }
		}

		[HasAndBelongsToMany(typeof(RelationalFoobar), Table = "ManySubClasses", ColumnKey = "id", ColumnRef = "ref_id", NotFoundBehaviour = NotFoundBehaviour.Ignore)]
		public IList ManySubClasses
		{
		    get { return subclasses; }
		    set { subclasses = value; }
		}
	}
	
	[ActiveRecord]
	public class RelationalFoobar : ActiveRecordBase
	{
		private int id;
		private NotFoundBehaviourClass notFoundBehaviourClass;
		private IList notFoundBehaviourClassList;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo(NotFoundBehaviour = NotFoundBehaviour.Ignore)]
		public NotFoundBehaviourClass NotFoundBehaviourClass
		{
			get { return notFoundBehaviourClass; }
			set { notFoundBehaviourClass = value; }
		}

		[HasAndBelongsToMany(typeof(NotFoundBehaviourClass), Table = "ManySubClasses", ColumnKey = "id", ColumnRef = "ref_id", NotFoundBehaviour = NotFoundBehaviour.Ignore)]
		public IList NotFoundBehaviourClassList
		{
		    get { return notFoundBehaviourClassList; }
		    set { notFoundBehaviourClassList = value; }
		}
	}
}
