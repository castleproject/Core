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
	internal class NoOpUpgradeableLock : IUpgradeableLockHolder
	{
		public static readonly IUpgradeableLockHolder Lock = new NoOpUpgradeableLock();

		public void Dispose()
		{

		}

		public bool LockAcquired
		{
			get { return true; }
		}

		public ILockHolder Upgrade()
		{
			return NoOpLock.Lock;
		}

		public ILockHolder Upgrade(bool waitForLock)
		{
			return NoOpLock.Lock;
		}
	}
}