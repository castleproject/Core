// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


namespace Castle.ActiveRecord.Linq
{
    using System;
    using System.Linq;
    using NHibernate;

    /// <summary>
    /// Class to provide a static mechanism for using active record classes in
    /// Linq expressions. This approach is less visually elegant than the 
    /// ActiveRecordLinqBase's Table property, but has the advantage of working
    /// on classes that are descended from ActiveRecordBase.
    /// </summary>
    public static class ActiveRecordLinq
    {
        /// <summary>
        /// AsQueryable enables you to use an active record class in a Linq expression even
        /// though the base class does not provide a static Table property.
        /// 
        /// Examples include:
        /// var items = from f in ActiveRecordLinq.AsQueryable&lt;foo&gt;() select f;
        /// var item = ActiveRecordLinq.AsQueryable&lt;foo&gt;().First();
        /// var items = from f in ActiveRecordLinq.AsQueryable&lt;foo&gt;() where f.Name == theName select f;
        /// var item = ActiveRecordLinq.AsQueryable&lt;foo&gt;().First(f => f.Name == theName);
        /// </summary>
        public static IOrderedQueryable<T> AsQueryable<T>()
        {
            return (IOrderedQueryable<T>)ActiveRecordMediator.ExecuteQuery(new LinqQuery<T>());
        }

        /// <summary>
        /// Extension method to ISession which creates a source for a Linq expression.
        /// </summary>
        public static IOrderedQueryable<T> AsQueryable<T>(this ISession session)
        {
            return new LinqQuery<T>().Execute(session);
        }

        /// <summary>
        /// Extension method to ISessionScope which creates a source for a Linq expression.
        /// </summary>
        public static IOrderedQueryable<T> AsQueryable<T>(this ISessionScope scope)
        {
            return (IOrderedQueryable<T>)ActiveRecordMediator.ExecuteQuery(new LinqQuery<T>());
        }

    }
}
