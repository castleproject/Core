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

namespace Castle.Facilities.NHibernateIntegration.SessionStores
{
	using System;
	using System.Collections;
	using NHibernate;

	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractSessionStore : MarshalByRefObject, ISessionStore
	{
		/// <summary>
		/// Gets the stack of <see cref="SessionDelegate"/> objects for the specified <paramref name="alias"/>.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		protected abstract Stack GetStackFor(String alias);

		/// <summary>
		/// Should return a previously stored session
		/// for the given alias if available, otherwise null.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public SessionDelegate FindCompatibleSession(String alias)
		{
			Stack stack = this.GetStackFor(alias);

			if (stack.Count == 0) return null;

			return stack.Peek() as SessionDelegate;
		}

		/// <summary>
		/// Should store the specified session instance
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="session"></param>
		public void Store(String alias, SessionDelegate session)
		{
			Stack stack = this.GetStackFor(alias);

			stack.Push(session);

			session.SessionStoreCookie = stack;
		}

		/// <summary>
		/// Should remove the session from the store
		/// only.
		/// </summary>
		/// <param name="session"></param>
		public void Remove(SessionDelegate session)
		{
			Stack stack = (Stack) session.SessionStoreCookie;

			if (stack == null)
			{
				throw new InvalidProgramException("AbstractSessionStore.Remove called " + 
				                                  "with no cookie - no pun intended");
			}

			if (stack.Count == 0)
			{
				throw new InvalidProgramException("AbstractSessionStore.Remove called " + 
				                                  "for an empty stack");
			}

			ISession current = stack.Peek() as ISession;

			if (session != current)
			{
				throw new InvalidProgramException("AbstractSessionStore.Remove tried to " + 
				                                  "remove a session which is not on the top or not in the stack at all");
			}

			stack.Pop();
		}

		/// <summary>
		/// Returns <c>true</c> if the current activity
		/// (which is an execution activity context) has no
		/// sessions available
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public bool IsCurrentActivityEmptyFor(String alias)
		{
			Stack stack = this.GetStackFor(alias);

			return stack.Count == 0;
		}
	}
}