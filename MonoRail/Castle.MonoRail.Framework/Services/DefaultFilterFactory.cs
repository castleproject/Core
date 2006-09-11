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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	
	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// Standard implementation of <see cref="IFilterFactory"/>.
	/// </summary>
	public class DefaultFilterFactory : IServiceEnabledComponent, IFilterFactory
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		#region IServiceEnabledComponent implementation
		
		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultFilterFactory));
			}
		}

		#endregion

		/// <summary>
		/// Creates a filter instance
		/// </summary>
		/// <param name="filterType">The filter's type</param>
		/// <returns>The filter instance</returns>
		public virtual IFilter Create(Type filterType)
		{
			if (filterType == null) throw new ArgumentNullException("filterType");
			
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Creating filter " + filterType.FullName);
			}
			
			try
			{
				return (IFilter) Activator.CreateInstance(filterType);
			}
			catch(Exception ex)
			{
				logger.Error("Could not create filter instance. Activation failed", ex);
				
				throw;
			}
		}

		/// <summary>
		/// Releases a filter instance
		/// </summary>
		/// <param name="filter">The filter instance</param>
		public virtual void Release(IFilter filter)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Releasing filter " + filter);
			}
		}
	}
}
