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
using System.Collections;
using System.Reflection;
using Castle.Facilities.Cache.Manager;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Description résumée de CacheConfig.
	/// </summary>
	public class CacheConfig
	{
		private IList _methods = new ArrayList();
		private IList _methodName = new ArrayList();
		private ICacheManager _cacheManager = null;

		public ICacheManager CacheManager
		{
			get { return _cacheManager; }
		}

		public CacheConfig(ICacheManager cacheManager)
		{
			_cacheManager = cacheManager;
		}

		public void AddMethodName(string value)
		{
			_methodName.Add(value);
		}

		public void AddMethod(MethodInfo method)
		{
			_methods.Add(method);
		}
	
		/// <summary>
		/// A 
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public bool IsMethodCache(MethodInfo method)
		{
			if (_methods.Contains(method)) return true;

			foreach(String methodName in _methodName)
			{
				if (method.Name.Equals(methodName))
				{
					return true;
				}
			}

			return false;
		}
	}
}
