// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	/// Defines the values for the generator for the Collection Id values.w
	/// </summary>
	[Serializable]
	public enum CollectionIDType
	{
		/// <summary>
		/// Use Identity column (auto number)
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
		/// The key value is always assigned.
		/// </summary>
		Assigned,
		/// <summary>
		/// This is a foreign key to another table
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionIDAttribute"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <param name="column">The column.</param>
		/// <param name="ColumnType">Type of the column.</param>
		public CollectionIDAttribute(CollectionIDType generator, String column, String ColumnType)
		{
			this.generator = generator;
			this.column = column;
			this.type = ColumnType;
		}

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public CollectionIDType Generator
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
		/// Gets or sets the type of the column.
		/// </summary>
		/// <value>The type of the column.</value>
		public String ColumnType
		{
			get { return type; }
			set { type = value; }
		}
	}
}
