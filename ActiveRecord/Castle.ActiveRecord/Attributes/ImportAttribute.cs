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
	/// This is used to map between a type to a friendly name that can be used in the queries.
	/// 
	/// This attribute is representing an &lt;import/&gt; in the mapping files
	/// </summary>
	/// <example>
	/// [Import(typeof(SummaryRow), "summary")]
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true), Serializable]
	public class ImportAttribute : Attribute
	{
		private readonly Type type;
		private String rename;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportAttribute"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="rename">The rename.</param>
		public ImportAttribute(Type type, string rename)
		{
			this.type = type;
			this.rename = rename;
		}

		/// <summary>
		/// Gets the type that is being imported
		/// </summary>
		/// <value>The type.</value>
		public Type Type
		{
			get { return this.type; }
		}

		/// <summary>
		/// Gets or sets the renamed string that will replace the full type name in HQL queries for the specified type.
		/// </summary>
		/// <value>The renamed value.</value>
		public String Rename
		{
			get { return rename; }
			set { rename = value; }
		}
	}
}