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
	using System;
	using System.Collections;
	using System.Reflection;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Creates <see cref="RescueDescriptor"/> from attributes 
	/// associated with the <see cref="IController"/>
	/// </summary>
	public class DefaultRescueDescriptorProvider : IRescueDescriptorProvider
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
				logger = loggerFactory.Create(typeof(DefaultRescueDescriptorProvider));
			}
		}

		#endregion

		/// <summary>
		/// Collects the rescues.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public RescueDescriptor[] CollectRescues(Type type)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collecting rescue information for {0}", type.Name);
			}

			ArrayList descriptors = new ArrayList();

			while(type != typeof(object))
			{
				object[] attributes = type.GetCustomAttributes(typeof(IRescueDescriptorBuilder), false);

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

				type = type.BaseType;
			}

			return (RescueDescriptor[]) descriptors.ToArray(typeof(RescueDescriptor));
		}

		/// <summary>
		/// Implementors should collect the rescue information
		/// and return descriptors instances, or an empty array if none
		/// was found.
		/// </summary>
		/// <param name="memberInfo">The action (MethodInfo)</param>
		/// <returns>
		/// An array of <see cref="RescueDescriptor"/>
		/// </returns>
		public RescueDescriptor[] CollectRescues(MethodInfo memberInfo)
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