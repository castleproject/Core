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

namespace ExtendingSample2.Facilities
{
	using System;
	using System.Reflection;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.ModelBuilder;

	public class NonOptionalPropertiesFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.ComponentModelBuilder.AddContributor(new NonOptionalInspector());
		}
	}
	
	public class NonOptionalInspector : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			PropertyInfo[] props = model.Implementation.GetProperties(
				BindingFlags.Public | BindingFlags.Instance);
			
			foreach(PropertyInfo prop in props)
			{
				if (prop.IsDefined(typeof(NonOptionalAttribute), false))
				{
					PropertySet propSet = model.Properties.FindByPropertyInfo(prop);
					
					if (propSet == null) continue;
					
					propSet.Dependency.IsOptional = false;
				}
			}
		}
	}
}
