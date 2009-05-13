// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	/// This is used to define a named SQL query.
	/// It represents the &lt;query&gt; element.
	/// </summary>
	/// <example>
	/// [assembly: SqlNamedQuery("allAdultUsers", "select * from User where Age > 21")]
	/// </example>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true), Serializable]
	public class SqlNamedQueryAttribute : Attribute
	{
		private string name;
		private string query;

		/// <summary>
		/// The name of the query
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// The query itself
		/// </summary>
		public string Query
		{
			get { return query; }
		}

		/// <summary>
		/// Create a new instance
		/// </summary>
		public SqlNamedQueryAttribute(string name, string query)
		{
			this.name = name;
			this.query = query;
		}
	}
}
