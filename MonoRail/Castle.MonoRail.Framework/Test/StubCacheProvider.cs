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

namespace Castle.MonoRail.Framework.Test
{
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Represents a mock implementation of <see cref="ICacheProvider"/> for unit test purposes.
	/// </summary>
	public class StubCacheProvider : ICacheProvider
	{
		private readonly IDictionary dictionary = new HybridDictionary(true);

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IMonoRailServices provider)
		{
		}

		/// <summary>
		/// Determines whether the specified key is on the cache.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the cache has the key; otherwise, <c>false</c>.
		/// </returns>
		public bool HasKey(string key)
		{
			return dictionary.Contains(key);
		}

		/// <summary>
		/// Gets the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public object Get(string key)
		{
			return dictionary[key];
		}

		/// <summary>
		/// Stores the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="data">The data.</param>
		public void Store(string key, object data)
		{
			dictionary[key] = data;
		}

		/// <summary>
		/// Deletes the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public void Delete(string key)
		{
			dictionary.Remove(key);
		}
	}
}