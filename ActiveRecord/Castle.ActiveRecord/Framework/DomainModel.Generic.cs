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

#if dotNet2

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord.Framework;
	using NHibernate;
	using NHibernate.Expression;


	/// <summary>
	/// Generic static class to ease the access to Active Record features on .Net 2
	/// </summary>
	public static class DomainModel<T> where T : class
	{
		
		public static T Find(object id)
		{
			return (T)DomainModel.FindByPrimaryKey(typeof(T), id, true);
		}

		public static T TryFind(object id)
		{
			return (T)DomainModel.FindByPrimaryKey(typeof(T), id, false);
		}
		
		public static void DeleteAll()
		{
			DomainModel.DeleteAll(typeof(T));
		}

		public static T[] FindAll()
		{
			return (T[])DomainModel.FindAll(typeof(T));
		}

		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[])DomainModel.FindAll(typeof(T), criterias);
		}

		public static T[] FindAll(ICriterion criteria, int firstResult, int maxresults)
		{
			return FindAll(new ICriterion[] { criteria }, firstResult, maxresults);
		}

		public static T[] FindAll(ICriterion[] criterias,int firstResult, int maxresults)
		{
            return (T[])DomainModel.SlicedFindAll(typeof(T), firstResult, maxresults, criterias);
		}

		public static T[] FindAll(ICriterion[] criterias, Order[] orders)
		{
			return (T[])DomainModel.FindAll(typeof(T), orders, criterias);
		}

		public static T[] FindAll( ICriterion[] criterias, Order[] orders,int firstResult, int maxresults)
		{
			return (T[]) DomainModel.SlicedFindAll(typeof(T),firstResult, maxresults,orders,criterias);
		}

        public static void Save(T instance)
        {
            DomainModel.Save(instance);
        }

        public static void Create(T instance)
        {
            DomainModel.Create(instance);
        }

        public static void Update(T instance)
        {
            DomainModel.Update(instance);
        }

        public static void Delete(T instance)
        {
            DomainModel.Delete(instance);
        }

        public static void Execute(T instnace, NHibernateDelegate call)
        {
            DomainModel.Execute(typeof(T), call, instnace);
        }
    }
}
#endif 