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

	/// <summary>
	/// Class to allow scopes to reach the implementation
	/// of <see cref="IThreadScopeInfo"/>. Also implements 
	/// the <see cref="IThreadScopeInfo"/> delegating the calls to 
	/// the scope set.
	/// </summary>
	public sealed class ThreadScopeAccessor : IThreadScopeInfo
	{
		private static readonly ThreadScopeAccessor instance = new ThreadScopeAccessor();
		
		private IThreadScopeInfo scopeInfo;

		public static ThreadScopeAccessor Instance
		{
			get { return instance; }
		}

		public IThreadScopeInfo ScopeInfo
		{
			get { return scopeInfo; }
			set { scopeInfo = value; }
		}

		#region IThreadScopeInfo Members

		public System.Collections.Stack CurrentStack
		{
			get { return scopeInfo.CurrentStack; }
		}

		public ISessionScope GetRegisteredScope()
		{
			return scopeInfo.GetRegisteredScope();
		}

		public void RegisterScope(ISessionScope scope)
		{
			scopeInfo.RegisterScope(scope);
		}

		public void UnRegisterScope(ISessionScope scope)
		{
			scopeInfo.UnRegisterScope(scope);
		}

		public bool HasInitializedScope
		{
			get { return scopeInfo.HasInitializedScope; }
		}

		#endregion
	}
}