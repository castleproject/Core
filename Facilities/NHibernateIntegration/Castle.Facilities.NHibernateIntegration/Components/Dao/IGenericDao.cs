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

namespace Castle.Facilities.NHibernateIntegration.Components.Dao
{
	using System;

	/// <summary>
	/// Summary description for IGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie &lt;steve.degosserie@vn.netika.com&gt;
	/// </remarks>
	public interface IGenericDao
	{
		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <returns>The <see cref="Array"/> of results</returns>
		Array FindAll(Type type);

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results</returns>
		Array FindAll(Type type, int firstRow, int maxRows);

		/// <summary>
		/// Finds an object instance by an unique ID
		/// </summary>
		/// <param name="type">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns>The object instance.</returns>
		object FindById(Type type, object id);

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance">The instance to be created on the database</param>
		/// <returns>The instance</returns>
		object Create(object instance);

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance">The instance to be updated on the database</param>
		void Update(object instance);

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance">The instance to be deleted from the database</param>
		void Delete(object instance);

		/// <summary>
		/// Deletes all rows for the specified type
		/// </summary>
		/// <param name="type">type on which the rows on the database should be deleted</param>
		void DeleteAll(Type type);

		/// <summary>
		/// Saves the instance to the database. If the primary key is unitialized
		/// it creates the instance on the database. Otherwise it updates it.
		/// <para>
		/// If the primary key is assigned, then you must invoke <see cref="Create"/>
		/// or <see cref="Update"/> instead.
		/// </para>
		/// </summary>
		/// <param name="instance">The instance to be saved</param>
		void Save(object instance);
	}
}