using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Expression;
using NShop.Repositories;
using NShop.Services;

namespace NShop.Decorators
{
	public class TranactionDecorator<T> : IRepository<T>
	where T: class
	{
		private readonly IsolationLevel isolationLevel = IsolationLevel.ReadCommitted;
		private readonly IRepository<T> inner;
		private readonly ISessionProvider sessionProvider;

		public TranactionDecorator()
		{

		}
		public TranactionDecorator(IRepository<T> inner,
								   ISessionProvider sessionProvider,
								   IsolationLevel isolationLevel)
		{
			this.inner = inner;
			this.sessionProvider = sessionProvider;
			this.isolationLevel = isolationLevel;
		}

		public T Load(int id)
		{
			return inner.Load(id);
		}

		public T Get(int id)
		{
			return inner.Get(id);
		}

		public ICollection<T> Find(params ICriterion[] predicates)
		{
			return inner.Find(predicates);
		}

		public void Save(T item)
		{
			using (ITransaction tx = sessionProvider.Session.BeginTransaction(isolationLevel))
			{
				inner.Save(item);
				tx.Commit();
			}
		}

		public void Delete(T item)
		{
			using (ITransaction tx = sessionProvider.Session.BeginTransaction(isolationLevel))
			{
				inner.Delete(item);
				tx.Commit();
			}
		}
	}
}