using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;
using NShop.Services;

namespace NShop.Repositories
{
    public class NHibernateRepository<T> : IRepository<T>
	where T :class
    {
        private readonly ISessionProvider sessionProvider;

        public NHibernateRepository(ISessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;
        }
        
        public T Load(int id)
        {
			return sessionProvider.Session.Load(typeof (T), id) as T;
        }

        public T Get(int id)
        {
			return sessionProvider.Session.Get(typeof(T), id) as T;
        }

        public virtual ICollection<T> Find(params ICriterion[] predicates) 
        {
        	ICriteria criteria = sessionProvider.Session.CreateCriteria(typeof(T));
			foreach (ICriterion predicate in predicates)
        	{
				criteria.Add(predicate);
        	}
			return ToGenericCollection(criteria);
        }

    	
    	public virtual void Save(T item)
        {
            sessionProvider.Session.Save(item);
        }
    	
        public virtual void Delete(T item) 
        {
			sessionProvider.Session.Delete(item);
        }

		private static IList<T> ToGenericCollection(ICriteria criteria)
		{
			List<T> list = new List<T>();
			foreach (T t in criteria.List())
			{
				list.Add(t);
			}
			return list;
		}

    }
}

