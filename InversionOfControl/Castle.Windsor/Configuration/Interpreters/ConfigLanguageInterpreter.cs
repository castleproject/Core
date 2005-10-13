// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Configuration;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;
	using Castle.Model.Resource;
	using Castle.Windsor.Configuration.Interpreters.CastleLanguage;

	/// <summary>
	/// 
	/// </summary>
	public class ConfigLanguageInterpreter : AbstractInterpreter
	{
		public ConfigLanguageInterpreter()
		{
		}

		public ConfigLanguageInterpreter(String filename) : base(filename)
		{
		}

		public ConfigLanguageInterpreter(Castle.Model.Resource.IResource source) : base(source)
		{
		}

		public override void ProcessResource(IResource resource, IConfigurationStore store)
		{
			using (Source)
			{
				WindsorConfLanguageLexer lexer = new WindsorConfLanguageLexer( Source.GetStreamReader() );

				WindsorParser parser = new WindsorParser(new IndentTokenStream(lexer));

				ConfigurationDefinition confDef = parser.start();

				Imports = confDef.Imports;

				IConfiguration container = confDef.Root.Children["container"];

				if (container == null)
				{
					throw new ConfigurationException("Root node 'container' not found.");
				}

				foreach(IConfiguration node in container.Children)
				{
					if (FacilitiesNodeName.Equals(node.Name))
					{
						AddFacilities(node.Children, store);
					}
					else if (ComponentsNodeName.Equals(node.Name))
					{
						AddComponents(node.Children, store);
					}
					else
					{
						String message = String.Format("Unexpected node {0}. We were expecting either {1} or {2}", 
							node.Name, FacilitiesNodeName, ComponentsNodeName);
						throw new ConfigurationException(message);
					}
				}
			}
		}

		private void AddComponents(ConfigurationCollection components, IConfigurationStore store)
		{
			foreach(IConfiguration component in components)
			{
				if (!ComponentNodeName.Equals(component.Name))
				{
					String message = String.Format("Unexpected node {0}. We were expecting {1}", 
						component.Name, ComponentNodeName);
					throw new ConfigurationException(message);
				}

				AddComponentConfig(component, store);
			}
		}

		private void AddFacilities(ConfigurationCollection facilities, IConfigurationStore store)
		{
			foreach(IConfiguration facility in facilities)
			{
				if (!FacilityNodeName.Equals(facility.Name))
				{
					String message = String.Format("Unexpected node {0}. We were expecting {1}", 
						facility.Name, FacilityNodeName);
					throw new ConfigurationException(message);
				}

				AddFacilityConfig(facility, store);
			}
		}
	}
}
