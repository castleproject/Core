using System;
using System.Collections.Generic;
using System.Security;
using NHibernate.Expression;
using NShop.Repositories;
using NShop.Services;

namespace NShop.Decorators
{
	public class SecurityDecorator<T> : IRepository<T>
	where T : class
	{
		private readonly ISecurityInformation security;
		private readonly IRepository<T> inner;

		public SecurityDecorator()
		{

		}
		public SecurityDecorator(IRepository<T> inner, 
		                         ISecurityInformation security)
		{
			this.inner = inner;
			this.security = security;
		}

		public T Load(int id)
		{
			AssertHasAutorization("load");
			return inner.Load(id);
		}
		
		public T Get(int id)
		{
			AssertHasAutorization("get");
			return inner.Get(id);
		}

		public ICollection<T> Find(params ICriterion[] predicates)
		{
			AssertHasAutorization("find");
			return inner.Find(predicates);
		}

		public void Save(T item)
		{
			AssertHasAutorization("save");
			inner.Save(item);
		}

		public void Delete(T item)
		{
			AssertHasAutorization("delete");
			inner.Delete(item);
		}

		private void AssertHasAutorization(string action)
		{
			if (security.HasAuthorizationFor(action) == false)
				throw new SecurityException("You can't do this, dude");
		}
	}
}