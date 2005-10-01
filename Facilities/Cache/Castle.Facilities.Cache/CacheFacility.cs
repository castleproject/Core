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

using System;
using System.Collections;
using System.Collections.Specialized;
using Castle.Facilities.Cache.Manager;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model.Configuration;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Description résumée de CacheFacility.
	/// </summary>
	public class CacheFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.AddComponent( "cache.interceptor", typeof(CacheInterceptor) );
			Kernel.AddComponent( "cache.configholder", typeof(CacheConfigHolder) );
			Kernel.ComponentModelBuilder.AddContributor( new CacheComponentInspector() );

			ConfigureCacheManager();
		}

		private void ConfigureCacheManager()
		{
			IConversionManager converter = Kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey ) as IConversionManager;
			Type cacheManagerType = null;
			IDictionary properties = new HybridDictionary();

			if (FacilityConfig != null)
			{
				IConfiguration cacheManagertNode = FacilityConfig.Children["cacheManager"];

				if (cacheManagertNode!=null)
				{
					String typeAttribute = cacheManagertNode.Attributes["type"];				

					if (typeAttribute != null)
					{
						cacheManagerType = (Type) converter.PerformConversion( typeAttribute, typeof(Type) );

						foreach(IConfiguration propertyNode in cacheManagertNode.Children)
						{
							properties.Add( propertyNode.Name, propertyNode.Value );
						}
					}
					else
					{
						cacheManagerType = typeof(MemoryCacheManager);
					}					
				}
				else
				{
					cacheManagerType = typeof(MemoryCacheManager);
				}

			}
			
			ICacheManager cacheManager = (ICacheManager)Activator.CreateInstance(cacheManagerType);
			cacheManager.Configure(properties);
			Kernel.AddComponent("cache.CacheManager", typeof(ICacheManager), cacheManagerType);
		}

	}
}
