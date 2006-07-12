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

	[Serializable]
	public enum PrimaryKeyType
	{
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
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class PrimaryKeyAttribute : WithAccessAttribute
	{
		private PrimaryKeyType _generator = PrimaryKeyType.Native;
		private String _column;
		private String _unsavedValue;
		private String _sequenceName;
		private String _type;
		private String _params;
		private int _length;

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

		public String SequenceName
		{
			get { return _sequenceName; }
			set { _sequenceName = value; }
		}

		public String ColumnType
		{
			get { return _type; }
			set { _type = value; }
		}

		public int Length
		{
			get { return _length; }
			set { _length = value; }
		}

		/// <summary>
		/// Comma separated value of parameters to the generator
		/// </summary>
		public String Params
		{
			get { return _params; }
			set { _params = value; }
		}
	}
}
