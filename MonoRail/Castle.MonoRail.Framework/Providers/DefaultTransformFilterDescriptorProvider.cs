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

namespace Castle.MonoRail.Framework.Providers
{
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Creates <see cref="TransformFilterDescriptor"/> from attributes 
	/// associated with the <see cref="IController"/>
	/// </summary>
	public class DefaultTransformFilterDescriptorProvider : ITransformFilterDescriptorProvider
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
				logger = loggerFactory.Create(typeof(DefaultFilterDescriptorProvider));
			}
		}

		#endregion

		/// <summary>
		/// Implementors should collect the transformfilter information
		/// and return descriptors instances, or an empty array if none
		/// was found.
		/// </summary>
		/// <param name="methodInfo">The action (MethodInfo)</param>
		/// <returns>
		/// An array of <see cref="TransformFilterDescriptor"/>
		/// </returns>
		public TransformFilterDescriptor[] CollectFilters(MethodInfo methodInfo)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collecting transform filters for {0}", methodInfo.Name);
			}

			object[] attributes = methodInfo.GetCustomAttributes(typeof(ITransformFilterDescriptorBuilder), true);

			List<TransformFilterDescriptor> filters = new List<TransformFilterDescriptor>();

			foreach(ITransformFilterDescriptorBuilder builder in attributes)
			{
				TransformFilterDescriptor[] descs = builder.BuildTransformFilterDescriptors();

				if (logger.IsDebugEnabled)
				{
					foreach(TransformFilterDescriptor desc in descs)
					{
						logger.DebugFormat("Collected transform filter {0} to execute in order {1}", desc.TransformFilterType, desc.ExecutionOrder);
					}
				}

				filters.AddRange(descs);
			}

			return filters.ToArray();
		}
	}
}