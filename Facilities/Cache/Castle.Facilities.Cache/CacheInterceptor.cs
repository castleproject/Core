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

using System;
using System.Text;
using Castle.Facilities.Cache.Manager;
using Castle.Model.Interceptor;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Summary description for CacheInterceptor.
	/// </summary>
	public class CacheInterceptor : IMethodInterceptor
	{
		public readonly object NULL_OBJECT = new Object(); 

		private CacheConfigHolder _cacheConfigHolder = null;

		public CacheInterceptor(CacheConfigHolder transactionConfHolder)
		{
			_cacheConfigHolder = transactionConfHolder;
		}

		#region IMethodInterceptor Members

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			CacheConfig config = _cacheConfigHolder.GetConfig( invocation.Method.DeclaringType );
			ICacheManager cacheManager = config.GetCacheManager( invocation.Method );

			if (config != null && config.IsMethodCache( invocation.Method ))
			{
				String cacheKey = GetCacheKey( invocation, args );
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

		private string GetCacheKey(IMethodInvocation invocation, object[] arguments)
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

		#endregion

	}
}
