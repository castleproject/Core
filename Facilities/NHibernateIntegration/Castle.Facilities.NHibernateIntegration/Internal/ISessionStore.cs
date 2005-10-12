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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using NHibernate;

	/// <summary>
	/// Provides the contract for implementors who want to 
	/// store valid session so they can be reused in a invocation
	/// chain.
	/// </summary>
	public interface ISessionStore
	{
		/// <summary>
		/// Should return a previously stored session 
		/// for the given alias if available, otherwise null.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		ISession FindCompatibleSession(String alias);

		/// <summary>
		/// Should store the specified session instance 
		/// and return a cookie which will be used to remove
		/// the session from the store.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="session"></param>
		/// <returns>Returns a cookie</returns>
		object Store(String alias, ISession session);

		/// <summary>
		/// Should remove the session from the store 
		/// only.
		/// </summary>
		/// <param name="cookie"></param>
		/// <param name="session"></param>
		void Remove(object cookie, ISession session);
	}
}
