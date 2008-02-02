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

namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord;
	
	using NHibernate;

	/// <summary>
	/// Abstract <seealso cref="ISessionScope"/> implementation
	/// </summary>
	public abstract class AbstractScope : MarshalByRefObject, ISessionScope
	{
		private readonly SessionScopeType type;
		
		private readonly FlushAction flushAction;

		/// <summary>
		/// Map between a key to its session
		/// </summary>
		protected IDictionary<object, ISession> key2Session = new Dictionary<object, ISession>();

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractScope"/> class.
		/// </summary>
		/// <param name="flushAction">The flush action.</param>
		/// <param name="type">The type.</param>
		public AbstractScope(FlushAction flushAction, SessionScopeType type)
		{
			this.flushAction = flushAction;
			this.type = type;
			
			ThreadScopeAccessor.Instance.RegisterScope(this);
		}

		/// <summary>
		/// Returns the <see cref="SessionScopeType"/> defined 
		/// for this scope
		/// </summary>
		public SessionScopeType ScopeType
		{
			get { return type; }
		}

		/// <summary>
		/// Returns the <see cref="ISessionScope.FlushAction"/> defined 
		/// for this scope
		/// </summary>
		public FlushAction FlushAction
		{
			get { return flushAction; }
		}

		/// <summary>
		/// Flushes the sessions that this scope 
		/// is maintaining
		/// </summary>
		public virtual void Flush()
		{
			foreach(ISession session in GetSessions())
			{
				session.Flush();
			}
		}

		/// <summary>
		/// Evicts the specified instance from the session cache.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void Evict(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			foreach(ISession session in GetSessions())
			{
				if (session.Contains(instance))
				{
					session.Evict(instance);
				}
			}
		}

		/// <summary>
		/// This method is invoked when the
		/// <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// instance needs a session instance. Instead of creating one it interrogates
		/// the active scope for one. The scope implementation must check if it
		/// has a session registered for the given key.
		/// <seealso cref="RegisterSession"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// 	<c>true</c> if the key exists within this scope instance
		/// </returns>
		public virtual bool IsKeyKnown(object key)
		{
			return key2Session.ContainsKey(key);
		}

		/// <summary>
		/// This method is invoked when no session was available
		/// at and the <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// just created one. So it registers the session created
		/// within this scope using a key. The scope implementation
		/// shouldn't make any assumption on what the key
		/// actually is as we reserve the right to change it
		/// <seealso cref="IsKeyKnown"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <param name="session">An instance of <c>ISession</c></param>
		public virtual void RegisterSession(object key, ISession session)
		{
			key2Session.Add(key, session);

			Initialize(session);
		}

		/// <summary>
		/// This method should return the session instance associated with the key.
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// the session instance or null if none was found
		/// </returns>
		public virtual ISession GetSession(object key)
		{
			return key2Session[key];
		}

		/// <summary>
		/// Implementors should return true if they
		/// want that their scope implementation
		/// be in charge of creating the session
		/// </summary>
		/// <value></value>
		public virtual bool WantsToCreateTheSession
		{
			get { return true; }
		}

		/// <summary>
		/// If the <see cref="WantsToCreateTheSession"/> returned
		/// <c>true</c> then this method is invoked to allow
		/// the scope to create a properly configured session
		/// </summary>
		/// <param name="sessionFactory">From where to open the session</param>
		/// <param name="interceptor">the NHibernate interceptor</param>
		/// <returns>the newly created session</returns>
		public virtual ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			ISession session = sessionFactory.OpenSession(interceptor);

			SetFlushMode(session);

			return session;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			ThreadScopeAccessor.Instance.UnRegisterScope(this);

			PerformDisposal(key2Session.Values);			
		}

		/// <summary>
		/// Initializes the specified session.
		/// </summary>
		/// <param name="session">The session.</param>
		protected virtual void Initialize(ISession session)
		{
		}

		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected virtual void PerformDisposal(ICollection<ISession> sessions)
		{
		}

		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		/// <param name="flush">if set to <c>true</c> [flush].</param>
		/// <param name="close">if set to <c>true</c> [close].</param>
		protected internal void PerformDisposal(ICollection<ISession> sessions, bool flush, bool close)
		{
			foreach(ISession session in sessions)
			{
				if (flush) session.Flush();
				if (close) session.Close();
			}
		}

		/// <summary>
		/// Discards the sessions.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected internal virtual void DiscardSessions(ICollection<ISession> sessions)
		{
			foreach(ISession session in sessions)
			{
				RemoveSession(session);
			}
		}

		/// <summary>
		/// Marks the session as failed
		/// </summary>
		/// <param name="session">The session</param>
		public abstract void FailSession(ISession session);

		/// <summary>
		/// Sets the flush mode.
		/// </summary>
		/// <param name="session">The session.</param>
		protected void SetFlushMode(ISession session)
		{
			if (FlushAction == FlushAction.Auto)
			{
				session.FlushMode = FlushMode.Auto;
			}
			else if (FlushAction == FlushAction.Never)
			{
				session.FlushMode = FlushMode.Never;
			}
		}

		/// <summary>
		/// Gets the sessions.
		/// </summary>
		/// <returns></returns>
		internal ICollection<ISession> GetSessions()
		{
			return key2Session.Values;
		}

		/// <summary>
		/// Removes the session.
		/// </summary>
		/// <param name="session">The session.</param>
		private void RemoveSession(ISession session)
		{
			foreach(KeyValuePair<object, ISession> entry in key2Session)
			{
				if (ReferenceEquals(entry.Value, session))
				{
					session.Close();
					key2Session.Remove(entry.Key);
					break;
				}
			}
		}
	}
}
