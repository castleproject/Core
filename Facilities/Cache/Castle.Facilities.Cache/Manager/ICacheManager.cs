using System;
using System.Collections;

namespace Castle.Facilities.Cache.Manager
{
	/// <summary>
	/// Description résumée de ICacheManager.
	/// </summary>
	public interface ICacheManager
	{
		#region Properties
		/// <summary>
		/// Adds an item with the specified key and value into cached data.
		/// Gets a cached object with the specified key.
		/// </summary>
		/// <value>The cached object or <c>null</c></value>
		object this [object key] 
		{
			get;
			set;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clears all elements from the cache.
		/// </summary>
		void Flush ();

		/// <summary>
		/// Configures the CacheController
		/// </summary>
		/// <param name="properties"></param>
		void Configure(IDictionary properties);
		#endregion
	}
}
