
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

namespace Castle.Facilities.TypedFactory
{
	using System;
	using System.Configuration;
	
	using Castle.Core;
	using Castle.Core.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Summary description for TypedFactoryFacility.
	/// </summary>
	public class TypedFactoryFacility : AbstractFacility
	{
		public void AddTypedFactoryEntry( FactoryEntry entry )
		{
			ComponentModel model = 
				new ComponentModel(entry.Id, entry.FactoryInterface, typeof(Empty));
			
			model.LifestyleType = LifestyleType.Singleton;
			model.ExtendedProperties["typed.fac.entry"] = entry;
			model.Interceptors.Add( new InterceptorReference( typeof(FactoryInterceptor) ) );

			Kernel.AddCustomComponent( model );
		}

		protected override void Init()
		{
			Kernel.AddComponent( "typed.fac.interceptor", typeof(FactoryInterceptor) );

			ITypeConverter converter = (ITypeConverter)
				Kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );

			AddFactories(FacilityConfig, converter);
		}

		protected virtual void AddFactories(IConfiguration facilityConfig, ITypeConverter converter)
		{
			if (facilityConfig != null)
			{
				foreach(IConfiguration config in facilityConfig.Children["factories"].Children)
				{
					String id = config.Attributes["id"];
					String creation = config.Attributes["creation"];
					String destruction = config.Attributes["destruction"];

					Type factoryType = (Type)
						converter.PerformConversion( config.Attributes["interface"], typeof(Type) );

					try
					{
						AddTypedFactoryEntry( 
							new FactoryEntry(id, factoryType, creation, destruction) );
					}
					catch(Exception ex)
					{
						throw new ConfigurationException("Invalid factory entry in configuration", ex);
					}
				}
			}
		}
	}
}
