// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Queries
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.ActiveRecord.Framework;
	using NHibernate;
	using NHibernate.Criterion;
	using NHibernate.Transform;

	/// <summary>
	/// Performs a projected selection from an entity, lifting only the required fields.
	/// Similar to SELECT Id,Name FROM MyTable instead of selecting everything.
	/// It is possible to combine this with grouping. 
	/// </summary>
	/// <typeparam name="ARType">The active record entity type</typeparam>
	/// <typeparam name="TResultItem">The result value to use: object[] means returning as is</typeparam>
	/// /// <example>
	/// <code>
	/// <![CDATA[
	/// ProjectionQuery<Post, PostTitleAndId> proj = new ProjectionQuery<Post, PostTitleAndId>(Projections.Property("Title"), Projections.Property("Id"));
	/// ICollection<PostTitleAndId> posts = proj.Execute();
	/// foreach(PostTitleAndId titleAndId in posts)
	/// {
	///		//push to site...
	/// }
	/// ]]>
	/// </code>
	/// </example>
	public class ProjectionQuery<ARType, TResultItem> : IActiveRecordQuery
	{
		private readonly ProjectionList projections;
		private readonly DetachedCriteria detachedCriteria;
		private readonly Order[] orders;
		private int? first, max;

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given
		/// </summary>
		/// <param name="projections">The projections to use in the query</param>
		public ProjectionQuery(ProjectionList projections)
		{
			this.projections = projections;
			orders = new Order[0];
			detachedCriteria = DetachedCriteria.For(RootType);
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="orders">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order[] orders, ProjectionList projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			this.orders = orders;
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="order">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, ProjectionList projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			orders = new Order[] { order };
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The results will be loaded according to the order specified
		/// </summary>
		public ProjectionQuery(Order order, ProjectionList projections)
		{
			this.projections = projections;
			detachedCriteria = DetachedCriteria.For(RootType);
			orders = new Order[] { order };
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		public ProjectionQuery(DetachedCriteria detachedCriteria, ProjectionList projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			orders = new Order[0];
		}

		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		public Type RootType
		{
			get { return typeof(ARType); }
		}

		/// <summary>
		/// Sets the query range.
		/// </summary>
		/// <param name="first">The first row to return.</param>
		/// <param name="max">The max number of rows to return.</param>
		/// <returns>The instance</returns>
		public ProjectionQuery<ARType, TResultItem> SetRange(int first, int max)
		{
			this.first = first;
			this.max = max;
			
			return this;
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
        /// <returns>IList&lt;TResultItem&gt; cast to object because of interface</returns>
		object IActiveRecordQuery.Execute(ISession session)
		{
			ICriteria criteria = CreateCriteria(session);

			if (first.HasValue)
			{
				criteria.SetFirstResult(first.Value);
				criteria.SetMaxResults(max.Value);
			}

			return criteria.List<TResultItem>();
		}

		/// <summary>
		/// Enumerates over the result of the query.
		/// Note: Only use if you expect most of your values to already exist in the second level cache!
		/// </summary>
		public IEnumerable Enumerate(ISession session)
		{
			return Execute();
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <returns>the result of the query</returns>
		public IList<TResultItem> Execute()
		{
			return (IList<TResultItem>) ActiveRecordMediator.ExecuteQuery(this);
		}

		private ICriteria CreateCriteria(ISession session)
		{
			AssertAllArgumentsValid();
			ICriteria criteria = detachedCriteria.GetExecutableCriteria(session);
			criteria.SetProjection(projections);

			// we are not returning a tuple, so we need the result transformer
			if (!typeof(TResultItem).IsPrimitive && typeof(TResultItem) != typeof(object[]))
			{
				criteria.SetResultTransformer(new TypedResultTransformer<TResultItem>());
			}

			CriteriaHelper.AddOrdersToCriteria(criteria, orders);

			return criteria;
		}

		private void AssertAllArgumentsValid()
		{
			if (projections == null)
			{
				throw new ArgumentNullException("projections");
			}
			if (orders == null)
			{
				throw new ArgumentNullException("orders");
			}
			if (detachedCriteria == null)
			{
				throw new ArgumentNullException("detachedCriteria");
			}
			if (projections.Length == 0)
			{
				throw new ActiveRecordException("Can't use projection query with zero projections!");
			}
		}

		/// <summary>
		/// This is used to convert the resulting tuples into strongly typed objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class TypedResultTransformer<T> : IResultTransformer
		{
			///<summary>
			///Convert the tuples into a strongly typed object
			///</summary>
			public object TransformTuple(object[] tuple, string[] aliases)
			{
				if (tuple.Length == 1 && typeof(T).IsAssignableFrom(tuple[0].GetType()))
					return tuple[0];
				return Activator.CreateInstance(typeof(T), tuple);
			}

			public IList TransformList(IList collection)
			{
				return collection;
			}
		}
	}

	/// <summary>
	/// Default implemenation of ProjectionQuery that returns an Untyped object array tuples
	/// </summary>
	public class ProjectionQuery<ARType> : ProjectionQuery<ARType, object[]>
	{
		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given
		/// </summary>
		/// <param name="projections">The projections to use in the query</param>
		public ProjectionQuery(ProjectionList projections) : base(projections)
		{
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="orders">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order[] orders, ProjectionList projections)
			: base(detachedCriteria, orders,projections)
		{
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="order">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, ProjectionList projections)
			: base(detachedCriteria,order, projections)
		{
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The results will be loaded according to the order specified
		/// </summary>
		public ProjectionQuery(Order order, ProjectionList projections) : base(order, projections)
		{
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		public ProjectionQuery(DetachedCriteria detachedCriteria, ProjectionList projections)
			: base(detachedCriteria, projections)
		{
		}
	}
}
