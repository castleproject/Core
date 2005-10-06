#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Expression;

namespace Castle.ActiveRecord
{
	public static class ActiveRecord<T> where T : ActiveRecordBase
	{
		public static T Find(object id)
		{
			return (T)ActiveRecordBase.FindByPrimaryKey(typeof(T), id, true);
		}

		public static T TryFind(object id)
		{
			return (T)ActiveRecordBase.FindByPrimaryKey(typeof(T), id, false);
		}
		
		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(T));
		}

		public static T[] FindAll()
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T));
		}

		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T), criterias);
		}

		public static T[] FindAll(ICriterion criteria, int firstResult, int maxresults)
		{
			return FindAll(new ICriterion[] { criteria }, firstResult, maxresults);
		}

		public static T[] FindAll(ICriterion[] criterias,int firstResult, int maxresults)
		{
			return (T[])ActiveRecordBase.SlicedFindAll(typeof(T), firstResult, maxresults, criterias);
		}

		public static T[] FindAll(ICriterion[] criterias, Order[] orders)
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T), orders, criterias);
		}

		public static T[] FindAll( ICriterion[] criterias, Order[] orders,int firstResult, int maxresults)
		{
			return (T[]) ActiveRecordBase.SlicedFindAll(typeof(T),firstResult, maxresults,orders,criterias);
		}
	}
}
#endif