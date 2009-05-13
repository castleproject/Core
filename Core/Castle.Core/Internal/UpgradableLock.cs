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
	using System;

	public struct UpgradableLock : IDisposable
	{
		private readonly SlimReaderWriterLock locker;
		private bool lockWasUpgraded;

		public UpgradableLock(SlimReaderWriterLock locker)
		{
			this.locker = locker;
			locker.EnterUpgradeableReadLock();
			lockWasUpgraded = false;
		}

		public void Upgrade()
		{
			locker.EnterWriteLock();
			lockWasUpgraded = true;
		}

		public void Dispose()
		{
			if (lockWasUpgraded)
			{
				locker.ExitWriteLock();
			}
			
			locker.ExitUpgradeableReadLock();
		}
	}
}
