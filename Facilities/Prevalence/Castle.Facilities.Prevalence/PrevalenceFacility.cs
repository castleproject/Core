using Bamboo.Prevalence.Util;
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
		private static readonly String IdKey = "id";
		private static readonly String IntervalKey = "snapshotIntervalInHours";

		public static readonly String SystemTypePropertyKey = "prevalence.systemtype";
		public static readonly String StorageDirPropertyKey = "prevalence.storagedir";
		public static readonly String AutoMigrationPropertyKey = "prevalence.autoversionmigration";
		public static readonly String EngineIdPropertyKey = "prevalence.engineid";
		public static readonly String ResetStoragePropertyKey = "prevalence.resetStorage";
		public static readonly String SnapshotPeriodPropertyKey = "prevalence.snapshotPeriod";
		
		public static readonly String CleanupPolicyComponentPropertyKey = "prevalence.cleanupPolicyComponent";
		public static readonly String SnapShotTakerComponentPropertyKey = "prevalence.snapshot.taker";

		private IKernel _kernel;
		private IConfiguration _facilityConfig;

		public PrevalenceFacility()
		{
		}

		#region IFacility implementation

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			_kernel = kernel;
			_facilityConfig = facilityConfig;


			RegisterExtensions();
			RegisterEngines();			
		}

		public void Terminate()
		{
			IConfiguration engines = GetEnginesConfig();
			
			foreach(IConfiguration engine in engines.Children)
			{
				TakeSnapshotIfRequired(engine);
				HandsOffFiles(engine);
			}
		}

		#endregion

		private IConfiguration GetEnginesConfig()
		{
			return _facilityConfig.Children["engines"];
		}

		protected void RegisterEngines()
		{
			IConfiguration engines = GetEnginesConfig();
			
			foreach(IConfiguration engine in engines.Children)
			{
				RegisterEngine(engine);
			}
		}

		protected void RegisterExtensions()
		{
			_kernel.ComponentModelBuilder.AddContributor( 
				new PrevalenceActivatorOverriderModelInspector() );
		}

		protected void RegisterEngine(IConfiguration engineConfig)
		{
			String engineKey = engineConfig.Attributes[IdKey];
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
			
			ConfigureSnapshot(engineConfig, properties);

			_kernel.AddComponentWithProperties(engineKey, typeof(PrevalenceEngine), properties);

			RegisterSystem(engineKey, systemId, systemType);
		}

		private void ConfigureSnapshot(IConfiguration engineConfig, IDictionary properties)
		{
			float period = GetSnapshotInterval(engineConfig);

			if (RequiresSnapshots(period))
			{
				if (engineConfig.Attributes["cleanupPolicyComponent"] == null)
				{
					_kernel.AddComponentInstance(CleanupPolicyComponentPropertyKey, CleanUpAllFilesPolicy.Default);
				}

				properties.Add(SnapshotPeriodPropertyKey, period);
			}
			else
			{
				properties.Add(SnapshotPeriodPropertyKey, 0f);
			}
		}

		private float GetSnapshotInterval(IConfiguration engineConfig)
		{
			try
			{
				return Convert.ToSingle(engineConfig.Attributes[IntervalKey]);
			}
			catch (FormatException e)
			{
				throw new KernelException("Invalid snapshotHoursPeriod.", e);
			}
		}

		private bool RequiresSnapshots(float period)
		{
			return period > 0;
		}

		protected Type ObtainSystemType(IConfiguration config)
		{
			String systemTypeName = config.Attributes["systemType"];

			Type systemType = null;

			systemType = Type.GetType(systemTypeName, false, false);

			if (systemType == null)
			{
				String message = String.Format(
					"Could not obtain type for prevalent system '{0}'.", systemTypeName);

				throw new KernelException(message);
			}

			return systemType;
		}

		protected void RegisterSystem(String engineKey, String systemId, Type systemType)
		{
			IDictionary properties = new HybridDictionary(true);

			properties[EngineIdPropertyKey] = engineKey;

			_kernel.AddComponentWithProperties(systemId, systemType, properties);
		}

		protected void TakeSnapshotIfRequired(IConfiguration engineConfig)
		{
			float period = GetSnapshotInterval(engineConfig);

			if (RequiresSnapshots(period))
			{
				PrevalenceEngine engine = (PrevalenceEngine) _kernel[engineConfig.Attributes[IdKey]];

				engine.TakeSnapshot();
			}
		}

		protected void HandsOffFiles(IConfiguration engineConfig)
		{
			PrevalenceEngine engine = (PrevalenceEngine) _kernel[engineConfig.Attributes[IdKey]];

			engine.HandsOffOutputLog();
		}
	}
}
