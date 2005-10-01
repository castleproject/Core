using System;
using System.Collections;
using System.Collections.Specialized;

namespace Castle.Facilities.Cache.Manager
{
	/// <summary>
	/// Description résumée de MemoryCache.
	/// </summary>
	public class MemoryCacheManager : ICacheManager
	{
		#region Fields 
		private IDictionary _cache = null;
		#endregion

		public MemoryCacheManager()
		{
			_cache = new HybridDictionary();
		}

		#region ICacheManager Members

		/// <summary>
		/// Adds an item with the specified key and value into cached data.
		/// Gets a cached object with the specified key.
		/// </summary>
		/// <value>The cached object or <c>null</c></value>
		public object this[object key]
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
					_cache[key] = value;	
				}			
			}
		}


		/// <summary>
		/// Clears all elements from the cache.
		/// </summary>
		public void Flush()
		{
			lock(this) 
			{
				_cache.Clear();
			}				
		}

		/// <summary>
		/// Configures the CacheController
		/// </summary>
		/// <param name="properties"></param>
		public void Configure(IDictionary properties)
		{
			
		}

		#endregion
	}
}
