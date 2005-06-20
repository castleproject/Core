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

	public enum OuterJoinEnum
	{
		Auto,
		Yes,
		No
	}

	public enum CascadeEnum
	{
		None,
		All,
		SaveUpdate,
		Delete
	}

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
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class BelongsToAttribute : Attribute
	{
		private Type _type;
		private String _column;
		private bool _update = true;
		private bool _insert = true;
		private bool _notnull;
		private bool _unique;
		private OuterJoinEnum _outerJoin = OuterJoinEnum.Auto;
		private CascadeEnum _cascade = CascadeEnum.None;

		public BelongsToAttribute(String column)
		{
			_column = column;
		}

		public BelongsToAttribute(Type type)
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

		public CascadeEnum Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public OuterJoinEnum OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}

		public bool Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public bool Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}

		public bool NotNull
		{
			get { return _notnull; }
			set { _notnull = value; }
		}

		public bool Unique
		{
			get { return _unique; }
			set { _unique = value; }
		}
	}
}
