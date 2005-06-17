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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;

	using Castle.Model.Configuration;

	/// <summary>
	/// Uses the ConfigurationStore registered in the kernel to obtain
	/// an <see cref="IConfiguration"/> associated with the component.
	/// </summary>
	[Serializable]
	public class ConfigurationModelInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Queries the kernel's ConfigurationStore for a configuration
		/// associated with the component name.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, Castle.Model.ComponentModel model)
		{
			model.Configuration = 
				kernel.ConfigurationStore.GetComponentConfiguration(model.Name);
		}
	}
}
