// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	internal sealed class SynchronizedDictionary<TKey, TValue> : IDisposable
	{
		private Dictionary<TKey, TValue> items;
		private ReaderWriterLockSlim itemsLock;

		public SynchronizedDictionary()
		{
			items = new Dictionary<TKey, TValue>();
			itemsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		[Obsolete] // TODO: Remove this property along with the `ModuleScope.Lock` property.
		public ReaderWriterLockSlim Lock => itemsLock;

		public void AddOrUpdateWithoutTakingLock(TKey key, TValue value)
		{
			items[key] = value;
		}

		public void Dispose()
		{
			itemsLock.Dispose();
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			TValue value;

			itemsLock.EnterReadLock();
			try
			{
				if (items.TryGetValue(key, out value))
				{
					return value;
				}
			}
			finally
			{
				itemsLock.ExitReadLock();
			}

			itemsLock.EnterUpgradeableReadLock();
			try
			{
				if (items.TryGetValue(key, out value))
				{
					return value;
				}
				else
				{
					value = valueFactory.Invoke(key);

					itemsLock.EnterWriteLock();
					try
					{
						items.Add(key, value);
						return value;
					}
					finally
					{
						itemsLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				itemsLock.ExitUpgradeableReadLock();
			}
		}

		public TValue GetOrAddWithoutTakingLock(TKey key, Func<TKey, TValue> valueFactory)
		{
			TValue value;

			if (items.TryGetValue(key, out value))
			{
				return value;
			}
			else
			{
				value = valueFactory.Invoke(key);
				items.Add(key, value);
				return value;
			}
		}

		public void ForEach(Action<TKey, TValue> action)
		{
			itemsLock.EnterReadLock();
			try
			{
				foreach (var item in items)
				{
					action.Invoke(item.Key, item.Value);
				}
			}
			finally
			{
				itemsLock.ExitReadLock();
			}
		}

		[Obsolete] // TODO: Remove this method along with the `ModuleScope.GetFromCache` method.
		public bool TryGetValueWithoutTakingLock(TKey key, out TValue value)
		{
			return items.TryGetValue(key, out value);
		}
	}
}
