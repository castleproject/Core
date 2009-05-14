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
	using System;

	[ActiveRecord(Lazy = false)]
	public class SimpleClass : ActiveRecordBase
	{
		private int id;
		private String name;

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	[ActiveRecord(Lazy = false)]
	public class SimpleClassOverride : SimpleClass
	{
		[PrimaryKey(IsOverride=true, Generator=PrimaryKeyType.Assigned)]
		public override int Id
		{
			get { return base.Id; }
			set { base.Id = value; }
		}

		[Property(IsOverride=true, Length=50)]
		public override string Name
		{
			get { return base.Name; }
			set { base.Name = value; }
		}
	}
}
