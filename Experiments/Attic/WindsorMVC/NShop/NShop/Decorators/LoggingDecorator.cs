using System;
using System.Collections.Generic;
using log4net;
using NHibernate.Expression;
using log4net.Core;
using NShop.Repositories;

namespace NShop.Decorators
{
	public class LoggingDecorator<T> : IRepository<T>
	where T: class
	{
		private readonly IRepository<T> inner;
		ILog logger = LogManager.GetLogger(typeof(IRepository<T>));
		public LoggingDecorator()
		{

		}
		public LoggingDecorator(IRepository<T> inner)
		{
			this.inner = inner;
		}

		public T Load(int id)
		{
			logger.InfoFormat("Loading {0} with #{1}", typeof (T), id);
			return inner.Load(id);
		}

		public T Get(int id)
		{
			logger.InfoFormat("Getting {0} with #{1}", typeof(T), id);
			return inner.Get(id);
		}

		public ICollection<T> Find(params ICriterion[] predicates)
		{
			Conjunction conjunction = new Conjunction();
			foreach (ICriterion predicate in predicates)
			{
				conjunction.Add(predicate);
			}
			logger.InfoFormat("Finding {0} with query {1}", typeof(T), conjunction);
			return inner.Find(predicates);
		}

		public void Save(T item)
		{
			logger.InfoFormat("Saving {0} with #{1}", typeof (T));
			inner.Save(item);
		}

		public void Delete(T item)
		{
			logger.InfoFormat("Deleting {0}", typeof (T));
			inner.Delete(item);
		}
	}
}