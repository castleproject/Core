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

	/// <summary>
	/// Base <see cref="IThreadScopeInfo"/> implementation. It's up 
	/// to derived classes to provide a correct implementation 
	/// of <c>CurrentStack</c> only
	/// </summary>
	public abstract class AbstractThreadScopeInfo : IThreadScopeInfo
	{
		public abstract Stack CurrentStack { get; }

		public void RegisterScope(ISessionScope scope)
		{
			CurrentStack.Push(scope);
		}

		public ISessionScope GetRegisteredScope()
		{
			Stack stack = CurrentStack;

			if (stack.Count == 0)
			{
				return null;
			}
			else
			{
				return stack.Peek() as ISessionScope;
			}
		}

		public void UnRegisterScope(ISessionScope scope)
		{
			if (GetRegisteredScope() != scope)
			{
				throw new ScopeMachineryException("Tried to unregister a scope that is not the active one");
			}

			CurrentStack.Pop();
		}

		public bool HasInitializedScope
		{
			get { return GetRegisteredScope() != null; }
		}
	}
}
