// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.Prevalence
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Bamboo.Prevalence;

	using Castle.MicroKernel;

	using Castle.Model.Configuration;

	/// <summary>
	/// Summary description for PrevalenceFacility.
	/// </summary>
	public class PrevalenceFacility : IFacility
	{
		public static readonly String SystemTypePropertyKey = "prevalence.systemtype";
		public static readonly String StorageDirPropertyKey = "prevalence.storagedir";
		public static readonly String AutoMigrationPropertyKey = "prevalence.autoversionmigration";
		public static readonly String EngineIdPropertyKey = "prevalence.engineid";
		public static readonly String ResetStoragePropertyKey = "prevalence.resetStorage";

		private IKernel kernel;

		public PrevalenceFacility()
		{
		}

		#region IFacility implementation

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			this.kernel = kernel;

			RegisterExtensions();
			RegisterEngines(facilityConfig);			
		}

		public void Terminate()
		{
		}

		#endregion

		protected void RegisterEngines(IConfiguration facilityConfig)
		{
			IConfiguration engines = facilityConfig.Children["engines"];
			
			foreach(IConfiguration engine in engines.Children)
			{
				RegisterEngine(engine);
			}
		}

		protected void RegisterExtensions()
		{
			kernel.ComponentModelBuilder.AddContributor( 
				new PrevalenceActivatorOverriderModelInspector() );
		}

		protected void RegisterEngine(IConfiguration engineConfig)
		{
			String engineKey = engineConfig.Attributes["id"];
			String systemId = engineConfig.Attributes["systemId"];
			String resetStorage = engineConfig.Attributes["resetStorage"];

			Type systemType = ObtainSystemType(engineConfig);
			bool autoVersion = Convert.ToBoolean( engineConfig.Attributes["autoVersionMigration"] );

			if (resetStorage == null) resetStorage = "false";

			IDictionary properties = new HybridDictionary(true);

			properties.Add(SystemTypePropertyKey, systemType);
			properties.Add(AutoMigrationPropertyKey, autoVersion);
			properties.Add(ResetStoragePropertyKey, Convert.ToBoolean(resetStorage) );
			properties.Add(StorageDirPropertyKey, engineConfig.Attributes["storageDir"]);

			kernel.AddComponentWithProperties(engineKey, typeof(PrevalenceEngine), properties);

			RegisterSystem(engineKey, systemId, systemType);
		}

		protected Type ObtainSystemType(IConfiguration config)
		{
			String systemTypeName = config.Attributes["systemType"];

			Type systemType = null;

			systemType = Type.GetType(systemTypeName, false, false);

			if (systemType == null)
			{
				String message = String.Format(
					"Could not obtain type for prevalent system '{0}'", systemTypeName);

				throw new KernelException(message);
			}

			return systemType;
		}

		protected void RegisterSystem(String engineKey, String systemId, Type systemType)
		{
			IDictionary properties = new HybridDictionary(true);

			properties[EngineIdPropertyKey] = engineKey;

			kernel.AddComponentWithProperties(systemId, systemType, properties);
		}
	}
}
