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

namespace Castle.Facilities.ManagedExtensions.Server
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.SubSystems.Conversion;

	using Castle.ManagementExtensions;
	using Castle.ManagementExtensions.Default;

	/// <summary>
	/// Summary description for ManagementExtensionModelServerInspector.
	/// </summary>
	public class ManagementExtensionModelServerInspector : IContributeComponentModelConstruction
	{
		IKernel _kernel;

		public ManagementExtensionModelServerInspector()
		{
		}

		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			_kernel = kernel;

			if (IsManagedComponent(model))
			{
				model.ExtendedProperties.Add(
					ManagementConstants.ComponentIsNaturalManageable, true);
			}
			else if (HasManageableConfigAttribute(model))
			{
				model.ExtendedProperties.Add(
					ManagementConstants.ComponentIsNonNaturalManageable, true);
			}
		}

		private bool IsManagedComponent(ComponentModel model)
		{
			ComponentType type = MInspector.Inspect( model.Implementation );
			return (type != ComponentType.None);
		}

		private bool HasManageableConfigAttribute(ComponentModel model)
		{
			if (model.Configuration == null) return false;

			String manageableValue = model.Configuration.Attributes["manageable"];

			if (manageableValue == null) return false;

			ITypeConverter converter = (ITypeConverter) 
				_kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );

			return (bool) 
				converter.PerformConversion( manageableValue, typeof(bool) );
		}
	}
}
