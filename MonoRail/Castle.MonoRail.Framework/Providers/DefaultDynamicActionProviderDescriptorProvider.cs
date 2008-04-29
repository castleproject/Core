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

namespace Castle.MonoRail.Framework.Providers
{
	using System;
	using System.Collections;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Creates <see cref="DynamicActionProviderAttribute"/> from attributes 
	/// associated with the <see cref="IController"/>
	/// </summary>
	public class DefaultDynamicActionProviderDescriptorProvider : IDynamicActionProviderDescriptorProvider
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
			ILoggerFactory loggerFactory = (ILoggerFactory)provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultDynamicActionProviderDescriptorProvider));
			}
		}

		#endregion

		/// <summary>
		/// Implementors should collect the dynamic action provider information
		/// and return descriptors instances, or an empty array if none
		/// was found.
		/// </summary>
		/// <param name="controllerType">The controller type</param>
		/// <returns>
		/// An array of <see cref="FilterDescriptor"/>
		/// </returns>
		public DynamicActionProviderDescriptor[] CollectProviders(Type controllerType)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collecting dynamic action providers for {0}", controllerType);
			}
			
			object[] attributes = controllerType.GetCustomAttributes(typeof(IDynamicActionProviderDescriptorBuilder), true);

			ArrayList providers = new ArrayList();
			
			foreach (IDynamicActionProviderDescriptorBuilder builder in attributes)
			{
				DynamicActionProviderDescriptor[] descs = builder.BuildDynamicActionProviderDescriptors();

				if (logger.IsDebugEnabled)
				{
					foreach (DynamicActionProviderDescriptor desc in descs)
					{
						logger.DebugFormat("Collected dynamic action provider {0} ",desc.DynamicActionProviderType);
					}
				}

				providers.AddRange(descs);
			}
			return (DynamicActionProviderDescriptor[])providers.ToArray(typeof(DynamicActionProviderDescriptor));
		}
	}
}
