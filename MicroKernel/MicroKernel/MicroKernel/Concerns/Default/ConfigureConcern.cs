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

namespace Castle.MicroKernel.Concerns.Default
{
	using System;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// Summary description for ConfigureConcern.
	/// </summary>
	public class ConfigureConcern : AbstractConcern, ICommissionConcern
	{
		public ConfigureConcern(IConcern next) : base(next)
		{
		}

		public override void Apply(IComponentModel model, object component)
		{
			IConfiguration configuration = 
				model.Configuration.GetChild("configuration", true);

			ContainerUtil.Configure( component, configuration );

			base.Apply( model, component );
		}
	}
}
