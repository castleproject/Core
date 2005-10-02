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
using Castle.Facilities.Cache.Manager;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model;
using Castle.Model.Configuration;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Description résumée de CacheComponentInspector.
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
		private CacheConfigHolder _cacheConfigHolder = null;
		private Castle.MicroKernel.IKernel _kernel =null;
		private Castle.Model.ComponentModel _model =null;

		#region IContributeComponentModelConstruction Members

		public void ProcessModel(Castle.MicroKernel.IKernel kernel, Castle.Model.ComponentModel model)
		{
			bool allowModelCache = IsCacheModelOn(kernel, model);

			if (allowModelCache)
			{
				model.Dependencies.Add( new DependencyModel( DependencyType.Service, null, typeof(CacheInterceptor), false ) );
				model.Interceptors.Add( new InterceptorReference(typeof(CacheInterceptor)) );

				_cacheConfigHolder = kernel[ typeof(CacheConfigHolder) ] as CacheConfigHolder;
				_model = model;
				_kernel = kernel;
				kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);
			}		
		}

		private void OnComponentRegistered(String key, IHandler handler)
		{
			object obj =_kernel[key];


			if ( typeof(ICacheManager).IsInstanceOfType( obj ) )
			{
				CacheConfig config = CreateCacheConfig(_kernel, _model);
				_cacheConfigHolder.Register(_model.Implementation, config);
			}
			else
			{
				if (IsCacheModelOn(_kernel, _model))
				{
					// Check .NET attribut Cache or config attribute ref 
					string cacheManagerId = GetCacheManagerId( _model );
					// Check if cacheManager is regsitered
					try
					{
						ICacheManager cacheManager = _kernel[cacheManagerId] as ICacheManager;
						// si oui faire CreateCacheConfig	
						if (cacheManager!=null)
						{
							CacheConfig config = CreateCacheConfig(_kernel, _model);
							_cacheConfigHolder.Register(_model.Implementation, config);
						}						
					}
					catch
					{
						// The ICacheManager has not yet been registered
						// it will be "injected" on ComponentRegistered event
					}
				}				
			}
		}

		private bool IsCacheModelOn(Castle.MicroKernel.IKernel kernel, ComponentModel model)
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

		
		private CacheConfig CreateCacheConfig(Castle.MicroKernel.IKernel kernel,ComponentModel model)
		{
			CacheConfig config = BuildCacheConfig(kernel, model);

			GatherCacheConfiguration(config, model);
			GatherCacheAttributes(config, model.Implementation);
			return config;
		}

		
		private CacheConfig BuildCacheConfig(Castle.MicroKernel.IKernel kernel,ComponentModel model)
		{
			ICacheManager cacheManager = null;
			string cacheManagerId = GetCacheManagerId(model);

			if (cacheManagerId == string.Empty )
			{
				throw new ArgumentException("You need to specify a CacheManager via attribute or ref attribute in config file.");
			}

			cacheManager = (ICacheManager)kernel[cacheManagerId];
			CacheConfig cacheConfig = new CacheConfig(cacheManager);
			return cacheConfig; 
		}


		private void GatherCacheConfiguration(CacheConfig config, ComponentModel model)
		{
			if (model.Configuration == null) return;
			
			IConfiguration cacheNode = model.Configuration.Children["cache"];

			if (cacheNode == null) return;

			foreach(IConfiguration methodNode in cacheNode.Children)
			{
				config.AddMethodName( methodNode.Value );
			}
		}

		private void GatherCacheAttributes(CacheConfig config, Type implementation)
		{
			MethodInfo[] methods = implementation.GetMethods( 
				BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic );

			foreach(MethodInfo method in methods)
			{
				if (method.IsDefined( typeof(CacheAttribute), true ))
				{
					config.AddMethod( method );
				}
			}
		}


		#endregion
	}
}
