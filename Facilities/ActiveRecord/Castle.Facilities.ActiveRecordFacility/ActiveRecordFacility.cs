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

namespace Castle.Facilities.ActiveRecordFacility
{
	using System.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Model;
	using Castle.Model.Configuration;

	public class ActiveRecordFacility : AbstractFacility
	{
		protected override void Init()
		{
			if( FacilityConfig == null )
			{
				throw new ConfigurationException( "The ActiveRecordFacility requires and external configuration." );
			}

			IConfiguration factoriesConfig = FacilityConfig.Children["factory"];
			
			if( factoriesConfig == null )
			{
				throw new ConfigurationException( "You need at least one factory to use the ActiveRecordFacility." );
			}
			
			ComponentModel model = new ComponentModel( "activerecord", typeof( NHibernateMappingEngine ), typeof( NHibernateMappingEngine ) );
			model.Configuration = factoriesConfig;
			Kernel.AddCustomComponent( model );
		}
	}
}
