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

namespace Castle.ActiveRecord.Queries.Modifiers
{
	using System;

	using NHibernate;
	using NHibernate.Transform;

	/// <summary>
	/// Defines a query result transformation.
	/// See <see cref="IResultTransformer"/> for more information.
	/// </summary>
	public class QueryResultTransformer : IQueryModifier
	{
		private readonly IResultTransformer transformer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResultTransformer"/> class.
		/// </summary>
		/// <param name="transformer">The result transformer.</param>
		public QueryResultTransformer(IResultTransformer transformer)
		{
			if (transformer == null) throw new ArgumentNullException("transformer");

			this.transformer = transformer;
		}

		/// <summary>
		/// Gets the <see cref="IResultTransformer"/>.
		/// </summary>
		public IResultTransformer ResultTransformer
		{
			get { return transformer; }
		}

		#region "Apply" method

		/// <summary>
		/// Applies this modifier to the query.
		/// </summary>
		/// <param name="query">The query</param>
		void IQueryModifier.Apply(IQuery query)
		{
			query.SetResultTransformer(transformer);
		}
		
		#endregion
	}
}
