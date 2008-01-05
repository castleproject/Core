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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord.Framework.Scopes;
	using NHibernate;

	/// <summary>
	/// Implementation of <see cref="ISessionScope"/> to 
	/// augment performance by caching the session, thus
	/// avoiding too much opens/flushes/closes.
	/// </summary>
	public class SessionScope : AbstractScope
	{
		/// <summary>
		/// Is set to true if the session went stalled due to an error (usually db operations)
		/// </summary>
		private bool hasSessionError;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionScope"/> class.
		/// </summary>
		/// <param name="flushAction">The flush action.</param>
		/// <param name="type">The type.</param>
		protected SessionScope(FlushAction flushAction, SessionScopeType type) : base(flushAction, type)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionScope"/> class.
		/// </summary>
		public SessionScope() : this(FlushAction.Auto)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionScope"/> class.
		/// </summary>
		/// <param name="flushAction">The flush action.</param>
		public SessionScope(FlushAction flushAction) : base(flushAction, SessionScopeType.Simple)
		{
		}

		/// <summary>
		/// Deprecated! Disposes the specified discard changes. Please use new SessionScope(FlushAction.Never)
		/// </summary>
		/// <param name="discardChanges">if set to <c>true</c> [discard changes].</param>
		[Obsolete("This useage is deprecated - please use new SessionScope(FlushAction.Never)")]
		public void Dispose(bool discardChanges)
		{
			ThreadScopeAccessor.Instance.UnRegisterScope(this);

			PerformDisposal(key2Session.Values, !discardChanges, true);

			key2Session.Clear();
			key2Session = null;
		}

		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected override void PerformDisposal(ICollection<ISession> sessions)
		{
			if (hasSessionError || FlushAction == FlushAction.Never)
			{
				PerformDisposal(sessions, false, true);
			}
			else if (FlushAction == FlushAction.Auto)
			{
				PerformDisposal(sessions, true, true);
			}
		}

		/// <summary>
		/// This is called when an action on a session fails
		/// </summary>
		/// <param name="session">The session</param>
		public override void FailSession(ISession session)
		{
			hasSessionError = true;
		}

		/// <summary>
		/// Gets or sets a flag indicating whether this instance has session error.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has session error; otherwise, <c>false</c>.
		/// </value>
		public bool HasSessionError
		{
			get { return hasSessionError; }
			set { hasSessionError = true; }
		}

		/// <summary>
		/// Gets the current scope
		/// </summary>
		/// <value>The current.</value>
		public static ISessionScope Current
		{
			get
			{
				if (ThreadScopeAccessor.Instance.HasInitializedScope)
				{
					return ThreadScopeAccessor.Instance.GetRegisteredScope();
				}
				
				return null;
			}
		}
	}
}
