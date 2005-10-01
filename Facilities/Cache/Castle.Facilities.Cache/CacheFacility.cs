using System;
using System.Collections;
using System.Collections.Specialized;
using Castle.Facilities.Cache.Manager;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model;
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
						cacheManagerType = typeof(Castle.Facilities.Cache.Manager.MemoryCacheManager);
					}					
				}
				else
				{
					cacheManagerType = typeof(Castle.Facilities.Cache.Manager.MemoryCacheManager);
				}

			}
			
			ICacheManager cacheManager = (ICacheManager)Activator.CreateInstance(cacheManagerType);
			cacheManager.Configure(properties);
			Kernel.AddComponent("cache.CacheManager", typeof(ICacheManager), cacheManagerType);
		}

	}
}
