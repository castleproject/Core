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
	using System.Resources;
	using System.Reflection;
	using System.Globalization;
	
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Standard implementation of <see cref="IResourceFactory" />
	/// </summary>
	public class DefaultResourceFactory : IServiceEnabledComponent, IResourceFactory
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
				logger = loggerFactory.Create(typeof(DefaultResourceFactory));
			}
		}

		#endregion

		/// <summary>
		/// Creates an implementation of <see cref="IResource"/>
		/// based on the descriptor.
		/// <seealso cref="ResourceManager"/>
		/// <seealso cref="ResourceFacade"/>
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="appAssembly"></param>
		/// <returns></returns>
		public IResource Create(ResourceDescriptor descriptor, Assembly appAssembly)
		{
			Assembly resAssembly = ResolveAssembly(descriptor.AssemblyName, appAssembly);
			CultureInfo culture = ResolveCulture(descriptor.CultureName);

			if (logger.IsDebugEnabled)
			{
				logger.Debug("Creating resource name {0}, assembly {1}, resource {2}", 
				             descriptor.Name, descriptor.AssemblyName, descriptor.ResourceName);
			}
			
			ResourceManager rm = new ResourceManager(descriptor.ResourceName, 
			                                         resAssembly, descriptor.ResourceType);

			return new ResourceFacade(rm.GetResourceSet(culture, true, true));
		}

		/// <summary>
		/// Releases a resource
		/// </summary>
		/// <param name="resource"></param>
		public void Release(IResource resource)
		{
			resource.Dispose();
		}

		private CultureInfo ResolveCulture(String name)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Resolving culture {0}", name);
			}

			if ("neutral".Equals(name))
			{
				return CultureInfo.InvariantCulture;
			}
			else if (name != null)
			{
				return CultureInfo.CreateSpecificCulture(name);
			}

			return CultureInfo.CurrentCulture;
		}

		private Assembly ResolveAssembly(String name, Assembly assembly)
		{
			if (name == null) return assembly;
			
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Resolving assembly {0}", name);
			}

			try
			{
				return Assembly.Load(name);
			}
			catch(Exception ex)
			{
				logger.Error("Could not load assembly", ex);
				
				throw;
			}
		}
	}
}