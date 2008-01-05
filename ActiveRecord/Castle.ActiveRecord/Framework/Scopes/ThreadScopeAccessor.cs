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
	using System.Collections;

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

		/// <summary>
		/// Gets the single instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ThreadScopeAccessor Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// Gets or sets the scope info.
		/// </summary>
		/// <value>The scope info.</value>
		public IThreadScopeInfo ScopeInfo
		{
			get { return scopeInfo; }
			set { scopeInfo = value; }
		}

		#region IThreadScopeInfo Members

		/// <summary>
		/// Gets the current stack.
		/// </summary>
		/// <value>The current stack.</value>
		public Stack CurrentStack
		{
			get { return scopeInfo.CurrentStack; }
		}

		/// <summary>
		/// Gets the registered scope.
		/// </summary>
		/// <returns></returns>
		public ISessionScope GetRegisteredScope()
		{
			if (scopeInfo == null)
			{
				throw new ActiveRecordException(
					"Can't get registered scope because the Active Record framework was not initialized.");
			}
			return scopeInfo.GetRegisteredScope();
		}

		/// <summary>
		/// Registers the scope.
		/// </summary>
		/// <param name="scope">The scope.</param>
		public void RegisterScope(ISessionScope scope)
		{
			if (scopeInfo == null)
			{
				throw new ActiveRecordException("A scope tried to registered itself within the framework, " +
				                                "but the Active Record was not initialized");
			}
			scopeInfo.RegisterScope(scope);
		}

		/// <summary>
		/// Unregister the scope.
		/// </summary>
		/// <param name="scope">The scope.</param>
		public void UnRegisterScope(ISessionScope scope)
		{
			scopeInfo.UnRegisterScope(scope);
		}

		/// <summary>
		/// Gets a value indicating whether this instance has initialized scope.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has initialized scope; otherwise, <c>false</c>.
		/// </value>
		public bool HasInitializedScope
		{
			get { return scopeInfo.HasInitializedScope; }
		}

		#endregion
	}
}
