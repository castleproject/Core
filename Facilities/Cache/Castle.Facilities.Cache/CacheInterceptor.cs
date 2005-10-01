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
	/// Description résumée de CacheInterceptor.
	/// </summary>
	public class CacheInterceptor : IMethodInterceptor
	{
		ICacheManager _cacheManager = null;

		private CacheConfigHolder _cacheConfigHolder = null;

		public CacheInterceptor(CacheConfigHolder transactionConfHolder, ICacheManager cacheManager)
		{
			_cacheManager= cacheManager;
			_cacheConfigHolder = transactionConfHolder;
		}

		#region Membres de IMethodInterceptor

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			CacheConfig config = _cacheConfigHolder.GetConfig( invocation.Method.DeclaringType );

			if (config != null && config.IsMethodCache( invocation.Method ))
			{
				String cacheKey = GetCacheKey( invocation, args );
				object result = _cacheManager[ cacheKey ];

				if (result == null)
				{
					//call target/sub-interceptor
//					Console.WriteLine("calling intercepted method") ;
					result = invocation.Proceed(args);

					//cache method result
//					Console.WriteLine("caching result");
					_cacheManager[ cacheKey ] = result;
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
