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
	/// Specify that this property is used for timestamping this entity
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class TimestampAttribute : WithAccessAttribute
	{
		private String column;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimestampAttribute"/> class.
		/// </summary>
		public TimestampAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimestampAttribute"/> class.
		/// </summary>
		/// <param name="column">The column name</param>
		public TimestampAttribute(String column)
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
	}
}
