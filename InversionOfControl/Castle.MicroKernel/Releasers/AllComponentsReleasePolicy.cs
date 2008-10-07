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

	[Serializable]
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		// TODO: Replace by Dictionary and ReadWritLockSlim
		private readonly IDictionary instance2Burden = Hashtable.Synchronized(
			new Hashtable(new Util.ReferenceEqualityComparer()));

		public virtual void Track(object instance, Burden burden)
		{
			instance2Burden[instance] = burden;
		}

		public bool HasTrack(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");

			return instance2Burden.Contains(instance);
		}

		public void Release(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			
			var burden = (Burden)instance2Burden[instance];

			if (burden != null)
			{
				instance2Burden.Remove(instance);

				burden.Release(this);
			}
		}

		public void Dispose()
		{
			foreach(DictionaryEntry entry in instance2Burden)
			{
				var burden = (Burden) entry.Value;
				burden.Release(this);
			}

			instance2Burden.Clear();
		}
	}
}
