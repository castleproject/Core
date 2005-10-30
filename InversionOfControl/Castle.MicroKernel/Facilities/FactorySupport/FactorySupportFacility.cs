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

namespace Castle.Facilities.FactorySupport
{
	using System;

	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;


	public class FactorySupportFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.ComponentModelCreated += new ComponentModelDelegate(Kernel_ComponentModelCreated);
		}

		private void Kernel_ComponentModelCreated(ComponentModel model)
		{
			String instanceAccessor = model.Configuration.Attributes["instance-accessor"];
			String factoryId = model.Configuration.Attributes["factoryId"];
			String factoryCreate = model.Configuration.Attributes["factoryCreate"];

			if ((factoryId != null && factoryCreate == null) || 
				(factoryId == null && factoryCreate != null))
			{
				String message = String.Format("When a factoryId is specified, you must specify " + 
					"the factoryCreate (which is the method to be called) as well - component {0}", model.Name);

				throw new FacilityException(message);
			}

			if (instanceAccessor != null)
			{
				model.ExtendedProperties.Add( "instance.accessor", instanceAccessor );
				model.CustomComponentActivator = typeof(AccessorActivator);
			}
			else if (factoryId != null)
			{
				model.ExtendedProperties.Add( "factoryId", factoryId );
				model.ExtendedProperties.Add( "factoryCreate", factoryCreate );
				model.CustomComponentActivator = typeof(FactoryActivator);
			}
		}
	}
}
