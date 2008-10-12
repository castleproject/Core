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

namespace Castle.MicroKernel.Releasers
{
	using System;
	using System.Collections;
	using System.Threading;

	[Serializable]
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		private readonly IDictionary instance2Burden =
			new Hashtable(new Util.ReferenceEqualityComparer());

		private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		public virtual void Track(object instance, Burden burden)
		{
			rwLock.EnterWriteLock();

			try
			{
				instance2Burden[instance] = burden;
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		public bool HasTrack(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			rwLock.EnterReadLock();
			try
			{
				return instance2Burden.Contains(instance);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		public void Release(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			rwLock.EnterUpgradeableReadLock();

			try
			{
				var burden = (Burden) instance2Burden[instance];

				if (burden == null)
					return;

				rwLock.EnterWriteLock();

				try
				{
					burden = (Burden) instance2Burden[instance];
					if (burden == null)
						return;

					instance2Burden.Remove(instance);

					burden.Release(this);
				}
				finally
				{
					rwLock.ExitWriteLock();
				}
			}
			finally
			{
				rwLock.ExitUpgradeableReadLock();
			}
		}

		public void Dispose()
		{
			rwLock.EnterWriteLock();

			try
			{
				Burden[] burdens = new Burden[instance2Burden.Count];
				instance2Burden.Values.CopyTo(burdens, 0);

				foreach(Burden burden in burdens)
				{
					if (instance2Burden.Contains(burden))
					{
						burden.Release(this);
						instance2Burden.Remove(burden);
					}
				}
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}
	}
}