// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
#if SILVERLIGHT
	internal class MonitorLock : Lock
	{
		private readonly object locker = new object();

		public override IUpgradeableLockHolder ForReadingUpgradeable()
		{
			return ForReadingUpgradeable(true);
		}

		public override ILockHolder ForReading()
		{
			return ForReading(true);
		}

		public override ILockHolder ForWriting()
		{
			return ForWriting(true);
		}

		public override IUpgradeableLockHolder ForReadingUpgradeable(bool waitForLock)
		{
			return new MonitorUpgradeableLockHolder(locker, waitForLock);
		}

		public override ILockHolder ForReading(bool waitForLock)
		{
			return new MonitorLockHolder(locker, waitForLock);
		}

		public override ILockHolder ForWriting(bool waitForLock)
		{
			return new MonitorLockHolder(locker, waitForLock);
		}
	}
#endif
}