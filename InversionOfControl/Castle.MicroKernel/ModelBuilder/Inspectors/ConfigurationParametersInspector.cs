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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;

	using Castle.Model;
	using Castle.Model.Configuration;

	/// <summary>
	/// Check for a node 'parameters' within the component 
	/// configuration. For each child it, a ParameterModel is created
	/// and added to ComponentModel's Parameters collection
	/// </summary>
	public class ConfigurationParametersInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Inspect the configuration associated with the component
		/// and populates the parameter model collection accordingly
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null) return;

			IConfiguration parameters = model.Configuration.Children["parameters"];

			if (parameters == null) return;

			foreach(IConfiguration parameter in parameters.Children)
			{
				String name = parameter.Name;
				String value = parameter.Value;

				if (value == null && parameter.Children.Count != 0)
				{
					IConfiguration parameterValue = parameter.Children[0];
					model.Parameters.Add( name, parameterValue );
				}
				else
				{
					model.Parameters.Add( name, value );
				}
			}
		}
	}
}
