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
	/// Define the possible strategies to set the Primary Key values
	/// </summary>
	[Serializable]
	public enum PrimaryKeyType
	{
		/// <summary>
		/// Use Identity column (auto number)
		/// Note: This force an immediate call to the DB when Create() is called
		/// </summary>
		Identity,
		/// <summary>
		/// Use a sequence
		/// </summary>
		Sequence,
		/// <summary>
		/// Use the HiLo algorithm to get the next value
		/// </summary>
		HiLo,
		/// <summary>
		/// Use a sequence and a HiLo algorithm - better performance on Oracle
		/// </summary>
		SeqHiLo,
		/// <summary>
		/// Use the hex representation of a unique identifier
		/// </summary>
		UuidHex,
		/// <summary>
		/// Use the string representation of a unique identifier
		/// </summary>
		UuidString,
		/// <summary>
		/// Generate a Guid for the primary key
		/// Note: You should prefer using GuidComb over this value.
		/// </summary>
		Guid,
		/// <summary>
		/// Generate a Guid in sequence, so it will have better insert performance in the DB.
		/// </summary>
		GuidComb,
		/// <summary>
		/// Use an identity or sequence if supported by the database, otherwise, use the HiLo algorithm
		/// </summary>
		Native,
		/// <summary>
		/// The primary key value is always assigned.
		/// Note: using this you will lose the ability to call Save(), and will need to call Create() or Update()
		/// explicitly.
		/// </summary>
		Assigned,
		/// <summary>
		/// This is a foreign key to another table
		/// </summary>
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
		private PrimaryKeyType generator = PrimaryKeyType.Native;
		private String column;
		private String unsavedValue;
		private String sequenceName;
		private String type;
		private String _params;
		private int length;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		public PrimaryKeyAttribute() : this(PrimaryKeyType.Native)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public PrimaryKeyAttribute(PrimaryKeyType generator)
		{
			this.generator = generator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <param name="column">The column.</param>
		public PrimaryKeyAttribute(PrimaryKeyType generator, String column) : this(generator)
		{
			this.column = column;
		}

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public PrimaryKeyType Generator
		{
			get { return generator; }
			set { generator = value; }
		}

		/// <summary>
		/// Gets or sets the column name
		/// </summary>
		/// <value>The column.</value>
		public String Column
		{
			get { return column; }
			set { column = value; }
		}

		/// <summary>
		/// Gets or sets the unsaved value.
		/// </summary>
		/// <value>The unsaved value.</value>
		public String UnsavedValue
		{
			get { return unsavedValue; }
			set { unsavedValue = value; }
		}

		/// <summary>
		/// Gets or sets the name of the sequence.
		/// </summary>
		/// <value>The name of the sequence.</value>
		public String SequenceName
		{
			get { return sequenceName; }
			set { sequenceName = value; }
		}

		/// <summary>
		/// Gets or sets the type of the column.
		/// </summary>
		/// <value>The type of the column.</value>
		public String ColumnType
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the length of values in the column
		/// </summary>
		/// <value>The length.</value>
		public int Length
		{
			get { return length; }
			set { length = value; }
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
