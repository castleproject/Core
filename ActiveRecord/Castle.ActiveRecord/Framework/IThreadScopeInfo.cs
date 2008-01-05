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

namespace Castle.ActiveRecord.Framework
{
	using System.Collections;

	/// <summary>
	/// Implementation of this interface provide a way to get the current scope.
	/// This is used by the rest of the Active Record framework to grab a scope (and from it a session).
	/// </summary>
	public interface IThreadScopeInfo
	{
		/// <summary>
		/// Gets the current stack.
		/// </summary>
		/// <value>The current stack.</value>
		Stack CurrentStack { get; }

		/// <summary>
		/// Gets the registered scope.
		/// </summary>
		/// <returns></returns>
		ISessionScope GetRegisteredScope();

		/// <summary>
		/// Registers the scope.
		/// </summary>
		/// <param name="scope">The scope.</param>
		void RegisterScope(ISessionScope scope);

		/// <summary>
		/// Unregister the scope.
		/// </summary>
		/// <param name="scope">The scope.</param>
		void UnRegisterScope(ISessionScope scope);

		/// <summary>
		/// Gets a value indicating whether this instance has initialized scope.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has initialized scope; otherwise, <c>false</c>.
		/// </value>
		bool HasInitializedScope { get; }
	}
}
