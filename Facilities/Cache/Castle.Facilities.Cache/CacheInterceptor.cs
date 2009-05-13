// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Castle.Core.Interceptor;

	/// <summary>
	/// Caches the return value of the intercepted method.
	/// </summary>
	public class CacheInterceptor : IInterceptor
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly object NullObject = new Object(); 

		private CacheConfigHolder _cacheConfigHolder = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheInterceptor"/> class.
		/// </summary>
		/// <param name="transactionConfHolder">The transaction conf holder.</param>
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
		/// <returns>the object stored in the cache.</returns>
		public void Intercept(IInvocation invocation)
		{
			CacheConfig config = _cacheConfigHolder.GetConfig( invocation.MethodInvocationTarget.DeclaringType );

			if (config != null && config.IsMethodCache( invocation.MethodInvocationTarget ))
			{
				ICacheManager cacheManager = config.GetCacheManager(invocation.MethodInvocationTarget);
				String cacheKey = cacheManager.CacheKeyGenerator.GenerateKey( invocation );
				object result = cacheManager[ cacheKey ];

				if (result == null)
				{
					//call target/sub-interceptor
					invocation.Proceed();
					result = invocation.ReturnValue;

					//cache method result
					if (result == null)
					{
						cacheManager[ cacheKey ] = NullObject;
					}
					else
					{
						cacheManager[ cacheKey ] = result;	
					}
				}
				else if (result == NullObject) 
				{ 
					// convert the marker object back into a null value 
					result = null; 
				}
				invocation.ReturnValue = result;
			}
			else
			{
				invocation.Proceed();
			}
		}
	}
}
