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

	using Bamboo.Prevalence;

	using Castle.Model;

	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for PrevalenceSystemComponentActivator.
	/// </summary>
	public class PrevalenceSystemComponentActivator : DefaultComponentActivator
	{
		public PrevalenceSystemComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		/// <summary>
		/// To obtain the system instance, we obtain
		/// the engine id that holds this system and use the
		/// apropriate property
		/// </summary>
		/// <returns></returns>
		protected override object Instantiate()
		{
			String engineId = (String) 
				Model.ExtendedProperties[PrevalenceFacility.EngineIdPropertyKey];
			
			PrevalenceEngine engine = (PrevalenceEngine) Kernel[engineId];

			return engine.PrevalentSystem;
		}
	}
}
