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

using System.Text.RegularExpressions;

namespace Castle.Facilities.NHibernateIntegration.Builders
{
	using System.Collections.Generic;
	using Core.Configuration;
	using log4net;
	using NHibernate.Cfg;
	using Persisters;

	/// <summary>
	/// Serializes the Configuration for subsequent initializations.
	/// </summary>
	public class PersistentConfigurationBuilder : DefaultConfigurationBuilder
	{
		private const string DEFAULT_EXTENSION = "dat";

		private static readonly ILog log = LogManager.GetLogger(typeof(PersistentConfigurationBuilder));

		private readonly IConfigurationPersister configurationPersister;

		/// <summary>
		/// Initializes the presistent <see cref="Configuration"/> builder
		/// with an specific <see cref="IConfigurationPersister"/>
		/// </summary>
		public PersistentConfigurationBuilder(IConfigurationPersister configurationPersister)
		{
			this.configurationPersister = configurationPersister;
		}

		/// <summary>
		/// Initializes the presistent <see cref="Configuration"/> builder
		/// using the default <see cref="IConfigurationPersister"/>
		/// </summary>
		public PersistentConfigurationBuilder()
			: this(new DefaultConfigurationPersister())
		{
		}

		/// <summary>
		/// Returns the Deserialized Configuration
		/// </summary>
		/// <param name="config">The configuration node.</param>
		/// <returns>NHibernate Configuration</returns>
		public override Configuration GetConfiguration(IConfiguration config)
		{
			log.Debug("Building the Configuration");

			string filename = this.GetFilenameFrom(config);
			IList<string> dependentFilenames = this.GetDependentFilenamesFrom(config);

			Configuration cfg;
			if (this.configurationPersister.IsNewConfigurationRequired(filename, dependentFilenames))
			{
				log.Debug("Configuration is either old or some of the dependencies have changed");
				cfg = base.GetConfiguration(config);
				this.configurationPersister.WriteConfiguration(filename, cfg);
			}
			else
			{
				cfg = this.configurationPersister.ReadConfiguration(filename);
			}
			return cfg;
		}

		private string GetFilenameFrom(IConfiguration config)
		{
			var filename = config.Attributes["fileName"] ?? config.Attributes["id"] + "." + DEFAULT_EXTENSION;
			return this.StripInvalidCharacters(filename);
		}

		private string StripInvalidCharacters(string input)
		{
			return Regex.Replace(input, "[:*?\"<>\\\\/]", "", RegexOptions.IgnoreCase);
		}

		private IList<string> GetDependentFilenamesFrom(IConfiguration config)
		{
			IList<string> list = new List<string>();

			IConfiguration assemblies = config.Children["assemblies"];
			if (assemblies != null)
			{
				foreach (var assembly in assemblies.Children)
				{
					list.Add(assembly.Value + ".dll");
				}
			}
			
			IConfiguration dependsOn = config.Children["dependsOn"];
			if (dependsOn != null)
			{
				foreach (var on in dependsOn.Children)
				{
					list.Add(on.Value);
				}
			}

			return list;
		}
	}
}