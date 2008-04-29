// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	/// Standard implementation of <see cref="IDynamicActionProviderFactory"/>.
	/// </summary>
	public class DefaultDynamicActionProviderFactory : IServiceEnabledComponent,IDynamicActionProviderFactory
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service provider.</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory)provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultDynamicActionProviderFactory));
			}
		}

		/// <summary>
		/// Creates the specified dynamic action provider type.
		/// </summary>
		/// <param name="dynamicActionProviderType">Type of the dynamic action provider.</param>
		/// <returns></returns>
		public virtual IDynamicActionProvider Create(Type dynamicActionProviderType)
		{
			if (dynamicActionProviderType == null) throw new ArgumentNullException("dynamicActionProviderType");

			if (logger.IsDebugEnabled)
			{
				logger.Debug("Creating dynamic action provider " + dynamicActionProviderType.FullName);
			}

			try
			{
				return (IDynamicActionProvider)Activator.CreateInstance(dynamicActionProviderType);
			}
			catch (Exception ex)
			{
				logger.Error("Could not create dynamic action provider instance. Activation failed", ex);

				throw;
			}
		}

		/// <summary>
		/// Releases the specified dynamic action provider.
		/// </summary>
		/// <param name="dynamicActionProvider">The dynamic action provider.</param>
		public virtual void Release(IDynamicActionProvider dynamicActionProvider)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Releasing dynamicActionProvider " + dynamicActionProvider);
			}
		}
	}
}
