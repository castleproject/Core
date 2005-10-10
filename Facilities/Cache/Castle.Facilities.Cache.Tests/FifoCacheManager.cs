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

namespace Castle.Facilities.Cache.Tests
{
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.Facilities.Cache.Manager;

	/// <summary>
	/// Description résumée de FifoCacheManager.
	/// </summary>
	public class FifoCacheManager : ICacheManager
	{
		#region Fields 
		private int _cacheSize = 0;
		private IDictionary _cache = null;
		private IList _keyList = null;
		private ICacheKeyGenerator _cacheKeyGenerator = null;
		#endregion

		public IList KeyList
		{
			get { return _keyList; }
		}

		#region Constructor (s) / Destructor
		/// <summary>
		/// 
		/// </summary>
		public FifoCacheManager(int cacheSize) 
		{
			_cacheSize = cacheSize;
			_cache = new HybridDictionary();
			_keyList = new ArrayList();
		}
		#endregion

		#region ICacheManager Members

		/// <summary>
		/// A generator of keys for a cache entry.
		/// </summary>
		public ICacheKeyGenerator CacheKeyGenerator
		{
			get { return _cacheKeyGenerator; }
			set { _cacheKeyGenerator = value; }
		}

		/// <summary>
		/// Clears all elements from the cache.
		/// </summary>
		public void Clear()
		{
			lock(this) 
			{
				_cache.Clear();
				_keyList.Clear();
			}		
		}


		/// <summary>
		/// Adds an item with the specified key and value into cached data.
		/// Gets a cached object with the specified key.
		/// </summary>
		/// <value>The cached object or <c>null</c></value>
		public object this [object key] 
		{
			get
			{
				lock (this) 
				{
					return _cache[key];
				}
			}
			set
			{
				lock (this) 
				{
					_cache.Add(key, value);
					_keyList.Add(key);
					if (_keyList.Count > _cacheSize) 
					{
						object oldestKey = _keyList[0];
						_keyList.Remove(0);
						_cache.Remove(oldestKey);
					}		
				}
			}
		}
		
		#endregion

	}
}
