using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using System.Data;
using NHibernate.Expression;

namespace NShop.Repositories
{
    public interface IRepository<T>
	where T : class
    {
        T Load(int id);
        T Get(int id);
		ICollection<T> Find(params ICriterion[] predicates);
        void Save(T item);
        void Delete(T item);
    }
}
