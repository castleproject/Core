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

	using Bamboo.Prevalence;

	using Castle.Model;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;

	/// <summary>
	/// Summary description for PrevalenceEngineComponentActivator.
	/// </summary>
	public class PrevalenceEngineComponentActivator : AbstractComponentActivator
	{
		public PrevalenceEngineComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object InternalCreate()
		{
			Type systemType = (Type) 
				Model.ExtendedProperties[PrevalenceFacility.SystemTypePropertyKey];
			String dir = (String) 
				Model.ExtendedProperties[PrevalenceFacility.StorageDirPropertyKey];
			bool autoVersionMigration = (bool) 
				Model.ExtendedProperties[PrevalenceFacility.AutoMigrationPropertyKey];
			
			return 
				PrevalenceActivator.CreateEngine( 
					systemType, 
					dir, 
					Convert.ToBoolean(autoVersionMigration) );
		}

		protected override void InternalDestroy(object instance)
		{
		}
	}
}
