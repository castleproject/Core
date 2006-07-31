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

namespace Castle.ActiveRecord.Tests.Model.Nested
{
	using System;

	[ActiveRecord]
	public class NestedParent : ActiveRecordBase
	{
		private int id;
		private INestedInner inner;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Nested(MapType = typeof(NestedInner))]
		public INestedInner Inner
		{
			get { return inner; }
			set { inner = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(NestedParent));
		}

		public static NestedParent[] FindAll()
		{
			return (NestedParent[]) FindAll(typeof(NestedParent));
		}

		public static NestedParent Find(int id)
		{
			return (NestedParent) FindByPrimaryKey(typeof(NestedParent), id);
		}
	}

	public class NestedInner : INestedInner
	{
		private DateTime dateProp;
		private int intProp;

		[Property]
		public int IntProp
		{
			get { return intProp; }
			set { intProp = value; }
		}

		[Property]
		public DateTime DateProp
		{
			get { return dateProp; }
			set { dateProp = value; }
		}
	}

	public interface INestedInner
	{
		int IntProp { get; set; }

		DateTime DateProp { get; set; }
	}
}
