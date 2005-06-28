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

namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;
	using System.Collections;

	using NHibernate;

	/// <summary>
	/// Abstract <seealso cref="ISessionScope"/> implementation
	/// </summary>
	public abstract class AbstractScope : ISessionScope
	{
		protected Hashtable _key2Session = new Hashtable();

		public AbstractScope()
		{
			ThreadScopeInfo.RegisterScope(this);
		}

		public virtual bool IsKeyKnown(object key)
		{
			return _key2Session.Contains(key);
		}

		public virtual void RegisterSession(object key, ISession session)
		{
			_key2Session[key] = session;

			Initialize(session);
		}

		public virtual ISession GetSession(object key)
		{
			return _key2Session[key] as ISession;
		}

		public void Dispose()
		{
			ThreadScopeInfo.UnRegisterScope(this);

			PerformDisposal(_key2Session.Values);			

			_key2Session.Clear();
			_key2Session = null;
		}

		protected virtual void Initialize(ISession session)
		{
		}

		protected virtual void PerformDisposal(ICollection sessions)
		{
		}
	}
}
