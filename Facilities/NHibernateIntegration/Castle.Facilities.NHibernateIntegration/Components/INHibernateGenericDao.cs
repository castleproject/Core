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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

#if DOTNET2
	using System.Collections.Generic;
#endif

    using NHibernate.Expression;

	/// <summary>
	/// Summary description for INHibernateGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
	/// Ported to .net2 by Ernst Naezer <ernst@auxilium.nl>	
    /// </remarks>
#if DOTNET2
	public interface INHibernateGenericDao<T> : IGenericDao<T>
	{
		T[] FindAll(ICriterion[] criterias);

		T[] FindAll(ICriterion[] criterias, int firstRow, int maxRows);

		T[] FindAll(ICriterion[] criterias, Order[] sortItems);

		T[] FindAll(ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows);

		T[] FindAllWithCustomQuery(string queryString);

		T[] FindAllWithCustomQuery(string queryString, int firstRow, int maxRows);

		T[] FindAllWithNamedQuery(string namedQuery);

		T[] FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);

		void InitializeLazyProperties(object instance);

		void InitializeLazyProperty(object instance, string propertyName);
	}

#else
        	
    /// <summary>
	/// Summary description for INHibernateGenericDao.
	/// </summary>
	/// <remarks>
	/// Contributed by Steve Degosserie <steve.degosserie@vn.netika.com>
	/// </remarks>
    public interface INHibernateGenericDao : IGenericDao
	{
		Array FindAll(Type type, ICriterion[] criterias);
		
		Array FindAll(Type type, ICriterion[] criterias, int firstRow, int maxRows);
		
		Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems);
		
		Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows);
		
		Array FindAllWithCustomQuery(string queryString);
		
		Array FindAllWithCustomQuery(string queryString, int firstRow, int maxRows);
		
		Array FindAllWithNamedQuery(string namedQuery);
		
		Array FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);
		
		void InitializeLazyProperties(object instance);
		
		void InitializeLazyProperty(object instance, string propertyName);
	}
#endif
}
