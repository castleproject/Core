// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Depicts the contract for cache provider. Was
	/// created to be used with providers like memcached.
	/// </summary>
	public interface ICacheProvider : IProvider
	{
		/// <summary>
		/// Determines whether the specified key is on the cache.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the cache has the key; otherwise, <c>false</c>.
		/// </returns>
		bool HasKey(String key);

		/// <summary>
		/// Gets the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		object Get(String key);

		/// <summary>
		/// Stores the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="data">The data.</param>
		void Store(String key, object data);

		/// <summary>
		/// Deletes the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		void Delete(String key);
	}
}
