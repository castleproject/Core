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
	/// Used for joined subclasses.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class JoinedKeyAttribute : Attribute
	{
		private string _column;

		/// <summary>
		/// Initializes a new instance of the <see cref="JoinedKeyAttribute"/> class.
		/// </summary>
		public JoinedKeyAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JoinedKeyAttribute"/> class.
		/// </summary>
		/// <param name="column">The column.</param>
		public JoinedKeyAttribute(String column)
		{
			_column = column;
		}

		/// <summary>
		/// Gets or sets the column name
		/// </summary>
		/// <value>The column.</value>
		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}
	}
}
