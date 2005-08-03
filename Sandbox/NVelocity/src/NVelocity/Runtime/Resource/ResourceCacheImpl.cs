// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Runtime.Resource
{
	using System;
	using System.Collections;
	using Commons.Collections;

	/// <summary>
	/// Default implementation of the resource cache for the default
	/// ResourceManager.  The cache uses a <i>least recently used</i> (LRU)
	/// algorithm, with a maximum size specified via the
	/// <code>resource.manager.cache.size</code> property (idenfied by the
	/// {@link
	/// org.apache.velocity.runtime.RuntimeConstants#RESOURCE_MANAGER_CACHE_SIZE}
	/// constant).  This property get be set to <code>0</code> or less for
	/// a greedy, unbounded cache (the behavior from pre-v1.5).
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
	/// </author>
	/// <author> <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
	/// </author>
	/// <version> $Id: ResourceCacheImpl.cs,v 1.5 2004/12/23 08:14:32 corts Exp $
	///
	/// </version>
	public class ResourceCacheImpl : ResourceCache
	{
		/// <summary>
		/// Cache storage, assumed to be thread-safe.
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'cache' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		protected internal IDictionary cache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Runtime services, generally initialized by the
		/// <code>initialize()</code> method.
		/// </summary>
		protected internal RuntimeServices rsvc = null;

		public ResourceCacheImpl()
		{
		}

		public void initialize(RuntimeServices rs)
		{
			rsvc = rs;

			int maxSize = rsvc.getInt(RuntimeConstants_Fields.RESOURCE_MANAGER_DEFAULTCACHE_SIZE, 89);
			if (maxSize > 0)
			{
				// Create a whole new Map here to avoid hanging on to a
				// handle to the unsynch'd LRUMap for our lifetime.
				LRUMap lruCache = LRUMap.Synchronized(new LRUMap(maxSize));
				lruCache.AddAll(cache);
				cache = lruCache;
			}

			rsvc.info("ResourceCache : initialized. (" + this.GetType() + ")");
		}

		public Resource get(Object key)
		{
			return (Resource) cache[key];
		}

		public Resource put(Object key, Resource value)
		{
			Object o = cache[key];
			cache[key] = value;
			return (Resource) o;
		}

		public Resource remove(Object key)
		{
			Object o = cache[key];
			cache.Remove(key);
			return (Resource) o;
		}

		public IEnumerator enumerateKeys()
		{
			return cache.Keys.GetEnumerator();
		}
	}
}