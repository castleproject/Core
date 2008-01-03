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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Web;
	
	using Castle.Core.Logging;

	/// <summary>
	/// Simple implementation of <see cref="ICacheProvider"/>
	/// that relies on ASP.Net Cache
	/// </summary>
	public class DefaultCacheProvider : ICacheProvider
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;
		
		#region IMRServiceEnabled implementation
		
		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IMonoRailServices provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultCacheProvider));
			}
		}

		#endregion

		/// <summary>
		/// Determines whether the specified key is on the cache.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the cache has the key; otherwise, <c>false</c>.
		/// </returns>
		public bool HasKey(String key)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Checking for entry existence with key {0}", key);
			}
			
			return Get(key) != null;
		}

		/// <summary>
		/// Gets the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public object Get(String key)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Getting entry with key {0}", key);
			}

			return GetCurrentContext().Cache.Get(key);
		}

		/// <summary>
		/// Stores the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="data">The data.</param>
		public void Store(String key, object data)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Storing entry {0} with data {1}", key, data);
			}

			GetCurrentContext().Cache.Insert(key, data);
		}

		/// <summary>
		/// Deletes the cache item by the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public void Delete(String key)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Deleting entry with key {0}", key);
			}

			GetCurrentContext().Cache.Remove(key);
		}

		/// <summary>
		/// Gets the current context.
		/// </summary>
		/// <returns></returns>
		private static HttpContext GetCurrentContext()
		{
			return HttpContext.Current;
		}
	}
}
