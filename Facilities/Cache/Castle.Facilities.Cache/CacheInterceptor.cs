using System;
using System.Collections;
using System.Collections.Specialized;

using System.Reflection;
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
			MethodInfo info = invocation.MethodInvocationTarget;
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
