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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Scopes;
	using NHibernate;

	/// <summary>
	/// Implementation of <see cref="ISessionScope"/> with
	/// an IStatelessSession to improve performance 
	/// by caching a session without a first-level-cache.
	/// </summary>
	public class StatelessSessionScope : SessionScope
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StatelessSessionScope"/> class.
		/// </summary>
		public StatelessSessionScope() : base(FlushAction.Never, SessionScopeType.Simple)
		{
		}

		/// <summary>
		/// If the <see cref="AbstractScope.WantsToCreateTheSession"/> returned
		/// <c>true</c> then this method is invoked to allow
		/// the scope to create a properly configured session
		/// </summary>
		/// <param name="sessionFactory">From where to open the session</param>
		/// <param name="interceptor">the NHibernate interceptor</param>
		/// <returns>the newly created session</returns>
		public override ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			return new StatelessSessionWrapper(sessionFactory.OpenStatelessSession());
		}
	}
}
