// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace ARProject
{
	using System;
	using Castle.ActiveRecord;
	using NHibernate.Expression;

	// This is file is provided just as a starting point 
	// especially if you are new to ActiveRecord
	//
	// Feel free to delete it right away.

	[ActiveRecord("TableName")]
	public class Sample : ActiveRecordBase
	{
		private int id;
		private String name;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		/// <summary>
		/// FindByPrimaryKey will thrown <c>NotFoundException</c>
		/// if the given primary key is not found on the table.
		/// </summary>
		/// <param name="id">Id to search for</param>
		/// <returns>The instance represented by the PK</returns>
		public static Sample Find(int id)
		{
			return (Sample) FindByPrimaryKey(typeof(Sample), id);
		}

		/// <summary>
		/// FindAll accepts ordering and criterias
		/// </summary>
		/// <returns>An array of instances, ordered by Name</returns>
		/// <remarks>
		/// Criterias use the property name, not the column name
		/// </remarks>
		public static Sample[] FindAll()
		{
			return (Sample[]) FindAll(typeof(Sample), new Order[] { Order.Asc("Name") });
		}

		/// <summary>
		/// FindOne assumes that none or only one 
		/// record exist for the given criteria
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <returns>The existing instance or null</returns>
		/// <remarks>
		/// Criterias use the property name, not the column name
		/// </remarks>
		public static Sample FindByName(String name)
		{
			return (Sample) FindOne(typeof(Sample), Expression.Eq("Name", name));
		}
	}
}
