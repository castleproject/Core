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

namespace Castle.Facilities.Cache.Manager
{
	using System.Text;

	using Castle.Model.Interceptor;

	/// <summary>
	/// Generates the key to retrieve/save objects from/to the cache.
	/// </summary>
	public class DefaultCacheKeyGenerator : ICacheKeyGenerator
	{
		/// <summary>
		/// Generates the key for a cache entry.
		/// </summary>
		/// <param name="invocation">the description of an invocation to the intercepted method.</param>
		/// <param name="arguments">the arguments of the invocation</param>
		/// <returns>the key for a cache entry.</returns>
		public string GenerateKey(IMethodInvocation invocation, object[] arguments)
		{
			StringBuilder cacheKey = new StringBuilder();
			cacheKey.Append(invocation.InvocationTarget.ToString());
			cacheKey.Append(".");
			cacheKey.Append(invocation.Method.Name);

			if ((arguments != null) && (arguments.Length != 0)) 
			{
				for (int i=0; i<arguments.Length; i++) 
				{
					cacheKey.Append(".").Append(arguments[i]);
				}
			}
			return cacheKey.ToString();
		}
	}
}
