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

using AspectSharp.Lang.AST;
using AspectEngine = AspectSharp.AspectEngine;
using AspectSharp.Builder;

namespace Castle.Facilities.AspectSharp
{
	using System;
	using System.Xml;

	using Castle.Model;
	using Castle.Model.Configuration;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	public class AspectSharpFacility : AbstractFacility
	{
		private AspectEngine _engine;

		public AspectSharpFacility()
		{
		}

		protected override void Init()
		{
			if (FacilityConfig == null) return;

			RegisterAspectEngine();
			RegisterInterceptor();

			Kernel.ProxyFactory = new AopProxyFactory();

			// Request the engine:
			_engine = (AspectEngine) Kernel[ typeof(AspectEngine) ];

			Kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);
		}

		private void RegisterAspectEngine()
		{
			AspectEngineBuilder builder = null;

			// see if we have an xml config
			foreach(IConfiguration child in FacilityConfig.Children)
			{
				if("configuration".Equals(child.Name))
				{
					string config = ConfigurationToXml(child);
					builder = new XmlEngineBuilder(config);
				}
			}

			// else we try to use the language engine 
			if(builder == null)
			{
				builder = new AspectLanguageEngineBuilder(FacilityConfig.Value);
			}

			ComponentModel model = 
				new ComponentModel("aspectsharp.engine", 
					typeof(AspectEngine), typeof(AspectEngine));
			
			model.ExtendedProperties.Add("builder", builder);
			model.CustomComponentActivator = typeof(AspectEngineActivator);

			Kernel.AddCustomComponent( model );
		}

		protected string ConfigurationToXml(IConfiguration config)
		{
			XmlElement configRoot = ConfigurationToXml(config, null);

			return configRoot.OuterXml;
		}

		protected XmlElement ConfigurationToXml(IConfiguration config, XmlElement root)
		{
			XmlDocument xmlDoc;

			if (root == null)
				xmlDoc = new XmlDocument();
			else
				xmlDoc = root.OwnerDocument;

			XmlElement node = xmlDoc.CreateElement(config.Name);
			node.InnerText = config.Value;

			foreach (string attributeName in config.Attributes)
			{
				XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(Convert.ToString(attributeName));
				xmlAttribute.Value = Convert.ToString(config.Attributes[attributeName]);

				node.Attributes.Append(xmlAttribute);
			}

			foreach (IConfiguration child in config.Children)
			{
				node.AppendChild(ConfigurationToXml(child, node));
			}

			return node;
		}

		private void RegisterInterceptor()
		{
			Kernel.AddComponent( "aspectsharp.interceptor", typeof(AopInterceptor) );
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
