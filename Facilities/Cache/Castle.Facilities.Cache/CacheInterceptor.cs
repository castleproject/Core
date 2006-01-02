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

namespace Castle.Facilities.Cache
{
	using System;

	using Castle.Facilities.Cache.Manager;
	using Castle.Model.Interceptor;

	/// <summary>
	/// Caches the return value of the intercepted method.
	/// </summary>
	public class CacheInterceptor : IMethodInterceptor
	{
		public static readonly object NULL_OBJECT = new Object(); 

		private CacheConfigHolder _cacheConfigHolder = null;

		public CacheInterceptor(CacheConfigHolder transactionConfHolder)
		{
			_cacheConfigHolder = transactionConfHolder;
		}

		/// <summary>
		/// Returns from the cache provider the value saved with the key generated
		/// using the specified <code>IMethodInvocation</code>. If the object is not
		/// found in the cache, the intercepted method is executed and its returned
		/// value is saved in the cached and returned by this method.
		/// </summary>
		/// <param name="invocation">the description of the intercepted method.</param>
		/// <param name="args">the arguments of the intercepted method.</param>
		/// <returns>the object stored in the cache.</returns>
		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			CacheConfig config = _cacheConfigHolder.GetConfig( invocation.Method.DeclaringType );

			if (config != null && config.IsMethodCache( invocation.Method ))
			{
				ICacheManager cacheManager = config.GetCacheManager( invocation.Method );
				String cacheKey = cacheManager.CacheKeyGenerator.GenerateKey( invocation, args );
				object result = cacheManager[ cacheKey ];

				if (result == null)
				{
					//call target/sub-interceptor
					result = invocation.Proceed(args);

					//cache method result
					if (result == null)
					{
						cacheManager[ cacheKey ] = NULL_OBJECT;
					}
					else
					{
						cacheManager[ cacheKey ] = result;	
					}
				}
				else if (result == NULL_OBJECT) 
				{ 
					// convert the marker object back into a null value 
					result = null; 
				} 

				return result;
			}
			else
			{
				return invocation.Proceed(args);
			}
		}
	}
}
