// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Threading;

	public class SlimReaderWriterLock
	{
#if SILVERLIGHT || !DOTNET35
		private readonly object locker = new object();

		public void EnterReadLock()
		{
			Monitor.Enter(locker);
		}

		public bool TryEnterReadLock()
		{
			return Monitor.TryEnter(locker,0);
		}

		public void EnterWriteLock()
		{
			Monitor.Enter(locker);
		}

		public bool TryEnterWriteLock()
		{
			return Monitor.TryEnter(locker,0);
		}

		public void ExitReadLock()
		{
			Monitor.Exit(locker);
		}

		public void ExitWriteLock()
		{
			Monitor.Exit(locker);
		}

		public void EnterUpgradeableReadLock()
		{
			EnterWriteLock();
		}

		public void ExitUpgradeableReadLock()
		{
			ExitWriteLock();
		}

#else
		private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		public void EnterReadLock()
		{
			locker.EnterReadLock();
		}
		public bool TryEnterReadLock()
		{
			return locker.TryEnterReadLock(0);
		}

		public void EnterWriteLock()
		{
			locker.EnterWriteLock();
		}
		public bool TryEnterWriteLock()
		{
			return locker.TryEnterWriteLock(0);
		}

		public void EnterUpgradeableReadLock()
		{
			locker.EnterUpgradeableReadLock();
		}

		public void ExitReadLock()
		{
			locker.ExitReadLock();
		}

		public void ExitWriteLock()
		{
			locker.ExitWriteLock();
		}

		public void ExitUpgradeableReadLock()
		{
			locker.ExitUpgradeableReadLock();
		}
#endif
	}
}
