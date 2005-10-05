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

using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Castle.Facilities.Cache.Manager;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Summary description for CacheConfig.
	/// </summary>
	public class CacheConfig
	{
		private IDictionary _cacheManagerByMethod = new HybridDictionary();
		private IDictionary _cacheManagerByMethodName = new HybridDictionary();
		private IKernel _kernel = null;

		private string _globalCacheManagerId = null;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="globalCacheManagerId"></param>
		/// <remarks>The globalCacheManagerId us only used when configure by attribute</remarks>
		public CacheConfig(IKernel kernel, string globalCacheManagerId)
		{
			_globalCacheManagerId = globalCacheManagerId;
			_kernel = kernel;
		}

		public ICacheManager GetCacheManager(MethodInfo method)
		{
			string cacheManagerId = string.Empty;

			if (_cacheManagerByMethod.Contains(method))
			{
				cacheManagerId = _cacheManagerByMethod[method] as string;
			}
			else if (_cacheManagerByMethodName.Contains(method.Name))
			{
				cacheManagerId = _cacheManagerByMethodName[method.Name] as string;
			}

			return (ICacheManager)_kernel[cacheManagerId];
		}

		public void AddMethod(MethodInfo method, string cacheManagerId)
		{
			if ( cacheManagerId == string.Empty )
			{
				if (_globalCacheManagerId != string.Empty )
				{
					_cacheManagerByMethod.Add(method, _globalCacheManagerId);
				}
				else
				{
					throw new FacilityException("You must specify a cache manager id on the cache class attribute or on the method attribute."); 
				}
			}
			else
			{
				_cacheManagerByMethod.Add(method, cacheManagerId);	
			}
		}

		public void AddMethodName(string methodName, string cacheManagerId)
		{
			_cacheManagerByMethodName.Add(methodName, cacheManagerId);
		}

		/// <summary>
		/// A 
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public bool IsMethodCache(MethodInfo method)
		{
			if (_cacheManagerByMethod.Contains(method)) return true;
			if (_cacheManagerByMethodName.Contains(method.Name)) return true;

			return false;
		}
	}
}
