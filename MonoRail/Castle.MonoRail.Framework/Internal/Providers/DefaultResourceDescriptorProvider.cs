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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Reflection;
	
	using Castle.Core.Logging;

	/// <summary>
	/// Creates <see cref="ResourceDescriptor"/> from attributes 
	/// associated with the <see cref="Controller"/>
	/// </summary>
	public class DefaultResourceDescriptorProvider : IResourceDescriptorProvider
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
				logger = loggerFactory.Create(typeof(DefaultResourceDescriptorProvider));
			}
		}

		#endregion

		public ResourceDescriptor[] CollectResources(MemberInfo memberInfo)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collecting resources information for {0}", memberInfo.Name);
			}
			
			object[] attributes = memberInfo.GetCustomAttributes(typeof(IResourceDescriptorBuilder), true);

			ArrayList descriptors = new ArrayList();

			foreach(IResourceDescriptorBuilder builder in attributes)
			{
				ResourceDescriptor[] descs = builder.BuildResourceDescriptors();
				
				if (logger.IsDebugEnabled)
				{
					foreach(ResourceDescriptor desc in descs)
					{
						logger.DebugFormat("Collected resource {0} Assembly Name {1} Culture {2} ResName {3} ResType {4}",
						                   desc.Name, desc.AssemblyName, desc.CultureName, desc.ResourceName, desc.ResourceType);
					}
				}
				
				descriptors.AddRange(descs);
			}

			return (ResourceDescriptor[]) descriptors.ToArray(typeof(ResourceDescriptor));
		}
	}
}
