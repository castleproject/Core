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
	/// This attribute is used to specify that a property is the versioning property of the class
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class VersionAttribute : WithAccessAttribute
	{
		private String column, type, unsavedValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="VersionAttribute"/> class.
		/// </summary>
		public VersionAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VersionAttribute"/> class.
		/// </summary>
		/// <param name="column">The column.</param>
		public VersionAttribute(String column)
		{
			this.column = column;
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
		/// Gets or sets the type of the column (should be an integer of some type)
		/// </summary>
		/// <value>The type.</value>
		public String Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the unsaved value for this column
		/// </summary>
		/// <value>The unsaved value.</value>
		public String UnsavedValue
		{
			get { return unsavedValue; }
			set { unsavedValue = value; }
		}
	}
}
