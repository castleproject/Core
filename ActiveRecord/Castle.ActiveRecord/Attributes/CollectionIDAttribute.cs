// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;

	[Serializable]
	public enum CollectionIDType
	{
		None,
		Identity,
		Sequence,
		HiLo,
		SeqHiLo,
		UuidHex,
		UuidString,
		Guid,
		GuidComb,
		Assigned,
		Foreign
	} 

	/// <summary>
	/// Used for a collection that requires a collection id.
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[HasManyAndBelongs/HasMany]
	///		[CollectionIDAttribute(CollectionIDAttribute.Native)]
	///		public int Id
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class CollectionIDAttribute : Attribute
	{
		private CollectionIDType generator = CollectionIDType.Assigned;
		private String column;
		private String type;

		public CollectionIDAttribute(CollectionIDType generator, String column, String ColumnType)
		{
			this.generator = generator;
			this.column = column;
			this.type = ColumnType;
		}

		public CollectionIDType Generator
		{
			get { return generator; }
			set { generator = value; }
		}

		public String Column
		{
			get { return column; }
			set { column = value; }
		}

		public String ColumnType
		{
			get { return type; }
			set { type = value; }
		}
	}
}
