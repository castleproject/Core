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

namespace Castle.MicroKernel.Releasers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	[Serializable]
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		private readonly IDictionary<object, Burden> instance2Burden =
			new Dictionary<object, Burden>(new Util.ReferenceEqualityComparer());

		private readonly ReaderWriterLock rwLock = new ReaderWriterLock();

		public virtual void Track(object instance, Burden burden)
		{
			rwLock.AcquireWriterLock(Timeout.Infinite);
			try
			{
				instance2Burden[instance] = burden;
			}
			finally
			{
				rwLock.ReleaseWriterLock();
			}
		}

		public bool HasTrack(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			rwLock.AcquireReaderLock(Timeout.Infinite);

			try
			{
				return instance2Burden.ContainsKey(instance);
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}

		public void Release(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			rwLock.AcquireReaderLock(Timeout.Infinite);

			try
			{
				Burden burden;

				if (!instance2Burden.TryGetValue(instance, out burden))
					return;

				LockCookie cookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);

				try
				{
					if (!instance2Burden.TryGetValue(instance, out burden))
						return;

					instance2Burden.Remove(instance);

					burden.Release(this);
				}
				finally
				{
					rwLock.DowngradeFromWriterLock(ref cookie);
				}
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}

		public void Dispose()
		{
			rwLock.AcquireWriterLock(Timeout.Infinite);

			try
			{
				KeyValuePair<object, Burden>[] burdens = 
					new KeyValuePair<object, Burden>[instance2Burden.Count];
				instance2Burden.CopyTo(burdens, 0);

				foreach (KeyValuePair<object, Burden> burden in burdens)
				{
					if (instance2Burden.ContainsKey(burden.Key))
					{
						burden.Value.Release(this);
						instance2Burden.Remove(burden.Key);
					}
				}
			}
			finally
			{
				rwLock.ReleaseWriterLock();
			}
		}
	}
}