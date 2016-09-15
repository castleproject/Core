﻿// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	internal class SlimReadLockHolder : ILockHolder
	{
		private readonly ReaderWriterLockSlim locker;
		private bool lockAcquired;

		public SlimReadLockHolder(ReaderWriterLockSlim locker, bool waitForLock)
		{
			this.locker = locker;
			if(waitForLock)
			{
				locker.EnterReadLock();
				lockAcquired = true;
				return;
			}
			lockAcquired = locker.TryEnterReadLock(0);
		}

		public void Dispose()
		{
			if (!LockAcquired) return;
			locker.ExitReadLock();
			lockAcquired = false;
		}

		public bool LockAcquired
		{
			get { return lockAcquired; }
		}
	}
}