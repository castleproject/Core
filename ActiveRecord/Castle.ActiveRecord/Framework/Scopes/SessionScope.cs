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

namespace Castle.ActiveRecord
{
	using System.Collections;

	using Castle.ActiveRecord.Framework.Scopes;

	/// <summary>
	/// Implementation of <see cref="ISessionScope"/> to 
	/// augment performance by caching the session, thus
	/// avoiding too much opens/flushes/closes.
	/// </summary>
	public class SessionScope : AbstractScope
	{
		protected SessionScope(SessionScopeType type) : base(type)
		{
		}

		public SessionScope() : base(SessionScopeType.Simple)
		{
		}

		public void Dispose(bool discardChanges)
		{
			ThreadScopeAccessor.Instance.UnRegisterScope(this);

			PerformDisposal(key2Session.Values, !discardChanges, true);

			key2Session.Clear();
			key2Session = null;
		}

		protected override void PerformDisposal(ICollection sessions)
		{
			PerformDisposal(sessions, true, true);
		}
	}
}
