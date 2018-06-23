// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public class SingletonDispenser<TKey, TItem>
		where TItem : class
	{
		private readonly ReaderWriterLockSlim locker;
		private readonly Dictionary<TKey, object> items;
		private readonly Func<TKey, TItem> factory;

		public SingletonDispenser(Func<TKey, TItem> factory)
		{
			if (factory == null)
				throw Error.ArgumentNull("factory");

			this.locker  = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			this.items   = new Dictionary<TKey, object>();
			this.factory = factory;
		}

		public TItem this[TKey key]
		{
			get { return GetOrCreate(key); }
			protected set { items[key] = value; }
		}

		private TItem GetOrCreate(TKey key)
		{
			object item;
			return TryGetExistingItem(key, out item)
				? item as TItem ?? WaitForCreate(key, item)
				: Create(key, item);
		}

		private bool TryGetExistingItem(TKey key, out object item)
		{
			locker.EnterReadLock();
			try
			{
				if (items.TryGetValue(key, out item))
				{
					return true;
				}
			}
			finally
			{
				locker.ExitReadLock();
			}

			locker.EnterUpgradeableReadLock();
			try
			{
				if (items.TryGetValue(key, out item))
				{
					return true;
				}
				else
				{
					locker.EnterWriteLock();
					try
					{
						items[key] = item = new ManualResetEvent(false);
						return false;
					}
					finally
					{
						locker.ExitWriteLock();
					}
				}
			}
			finally
			{
				locker.ExitUpgradeableReadLock();
			}
		}

		private TItem WaitForCreate(TKey key, object item)
		{
			var handle = (ManualResetEvent) item;

			handle.WaitOne();

			locker.EnterReadLock();
			try
			{
				return (TItem)items[key];
			}
			finally
			{
				locker.ExitReadLock();
			}
		}

		private TItem Create(TKey key, object item)
		{
			var handle = (ManualResetEvent) item;

			var result = factory(key);

			locker.EnterWriteLock();
			try
			{
				items[key] = result;
			}
			finally
			{
				locker.ExitWriteLock();
			}

			handle.Set();
			return result;
		}
	}
}
#endif
