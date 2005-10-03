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

namespace Castle.Facilities.NHibernateExtension
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Collection;
	using NHibernate.Expression;
	using NHibernate.Proxy;

	using Castle.Facilities.NHibernateExtension.Util;

	/// <summary>
	/// Summary description for GenericDao.
	/// </summary>
	[UsesAutomaticSessionCreation]
	public class NHibernateGenericDao : INHibernateGenericDao
	{
		public NHibernateGenericDao() {}

		#region IGenericDAO Members
		public virtual Array FindAll(Type type)
		{
			return FindAll(type,int.MinValue,int.MinValue);
		}

		public virtual Array FindAll(Type type, int firstRow, int maxRows)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				ICriteria criteria = session.CreateCriteria(type);

				if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
				if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
				IList result = criteria.List();
				if (result == null || result.Count == 0) return null;
		
				Array array = Array.CreateInstance(type, result.Count);
				result.CopyTo(array,0);
				
				return array;
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform FindAll for " + type.Name, ex);
			}
		}

		public virtual object FindById(Type type, object id)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				return session.Load( type, id );
			}
			catch(ObjectNotFoundException)
			{
				return null;
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform FindByPrimaryKey for " + type.Name, ex);
			}
		}

		public virtual object Create(object instance)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				return session.Save(instance);
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Create for " + instance.GetType().Name, ex);
			}
		}

		public virtual void Delete(object instance)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				session.Delete(instance);
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Delete for " + instance.GetType().Name, ex);
			}
		}

		public virtual void Update(object instance)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				session.Update(instance);
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
			}
		}

		public virtual void DeleteAll(Type type)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				session.Delete(String.Format("from {0}", type.Name));
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform DeleteAll for " + type.Name, ex);
			}
		}

		public virtual void Save(object instance)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				session.SaveOrUpdate(instance);
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Save for " + instance.GetType().Name, ex);
			}
		}

		#endregion

		#region INHibernateGenericDAO Members
		public virtual Array FindAll(Type type, ICriterion[] criterias)
		{
			return FindAll(type,criterias,null,int.MinValue,int.MinValue);
		}

		public virtual Array FindAll(Type type, ICriterion[] criterias, int firstRow, int maxRows)
		{
			return FindAll(type,criterias,null,firstRow,maxRows);
		}

		public virtual Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems)
		{
			return FindAll(type,criterias,sortItems,int.MinValue,int.MinValue);
		}

		public virtual Array FindAll(Type type, ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows)
		{
			ISession session = SessionManager.CurrentSession;

			try
			{
				ICriteria criteria = session.CreateCriteria(type);

				if (criterias != null)
				{
					foreach( ICriterion cond in criterias )
						criteria.Add( cond );
				}

				if (sortItems != null)
				{
					foreach( Order order in sortItems )
						criteria.AddOrder( order );
				}

				if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
				if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
				IList result = criteria.List();
				if (result == null || result.Count == 0) return null;
		
				Array array = Array.CreateInstance(type, result.Count);
				result.CopyTo(array,0);
				
				return array;
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform FindAll for " + type.Name, ex);
			}
		}
		public virtual Array FindAllWithCustomQuery(string queryString)
		{
			return FindAllWithCustomQuery(queryString,int.MinValue,int.MinValue);
		}

		public virtual Array FindAllWithCustomQuery(string queryString, int firstRow, int maxRows)
		{
			if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");

			ISession session = SessionManager.CurrentSession;

			try
			{
				IQuery query = session.CreateQuery(queryString);

				if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
				if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
				IList result = query.List();
				if (result == null || result.Count == 0) return null;

				Array array = Array.CreateInstance(result[0].GetType(), result.Count);
				result.CopyTo(array,0);
		
				return array;
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Find for custom query : " + queryString, ex);
			}
		}


		public virtual Array FindAllWithNamedQuery(string namedQuery)
		{
			return FindAllWithNamedQuery(namedQuery,int.MinValue,int.MinValue);
		}

		public virtual Array FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
		{
			if (namedQuery == null || namedQuery.Length == 0) throw new ArgumentNullException("queryString");

			ISession session = SessionManager.CurrentSession;

			try
			{
				IQuery query = session.GetNamedQuery(namedQuery);
				if (query == null) throw new ArgumentException("Cannot find named query","namedQuery");

				if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
				if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
				IList result = query.List();
				if (result == null || result.Count == 0) return null;

				Array array = Array.CreateInstance(result[0].GetType(), result.Count);
				result.CopyTo(array,0);
		
				return array;
			}
			catch(Exception ex)
			{
				throw new DataException("Could not perform Find for named query : " + namedQuery, ex);
			}
		}

		public void InitializeLazyProperties(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			
			ISession session = SessionManager.CurrentSession;
			
			foreach (object val in ReflectionUtil.GetPropertiesDictionary(instance).Values)
			{
				if(val is INHibernateProxy || val is PersistentCollection)
				{
					if (!NHibernateUtil.IsInitialized(val))
					{
						session.Lock(instance, LockMode.None);
						NHibernateUtil.Initialize(val);
					}
				}
			}
		}

		public void InitializeLazyProperty(object instance, string propertyName)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (propertyName == null || propertyName.Length == 0) throw new ArgumentNullException("collectionPropertyName");

			IDictionary properties = ReflectionUtil.GetPropertiesDictionary(instance);
			if (! properties.Contains(propertyName))
				throw new ArgumentOutOfRangeException("collectionPropertyName","Property "
							+ propertyName + " doest not exist for type "
							+ instance.GetType().ToString() + ".");
			
			ISession session = SessionManager.CurrentSession;

			object val = properties[propertyName];
			if(val is INHibernateProxy || val is PersistentCollection)
			{
				if (!NHibernateUtil.IsInitialized(val))
				{
					session.Lock(instance, LockMode.None);
					NHibernateUtil.Initialize(val);
				}
			}
		}
		#endregion
	}
}
