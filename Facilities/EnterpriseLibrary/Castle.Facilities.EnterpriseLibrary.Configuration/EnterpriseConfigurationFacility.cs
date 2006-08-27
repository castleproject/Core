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

namespace Castle.Facilities.EnterpriseLibrary.Configuration
{
	using System;
	
	using Castle.Core;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.ComponentActivator;

	using Microsoft.Practices.EnterpriseLibrary.Configuration;


	public class EnterpriseConfigurationFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.ComponentModelBuilder.AddContributor( new EntLibConfigurationInspector() );
		}
	}

	internal class EntLibConfigurationInspector : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null) return;

			String configKey = model.Configuration.Attributes["configurationkey"];

			if (configKey == null) return;

			model.ExtendedProperties["configurationkey"] = configKey;
			model.CustomComponentActivator = typeof(EntLibComponentActivator);
		}
	}

	internal class EntLibComponentActivator : AbstractComponentActivator
	{
		public EntLibComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object InternalCreate()
		{
			String configKey = (String) Model.ExtendedProperties["configurationkey"];

			return ConfigurationManager.GetConfiguration(configKey);
		}

		protected override void InternalDestroy(object instance)
		{
			String configKey = (String) Model.ExtendedProperties["configurationkey"];

			ConfigurationManager.WriteConfiguration(configKey, instance);
		}
	}
}
