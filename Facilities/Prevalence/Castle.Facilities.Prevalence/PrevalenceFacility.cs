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

namespace Castle.Facilities.Prevalence
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Bamboo.Prevalence;
	using Bamboo.Prevalence.Util;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.Core.Configuration;

	/// <summary>
	/// Summary description for PrevalenceFacility.
	/// </summary>
	public class PrevalenceFacility : AbstractFacility
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

		public PrevalenceFacility()
		{
		}

		#region IFacility implementation

		protected override void Init()
		{
			RegisterExtensions();
			RegisterEngines();			
		}

		public override void Dispose()
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
			return FacilityConfig.Children["engines"];
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
			Kernel.ComponentModelBuilder.AddContributor( 
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

			Kernel.AddComponentWithExtendedProperties(engineKey, typeof(PrevalenceEngine), properties);

			RegisterSystem(engineKey, systemId, systemType);
		}

		private void ConfigureSnapshot(IConfiguration engineConfig, IDictionary properties)
		{
			float period = GetSnapshotInterval(engineConfig);

			if (RequiresSnapshots(period))
			{
				if (engineConfig.Attributes["cleanupPolicyComponent"] == null)
				{
					Kernel.AddComponentInstance(CleanupPolicyComponentPropertyKey, CleanUpAllFilesPolicy.Default);
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

			Kernel.AddComponentWithExtendedProperties(systemId, systemType, properties);
		}

		protected void TakeSnapshotIfRequired(IConfiguration engineConfig)
		{
			float period = GetSnapshotInterval(engineConfig);

			if (RequiresSnapshots(period))
			{
				PrevalenceEngine engine = (PrevalenceEngine) Kernel[engineConfig.Attributes[IdKey]];

				engine.TakeSnapshot();
			}
		}

		protected void HandsOffFiles(IConfiguration engineConfig)
		{
			PrevalenceEngine engine = (PrevalenceEngine) Kernel[engineConfig.Attributes[IdKey]];

			engine.HandsOffOutputLog();
		}
	}
}
