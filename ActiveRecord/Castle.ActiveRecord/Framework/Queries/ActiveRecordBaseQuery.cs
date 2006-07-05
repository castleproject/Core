// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Queries;
	using Castle.ActiveRecord.Queries.Modifiers;

	using NHibernate;

	using Castle.ActiveRecord.Framework;
	
	/// <summary>
	/// Base class for all ActiveRecord queries.
	/// </summary>
	public abstract class ActiveRecordBaseQuery : IActiveRecordQuery, ICloneable
	{
		private readonly Type targetType;
		
		private IList modifiers;

		public ActiveRecordBaseQuery(Type type)
		{
			this.targetType = type;
		}

		public Type Target
		{
			get { return targetType; }
		}

		#region IActiveRecordQuery implementation
		object IActiveRecordQuery.Execute(ISession session)
		{
			return InternalExecute(session);
		}

		IEnumerable IActiveRecordQuery.Enumerate(ISession session)
		{
			return InternalEnumerate(session);
		}
		#endregion

		/// <summary>
		/// Simply creates the query and then call its <see cref="IQuery.List"/> method.
		/// </summary>
		/// <param name="session">The <c>NHibernate</c>'s <see cref="ISession"/></param>
		protected virtual object InternalExecute(ISession session)
		{
			return CreateQuery(session).List();
		}

		/// <summary>
		/// Simply creates the query and then call its <see cref="IQuery.Enumerable"/> method.
		/// </summary>
		/// <param name="session">The <c>NHibernate</c>'s <see cref="ISession"/></param>
		protected virtual IEnumerable InternalEnumerate(ISession session)
		{
			return CreateQuery(session).Enumerable();
		}

		/// <summary>
		/// Creates the <see cref="IQuery" /> instance.
		/// </summary>
		protected abstract IQuery CreateQuery(ISession session);

		/// <summary>
		/// Just a default clone implementation...
		/// </summary>
		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Adds a query modifier, to be applied with <see cref="ApplyModifiers"/>.
		/// </summary>
		/// <param name="modifier">The modifier</param>
		protected void AddModifier(IQueryModifier modifier)
		{
			if (modifiers == null)
				modifiers = new ArrayList();

			modifiers.Add(modifier);
		}
		
		/// <summary>
		/// Applies the modifiers added with <see cref="AddModifier"/>.
		/// </summary>
		/// <param name="query">The query in which to apply the modifiers</param>
		/// <remarks>
		/// This method is not called automatically 
		/// by <see cref="ActiveRecordBaseQuery"/>, but is called from
		/// <see cref="HqlBasedQuery"/>.
		/// </remarks>
		protected void ApplyModifiers(IQuery query)
		{
			if (modifiers != null)
				foreach (IQueryModifier modifier in modifiers)
					modifier.Apply(query);
		}
		
		/// <summary>
		/// Converts the results stored in an <see cref="IList"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="t">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		[Obsolete("Use SupportingUtils.BuildArray directly")]
		protected Array GetResultsArray(Type t, IList list, bool distinct)
		{
			return SupportingUtils.BuildArray(t, list, distinct);
		}

		/// <summary>
		/// Converts the results stored in an <see cref="IList"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="t">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="entityIndex">
		/// If the HQL clause selects more than one field, or a join is performed
		/// without using <c>fetch join</c>, the contents of the result list will
		/// be of type <c>object[]</c>. Specify which index in this array should be used to
		/// compose the new result array. Use <c>-1</c> to ignore this parameter.
		/// </param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		[Obsolete("Use SupportingUtils.BuildArray directly")]
		protected Array GetResultsArray(Type t, IList list, int entityIndex, bool distinct)
		{
			return SupportingUtils.BuildArray(t, list, entityIndex, distinct);
		}
	}
}
