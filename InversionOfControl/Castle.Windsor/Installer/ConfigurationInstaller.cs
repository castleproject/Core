// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Installer
{
	using System;
	using Castle.MicroKernel;
	using Castle.Windsor.Configuration;

	/// <summary>
	/// Delegate to provide environment name.
	/// </summary>
	/// <returns>The environment name.</returns>
	public delegate String EnvironmentDelegate();
	
	public class ConfigurationInstaller : IWindsorInstaller
	{
		private readonly IConfigurationInterpreter interpreter;
		private EnvironmentDelegate environment;
		
		/// <summary>
		/// Initializes a new instance of the ConfigurationInstaller class.
		/// </summary>
		public ConfigurationInstaller(IConfigurationInterpreter interpreter)
		{
			if (interpreter == null)
			{
				throw new ArgumentNullException("interpreter");
			}
			this.interpreter = interpreter;
		}
		
		/// <summary>
		/// Sets the configuration environment name.
		/// </summary>
		/// <param name="environmentName">The environment name.</param>
		/// <returns></returns>
		public ConfigurationInstaller Environment(String environmentName)
		{
			return Environment(delegate { return environmentName; });
		}
		
		/// <summary>
		/// Set the configuration environment strategy.
		/// </summary>
		/// <param name="environment">The environment strategy.</param>
		/// <returns></returns>
		public ConfigurationInstaller Environment(EnvironmentDelegate environment)
		{
			this.environment = environment;
			return this;
		}

		void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
		{			
			if (environment != null)
			{
				interpreter.EnvironmentName = environment();
			}

			interpreter.ProcessResource(interpreter.Source, store);
		}
	}
}
