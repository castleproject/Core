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

namespace Castle.MicroKernel.Model.Default
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// Summary description for DefaultComponentModel.
	/// </summary>
	public class DefaultComponentModel : IComponentModel
	{
		private Type m_service;
		private String m_name;
		private Lifestyle m_lifestyle;
		private Activation m_activation;
		private ILogger m_logger;
		private IConfiguration m_config;
		private IContext m_context;
		private IDependencyModel[] m_dependencies;
		private IConstructionModel m_constructionModel;
		private IDictionary m_dictionary = new HybridDictionary();

		protected DefaultComponentModel()
		{
			Context = new DefaultContext();
		}

		public DefaultComponentModel(
			ComponentData data,
			Type service,
			ILogger logger,
			IConfiguration configuration,
			IConstructionModel constructionModel) : this()
		{
			AssertUtil.ArgumentNotNull(data, "data");
			AssertUtil.ArgumentNotNull(service, "service");
			AssertUtil.ArgumentNotNull(logger, "logger");
			AssertUtil.ArgumentNotNull(configuration, "configuration");
			AssertUtil.ArgumentNotNull(constructionModel, "constructionModel");

			m_name = data.Name;
			m_service = service;
			m_constructionModel = constructionModel;
			SupportedLifestyle = data.SupportedLifestyle;
            ActivationPolicy = data.ActivationPolicy;
            Logger = logger;
			Configuration = configuration;
			Dependencies = data.DependencyModel;
		}

		#region IComponentModel Members

		public String Name
		{
			get { return m_name; }
		}

		public Type Service
		{
			get { return m_service; }
		}

		public Lifestyle SupportedLifestyle
		{
			get { return m_lifestyle; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_lifestyle = value;
            }
        }

		public Activation ActivationPolicy
		{
			get { return m_activation; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_activation = value;
            }
        }

		public ILogger Logger
		{
			get { return m_logger; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_logger = value;
            }
        }

        public IConfiguration Configuration
		{
			get { return m_config; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_config = value;
            }
        }

        public IContext Context
		{
			get { return m_context; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_context = value;
            }
        }

        public IDependencyModel[] Dependencies
		{
			get { return m_dependencies; }
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_dependencies = value;
            }
        }

        public IConstructionModel ConstructionModel
		{
			get { return m_constructionModel; }
		}

		public IDictionary Properties
		{
			get { return m_dictionary; }
		}

		#endregion
	}
}