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

	internal class ScopeUtil
	{
		internal static ISessionScope FindPreviousScope(ISessionScope thisScope, 
			bool preferenceForTransactional)
		{
			return FindPreviousScope(thisScope, preferenceForTransactional, false);
		}

		internal static ISessionScope FindPreviousScope(ISessionScope thisScope, 
			bool preferenceForTransactional, bool doNotReturnSessionScope)
		{
			object[] items = ThreadScopeAccessor.Instance.CurrentStack.ToArray();

			ISessionScope first = null;

			for (int i = 0; i < items.Length; i++)
			{
				ISessionScope scope = items[i] as ISessionScope;

				if (scope == thisScope) continue;

				if (first == null) first = scope;

				if (!preferenceForTransactional) break;

				if (preferenceForTransactional && scope.ScopeType == SessionScopeType.Transactional)
				{
					return scope;
				}
			}

			return doNotReturnSessionScope ? null : first;
		}
	}
}
