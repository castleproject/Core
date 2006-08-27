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
	using System.IO;

	using Bamboo.Prevalence;
	using Bamboo.Prevalence.Util;

	using Castle.Core;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;

	/// <summary>
	/// Summary description for PrevalenceEngineComponentActivator.
	/// </summary>
	public class PrevalenceEngineComponentActivator : DefaultComponentActivator
	{
		public PrevalenceEngineComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate(Castle.MicroKernel.CreationContext context)
		{
			Type systemType = (Type) 
				Model.ExtendedProperties[PrevalenceFacility.SystemTypePropertyKey];
			String dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (String) 
				Model.ExtendedProperties[PrevalenceFacility.StorageDirPropertyKey]);
			bool autoVersionMigration = (bool) 
				Model.ExtendedProperties[PrevalenceFacility.AutoMigrationPropertyKey];
			bool resetStorage = (bool) 
				Model.ExtendedProperties[PrevalenceFacility.ResetStoragePropertyKey];
			float snapshotPeriod = (float)Model.ExtendedProperties[PrevalenceFacility.SnapshotPeriodPropertyKey];
			
			if (resetStorage)
			{
				DeleteStorageDir(dir);
			}

			PrevalenceEngine engine = PrevalenceActivator.CreateEngine( 
				systemType, 
				dir, 
				autoVersionMigration );

			if (snapshotPeriod > 0)
			{
				CreateSnapshotTaker(engine, snapshotPeriod);
			}

			return engine;
		}

		private void CreateSnapshotTaker(PrevalenceEngine engine, float snapshotPeriod)
		{
			TimeSpan period = TimeSpan.FromHours(snapshotPeriod);
			ICleanUpPolicy policy = (ICleanUpPolicy) Kernel[PrevalenceFacility.CleanupPolicyComponentPropertyKey];

			SnapshotTaker taker = new SnapshotTaker(engine, period, policy);

			Kernel.AddComponentInstance(PrevalenceFacility.SnapShotTakerComponentPropertyKey, taker);
		}

		private void DeleteStorageDir(String dir)
		{
			DirectoryInfo di = new DirectoryInfo(dir);
			
			if (di.Exists)
			{
				di.Delete(true);
			}
		}

		public static void Kernel_ComponentDestroyed(ComponentModel model, object instance)
		{
			if (instance is PrevalenceEngine)
			{
				PrevalenceEngine engine = (PrevalenceEngine) instance;

				engine.TakeSnapshot();
			}
		}
	}
}
