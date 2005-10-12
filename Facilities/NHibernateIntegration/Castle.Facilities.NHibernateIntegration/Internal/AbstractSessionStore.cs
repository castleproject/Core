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

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using System;
	using System.Collections;

	using NHibernate;

	public abstract class AbstractSessionStore : MarshalByRefObject, ISessionStore
	{
		protected abstract Stack GetStackFor(String alias);

		public ISession FindCompatibleSession(String alias)
		{
			Stack stack = GetStackFor(alias);

			if (stack.Count == 0) return null;

			return stack.Peek() as ISession;
		}

		public object Store(String alias, ISession session)
		{
			Stack stack = GetStackFor(alias);

			stack.Push(session);

			return stack;
		}

		public void Remove(object cookie, ISession session)
		{
			Stack stack = (Stack) cookie;

			if (stack.Count == 0)
			{
				throw new InvalidProgramException("ThreadLocalSessionStore.Remove called " + 
					"for an empty stack");
			}

			ISession current = stack.Peek() as ISession;

			if (session != current)
			{
				throw new InvalidProgramException("ThreadLocalSessionStore.Remove tried to " + 
					"remove a session which is not on the top or not in the stack at all");
			}

			stack.Pop();
		}
	}
}
