// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;


	public class WeakReferenceCacheProvider : ICacheProvider
	{
		private IDictionary entries = Hashtable.Synchronized(new Hashtable());

		public void Init(IServiceProvider serviceProvider)
		{
		}

		public bool HasKey(String key)
		{
			return Get(key) != null;
		}

		public object Get(String key)
		{
			WeakReference reference = (WeakReference) entries[key];

			if (reference == null) return null;

			if (reference.IsAlive)
			{
				return reference.Target;
			}

			Delete(key);

			return null;
		}

		public void Store(String key, object data)
		{
			entries[key] = new WeakReference(data);
		}

		public void Delete(String key)
		{
			entries.Remove(key);
		}
	}
}
