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

	/// <summary>
	/// Maps a one to one association.
	/// </summary>
	/// <example>
	/// <code>
	/// public class Post : ActiveRecordBase
	/// {
	///		...
	///		
	/// 	[BelongsTo("blogid")]
	///		public Blog Blog
	///		{
	/// 		get { return _blog; }
	/// 		set { _blog = value; }
	///		}
	///	</code>
	/// </example>
	/// <remarks>
	/// Please note that the 'blogid' foreign key lies on the 'Post' table.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class BelongsToAttribute : Attribute
	{
		private Type _type;
		private String _column;
		private String _cascade;
		private String _outerJoin;
		private String _update;
		private String _insert;

		public BelongsToAttribute(String column)
		{
			_column = column;
		}

		public BelongsToAttribute( Type type )
		{
			_type = type;
		}

		public Type Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public String Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public String OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}

		public String Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public String Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}
	}
}
