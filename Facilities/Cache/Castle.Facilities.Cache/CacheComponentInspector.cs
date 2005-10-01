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
	/// Description résumée de CacheComponentInspector.
	/// </summary>
	public class CacheComponentInspector : IContributeComponentModelConstruction
	{
		CacheConfigHolder _cacheConfigHolder = null;

		#region Membres de IContributeComponentModelConstruction

		public void ProcessModel(Castle.MicroKernel.IKernel kernel, Castle.Model.ComponentModel model)
		{
			bool allowModelCache = IsCacheModelOn(kernel, model);

			if (allowModelCache)
			{
				model.Dependencies.Add( new DependencyModel( DependencyType.Service, null, typeof(CacheInterceptor), false ) );
				model.Interceptors.Add( new InterceptorReference(typeof(CacheInterceptor)) );

				_cacheConfigHolder = kernel[ typeof(CacheConfigHolder) ] as CacheConfigHolder;
				CacheConfig config = CreateCacheConfig(model);
				_cacheConfigHolder.Register(model.Implementation, config);
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


		private CacheConfig CreateCacheConfig(ComponentModel model)
		{
			CacheConfig config = new CacheConfig();
			GatherTransactionConfiguration(config, model);
			GatherCacheAttributes(config, model.Implementation);
			return config;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="model"></param>
		/// <example>
		///		<component id="ServiceA"
		///			 cache="true"
		///		     service="TestConfig.IServiceA, TestConfig" 
		///		     type="TestConfig.ServiceA, TestConfig">
		///		  <cache>
		///		    <method>MyMethod</method>
		///		  </cache>
		//		</component>
		/// </example>
		private void GatherTransactionConfiguration(CacheConfig config, ComponentModel model)
		{
			if (model.Configuration == null) return;
			
			IConfiguration transactionNode = model.Configuration.Children["cache"];

			if (transactionNode == null) return;

			foreach(IConfiguration methodNode in transactionNode.Children)
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
