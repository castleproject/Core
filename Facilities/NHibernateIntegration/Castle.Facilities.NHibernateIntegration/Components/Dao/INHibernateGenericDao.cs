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
	using NHibernate.Criterion;

	/// <summary>
	/// Summary description for INHibernateGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie &lt;steve.degosserie@vn.netika.com&gt;
	/// </remarks>
	public interface INHibernateGenericDao : IGenericDao
	{
		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		Array FindAll(Type type, ICriterion[] criterias);

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		Array FindAll(Type type, ICriterion[] criterias, int firstRow, int maxRows);

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="sortItems">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems);

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criteria.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="criterias">The criteria expression</param>
		/// <param name="sortItems">An <see cref="Array"/> of <see cref="Order"/> objects.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns>The <see cref="Array"/> of results.</returns>
		Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows);

		/// <summary>
		/// Finds all with custom query.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <returns></returns>
		Array FindAllWithCustomQuery(string queryString);

		/// <summary>
		/// Finds all with custom HQL query.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns></returns>
		Array FindAllWithCustomQuery(string queryString, int firstRow, int maxRows);

		/// <summary>
		/// Finds all with named HQL query.
		/// </summary>
		/// <param name="namedQuery">The named query.</param>
		/// <returns></returns>
		Array FindAllWithNamedQuery(string namedQuery);

		/// <summary>
		/// Finds all with named HQL query.
		/// </summary>
		/// <param name="namedQuery">The named query.</param>
		/// <param name="firstRow">The number of the first row to retrieve.</param>
		/// <param name="maxRows">The maximum number of results retrieved.</param>
		/// <returns></returns>
		Array FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);

		/// <summary>
		/// Initializes the lazy properties.
		/// </summary>
		/// <param name="instance">The instance.</param>
		void InitializeLazyProperties(object instance);

		/// <summary>
		/// Initializes the lazy property.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="propertyName">Name of the property.</param>
		void InitializeLazyProperty(object instance, string propertyName);
	}
}