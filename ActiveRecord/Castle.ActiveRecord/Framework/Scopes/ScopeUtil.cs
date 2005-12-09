namespace Castle.ActiveRecord.Framework.Scopes
{
	using System;

	class ScopeUtil
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
