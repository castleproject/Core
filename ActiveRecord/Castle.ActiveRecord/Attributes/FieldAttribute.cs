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
	/// Maps a standard column of the table.
	/// </summary>
	/// <example>
	/// In the following example, the column is also
	/// called 'name', so you don't have to specify.
	/// <code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		[Field]
	///		string name;
	///		
	///		
	///	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field), Serializable]
	public class FieldAttribute : PropertyAttribute
	{
		public FieldAttribute() 
		{
			Access = PropertyAccess.Field;
		}

		public FieldAttribute(String column) : base(column)
		{
			Access = PropertyAccess.Field;
		}

		public FieldAttribute(String column, String type) : base(column, type)
		{
			Access = PropertyAccess.Field;
		}
	}
}
