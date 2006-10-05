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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Maps a one to many association.
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	/// 	[HasMany(typeof(Post), RelationType.Bag, Key="Posts", Table="Posts", Column="blogid")]
	///		public IList Posts
	///		{
	///			get { return _posts; }
	///			set { _posts = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class HasManyAttribute : RelationAttribute
	{
		protected String keyColumn;
		protected String[] compositeKeyColumns;

		public HasManyAttribute()
		{

		}
		
		public HasManyAttribute(Type mapType)
		{
			base.mapType = mapType;
		}

		public HasManyAttribute(Type mapType, String keyColumn, String table)
		{
			this.keyColumn = keyColumn;
			base.mapType = mapType;
			base.table = table;
		}

		public String ColumnKey
		{
			get { return keyColumn; }
			set { keyColumn = value; }
		}
	    
		public String[] CompositeKeyColumnKeys
		{
			get { return this.compositeKeyColumns; }
			set { this.compositeKeyColumns = value; }
		}
	}
}
