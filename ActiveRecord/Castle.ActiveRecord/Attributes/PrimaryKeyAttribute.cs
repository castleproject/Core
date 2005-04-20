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

	public enum PrimaryKeyType
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
		Native,
		Assigned,
		Foreign
	}

	/// <summary>
	/// Indicates the property which is the primary key.
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[PrimaryKey(PrimaryKeyType.Native)]
	///		public int Id
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class PrimaryKeyAttribute : Attribute
	{
		private PrimaryKeyType _generator = PrimaryKeyType.Native;
		private String _column;
		private String _unsavedValue;
		private bool _composite;

		public PrimaryKeyAttribute() : this(PrimaryKeyType.Native)
		{
		}

		public PrimaryKeyAttribute(PrimaryKeyType generator)
		{
			_generator = generator;
		}

		public PrimaryKeyAttribute(PrimaryKeyType generator, String column) : this(generator)
		{
			_column = column;
		}

		public PrimaryKeyType Generator
		{
			get { return _generator; }
			set { _generator = value; }
		}

		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public String UnsavedValue
		{
			get { return _unsavedValue; }
			set { _unsavedValue = value; }
		}

		public bool IsComposite
		{
			get { return _composite; }
			set { _composite = value; }
		}
	}
}
