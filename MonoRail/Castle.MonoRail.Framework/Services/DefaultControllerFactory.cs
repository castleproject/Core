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
	using System.Reflection;
	
	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services.Utils;

	/// <summary>
	/// Standard implementation of <see cref="IControllerFactory"/>.
	/// It inspects assemblies looking for concrete classes
	/// that extend <see cref="Controller"/>.
	/// </summary>
	public class DefaultControllerFactory : AbstractControllerFactory, IInitializable
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		private string[] assemblies;

		public DefaultControllerFactory()
		{
		}

		#region IInitializable implementation
		
		/// <summary>
		/// Invoked by the framework in order to initialize the state
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			
			if (assemblies != null)
			{
				foreach(String assembly in assemblies)
				{
					Inspect(assembly);
				}
			}
			
			assemblies = null;
		}
		
		#endregion

		public override void Service(IServiceProvider provider)
		{
			base.Service(provider);
			
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(AbstractControllerFactory));
			}
			
			MonoRailConfiguration config = (MonoRailConfiguration) provider.GetService(typeof(MonoRailConfiguration));
			
			if (config != null)
			{
				assemblies = config.ControllersConfig.Assemblies;
			}
		}

		/// <summary>
		/// Loads the assembly and inspect its public types.
		/// </summary>
		/// <param name="assemblyFileName"></param>
		public void Inspect(String assemblyFileName)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Inspecting assembly {0}", assemblyFileName);
			}
			
			Assembly assembly = Assembly.Load( assemblyFileName );

			Inspect(assembly);
		}

		/// <summary>
		/// Inspect the assembly's public types.
		/// </summary>
		public void Inspect(Assembly assembly)
		{
			Type[] types = assembly.GetExportedTypes();

			foreach(Type type in types)
			{
				if (!type.IsPublic || type.IsAbstract || type.IsInterface || type.IsValueType)
				{
					continue;
				}

				if (typeof(Controller).IsAssignableFrom(type))
				{
					ControllerDescriptor contrDesc = ControllerInspectionUtil.Inspect(type);
					
					if (logger.IsDebugEnabled)
					{
						logger.Debug("Registering controller descriptor for Area: {0} Name: {1}", contrDesc.Area, contrDesc.Name);
					}
					
					RegisterController(contrDesc);
				}
			}
		}

		private void RegisterController(ControllerDescriptor descriptor)
		{
			Tree.AddController(descriptor.Area, descriptor.Name, descriptor.ControllerType);
		}
	}
}
