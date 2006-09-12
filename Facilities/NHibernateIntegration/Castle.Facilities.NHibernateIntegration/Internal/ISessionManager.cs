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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using NHibernate;

	/// <summary>
	/// Provides a bridge to NHibernate allowing the implementation
	/// to cache created session (through an invocation) and 
	/// enlist it on transaction if one is detected on the thread.
	/// </summary>
	public interface ISessionManager
	{
		/// <summary>
		/// The flushmode the created session gets
		/// </summary>
		FlushMode DefaultFlushMode { get;set;}
		
		/// <summary>
		/// Returns a valid opened and connected ISession instance
		/// </summary>
		/// <returns></returns>
		ISession OpenSession();

		/// <summary>
		/// Returns a valid opened and connected ISession instance
		/// for the given connection alias.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		ISession OpenSession(String alias);
	}
}
