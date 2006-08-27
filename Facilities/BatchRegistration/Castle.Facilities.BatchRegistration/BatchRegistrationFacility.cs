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

namespace Castle.Facilities.BatchRegistration
{
	using System;
	using System.Configuration;

	using Castle.Core.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;


	public class BatchRegistrationFacility : AbstractFacility
	{
		protected override void Init()
		{
			if (FacilityConfig == null) return;

			foreach(IConfiguration config in FacilityConfig.Children)
			{
				if ("assemblyBatch".Equals(config.Name))
				{
					AddComponents(config);
				}
				else if ("addFacility".Equals(config.Name))
				{
					AddFacility(config);
				}
				else
				{
					throw new ConfigurationException("Invalid node inside facility configuration. " + 
						"Expected assemblyBatch");
				}
			}
		}

		private void AddComponents(IConfiguration config)
		{
			String assemblyName = config.Attributes["name"];

			if (assemblyName == null || assemblyName.Length == 0)
			{
				throw new ConfigurationException("The assemblyBatch node must have a 'name' " + 
					" attribute with the name of the assembly");
			}

			ComponentScanner scanner = new ComponentScanner(assemblyName);

			ConfigureScanner(config, scanner);

			ComponentDefinition[] definitions = scanner.Process();

			foreach(ComponentDefinition definition in definitions)
			{
				if (definition.ServiceType == null)
				{
					Kernel.AddComponent( definition.Key, definition.ClassType );
				}
				else
				{
					Kernel.AddComponent( definition.Key, definition.ServiceType, definition.ClassType );
				}
			}
		}

		private void AddFacility(IConfiguration config)
		{
			String id = config.Attributes["id"];
			String type = config.Attributes["type"];

			if (type == null || type.Length == 0)
			{
				throw new ConfigurationException("The addFacility node must have a 'type' " + 
					" attribute with the Type's name");
			}
			if (id == null || id.Length == 0)
			{
				throw new ConfigurationException("The addFacility node must have a 'id' " + 
					" attribute with facility's key");
			}

			Kernel.AddFacility( id, InstatiateFacility( type ) );
		}

		private IFacility InstatiateFacility(String facilityType)
		{
			Type type = TypeLoadUtil.GetType(facilityType);

			return (IFacility) Activator.CreateInstance( type );
		}

		private void ConfigureScanner(IConfiguration config, ComponentScanner scanner)
		{
			if (config.Attributes["useAttributes"] != null)
			{
				scanner.UseAttributes = config.Attributes["useAttributes"].Equals("true");
			}
	
			foreach(IConfiguration innerConfig in config.Children)
			{
				if ("include".Equals(innerConfig.Name) )
				{
					String key = innerConfig.Attributes["key"];
					String service = innerConfig.Attributes["service"];
					String component = innerConfig.Attributes["component"];

					scanner.AddInclude( key, service, component );
				}
				else if ("exclude".Equals(innerConfig.Name) )
				{
					String type = innerConfig.Attributes["type"];

					scanner.AddExclude( type );
				}
				else
				{
					throw new ConfigurationException("Invalid node inside assemblyBatch " + 
						"configuration. Expected 'include' or 'exclude'");
				}
			}
		}
	}	
}
