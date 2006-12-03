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
	/// Creates <see cref="RescueDescriptor"/> from attributes 
	/// associated with the <see cref="Controller"/>
	/// </summary>
	public class DefaultRescueDescriptorProvider : IRescueDescriptorProvider
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
				logger = loggerFactory.Create(typeof(DefaultRescueDescriptorProvider));
			}
		}

		#endregion

		public RescueDescriptor[] CollectRescues(MemberInfo memberInfo)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collecting rescue information for {0}", memberInfo.Name);
			}
			
			object[] attributes = memberInfo.GetCustomAttributes(typeof(IRescueDescriptorBuilder), true);

			ArrayList descriptors = new ArrayList();

			foreach(IRescueDescriptorBuilder builder in attributes)
			{
				RescueDescriptor[] descs = builder.BuildRescueDescriptors();
				
				if (logger.IsDebugEnabled)
				{
					foreach(RescueDescriptor desc in descs)
					{
						logger.DebugFormat("Collected rescue with view name {0} for exception type {1}", 
						                   desc.ViewName, desc.ExceptionType);
					}
				}
				
				descriptors.AddRange(descs);
			}

			return (RescueDescriptor[]) descriptors.ToArray(typeof(RescueDescriptor));
		}
	}
}
