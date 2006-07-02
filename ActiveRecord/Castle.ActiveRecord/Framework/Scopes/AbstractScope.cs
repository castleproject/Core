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

namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections;

	using NHibernate;

	/// <summary>
	/// Abstract <seealso cref="ISessionScope"/> implementation
	/// </summary>
	public abstract class AbstractScope : MarshalByRefObject, ISessionScope
	{
		private readonly SessionScopeType type;

		protected Hashtable key2Session = new Hashtable();

		public AbstractScope(SessionScopeType type)
		{
			this.type = type;
			ThreadScopeAccessor.Instance.RegisterScope(this);
		}

		public SessionScopeType ScopeType
		{
			get { return type; }
		}

		public virtual bool IsKeyKnown(object key)
		{
			return key2Session.Contains(key);
		}

		public virtual void RegisterSession(object key, ISession session)
		{
			key2Session.Add(key, session);

			Initialize(session);
		}

		public virtual ISession GetSession(object key)
		{
			return key2Session[key] as ISession;
		}

		public virtual bool WantsToCreateTheSession
		{
			get { return false; }
		}

		public virtual ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			ThreadScopeAccessor.Instance.UnRegisterScope(this);

			PerformDisposal(key2Session.Values);			
		}

		protected virtual void Initialize(ISession session)
		{
		}

		protected virtual void PerformDisposal(ICollection sessions)
		{
		}

		protected internal void PerformDisposal( ICollection sessions, bool flush, bool close )
		{
			foreach(ISession session in sessions)
			{
				if (flush) session.Flush();
				if (close) session.Close();
			}
		}

		protected internal virtual void DiscardSessions(ICollection sessions)
		{
			foreach(ISession session in sessions)
			{
				RemoveSession(session);
			}
		}

		internal ICollection GetSessions()
		{
			return key2Session.Values;
		}

		private void RemoveSession(ISession session)
		{
			foreach(DictionaryEntry entry in key2Session)
			{
				if (Object.ReferenceEquals(entry.Value, session))
				{
					session.Close();
					key2Session.Remove(entry.Key);
					break;
				}
			}
		}
	}
}
