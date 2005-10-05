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
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model;
using Castle.Model.Configuration;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Summary description for CacheComponentInspector.
	/// </summary>
	/// <example>
	///		<component id="ServiceA"
	///			 cache="true"
	///		     service="TestConfig.IServiceA, TestConfig" 
	///		     type="TestConfig.ServiceA, TestConfig">
	///		  <cache ref="CacheManager">
	///		    <method>MyMethod</method>
	///		  </cache>
	//		</component>
	/// </example>
	public class CacheComponentInspector : IContributeComponentModelConstruction
	{
		#region IContributeComponentModelConstruction Members

		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			CacheConfigHolder cacheConfigHolder = null;
			bool allowModelCache = IsCacheModelOn(kernel, model);

			if (allowModelCache)
			{
				model.Dependencies.Add( new DependencyModel( DependencyType.Service, null, typeof(CacheInterceptor), false ) );
				model.Interceptors.Add( new InterceptorReference(typeof(CacheInterceptor)) );

				cacheConfigHolder = kernel[ typeof(CacheConfigHolder) ] as CacheConfigHolder;

				if (IsCacheModelOn(kernel, model))
				{
					CacheConfig config = CreateCacheConfig(kernel, model);
					cacheConfigHolder.Register( model.Implementation, config);
				}
			}		
		}


		private bool IsCacheModelOn(IKernel kernel, ComponentModel model)
		{
			IConversionManager converter = kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey ) as IConversionManager;
			bool allowCache = false;

			if (model.Configuration != null)
			{
				String enableInterceptionAttribute = model.Configuration.Attributes["Cache"];
				
				if (enableInterceptionAttribute != null)
				{
					allowCache = (bool) converter.PerformConversion( enableInterceptionAttribute, typeof(bool) );
				}
			}

			if ( model.Implementation.IsDefined( typeof(CacheAttribute), true ) )
			{
				allowCache = true;
			}

			return allowCache;
		}


		private string GetCacheManagerId(ComponentModel model)
		{
			string cacheManagerId = string.Empty;

			if (model.Configuration != null)
			{
				IConfiguration cacheNode = model.Configuration.Children["cache"];

				if (cacheNode != null)
				{
					cacheManagerId = cacheNode.Attributes["ref"];
				}
			}
			else
			{
				if ( model.Implementation.IsDefined( typeof(CacheAttribute), true ) )
				{
					CacheAttribute[] attributs = 
						model.Implementation.GetCustomAttributes(typeof(CacheAttribute), true) as CacheAttribute[];
					cacheManagerId = attributs[0].CacheManagerId;
				}
			}

			return cacheManagerId;
		}

		
		private CacheConfig CreateCacheConfig(IKernel kernel,ComponentModel model)
		{
			CacheConfig config = BuildCacheConfig(kernel, model);

			GatherCacheConfiguration(config, model);
			GatherCacheAttributes(config, model);
			return config;
		}

		
		private CacheConfig BuildCacheConfig(IKernel kernel,ComponentModel model)
		{
			string cacheManagerId = GetCacheManagerId(model);
			CacheConfig cacheConfig = new CacheConfig( kernel, cacheManagerId );
			return cacheConfig; 
		}


		private void GatherCacheConfiguration(CacheConfig config, ComponentModel model)
		{
			if (model.Configuration == null) return;
			
			// Récuperer tt les fils cache
			IConfiguration cacheNode = model.Configuration.Children["cache"];

			if (cacheNode == null) return;

			foreach(IConfiguration configuration in model.Configuration.Children)
			{
				if (configuration.Name=="cache")
				{
					foreach(IConfiguration methodNode in configuration.Children)
					{
						config.AddMethodName( methodNode.Value, configuration.Attributes["ref"] );
					}
				}
			}
		}

		private void GatherCacheAttributes(CacheConfig config, ComponentModel model)
		{
			MethodInfo[] methods = model.Implementation.GetMethods( 
				BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic );

			foreach(MethodInfo method in methods)
			{
				if (method.IsDefined( typeof(CacheAttribute), true ))
				{
					CacheAttribute[] attributs = method.GetCustomAttributes(typeof(CacheAttribute), true) as CacheAttribute[];
					string cacheManagerId = attributs[0].CacheManagerId;

					config.AddMethod( method, cacheManagerId );
				}
			}
		}


		#endregion
	}
}
