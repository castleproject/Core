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

using AspectSharp.Lang.AST;
using Castle.Model;
using AspectEngine = AspectSharp.AspectEngine;
using AspectSharp.Builder;

namespace Castle.Facilities.AspectSharp
{
	using System;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;

	public class AspectSharpFacility : IFacility
	{
		private AspectEngine _engine;

		public AspectSharpFacility()
		{
		}

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			RegisterAspectEngine(kernel, facilityConfig);
			RegisterInterceptor(kernel);

			kernel.ProxyFactory = new AopProxyFactory();

			// Request the engine:
			_engine = (AspectEngine) kernel[ typeof(AspectEngine) ];

			kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);
		}
		
		public void Terminate()
		{
		}

		private void RegisterAspectEngine(IKernel kernel, IConfiguration facilityConfig)
		{
			String contents = facilityConfig.Value;

			// By now we're supporting only AspectLanguageEngineBuilder

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);

			ComponentModel model = 
				new ComponentModel("aspectsharp.engine", 
					typeof(AspectEngine), typeof(AspectEngine));
			
			model.ExtendedProperties.Add("builder", builder);
			model.CustomComponentActivator = typeof(AspectEngineActivator);

			kernel.AddCustomComponent( model );
		}

		private void RegisterInterceptor(IKernel kernel)
		{
			kernel.AddComponent( "aspectsharp.interceptor", typeof(AopInterceptor) );
		}

		private void OnComponentRegistered(String key, IHandler handler)
		{
			// We check if the implementation of this component 
			// has one or more aspects associated:

			AspectDefinition[] aspects = 
				_engine.AspectMatcher.Match( handler.ComponentModel.Implementation, 
				_engine.Configuration.Aspects );

			if (aspects.Length != 0)
			{
				handler.ComponentModel.ExtendedProperties["aop.aspect"] = Union(aspects);

				// This component has aspects associated.
				// We just associate an interceptor to dispatch

				handler.ComponentModel.Interceptors.Add( 
					new InterceptorReference( typeof(AopInterceptor) ) );
			}
		}

		private AspectDefinition Union(AspectDefinition[] aspects)
		{
			if (aspects.Length == 1)
			{
				return aspects[0];
			}

			// TODO: Merge aspects

			return aspects[0];
		}
	}
}
