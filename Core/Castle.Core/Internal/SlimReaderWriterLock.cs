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

namespace Castle.Core.Internal
{
	using System.Threading;

	public class SlimReaderWriterLock
	{
#if SILVERLIGHT
		object _lock = new object();

		public SlimReaderWriterLock()
		{
		}

		public void EnterReadLock()
		{
			Monitor.Enter(this._lock);
		}

		public void EnterWriteLock()
		{
			Monitor.Enter(this._lock);
		}

		public void ExitReadLock()
		{
			Monitor.Exit(this._lock);
		}

		public void ExitWriteLock()
		{
			Monitor.Exit(this._lock);
		}
#else
		private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);

		public void EnterReadLock()
		{
			this._lock.EnterUpgradeableReadLock();
		}

		public void EnterWriteLock()
		{
			this._lock.EnterWriteLock();
		}

		public void ExitReadLock()
		{
			this._lock.ExitUpgradeableReadLock();
		}

		public void ExitWriteLock()
		{
			this._lock.ExitWriteLock();
		}
#endif
	}
}
