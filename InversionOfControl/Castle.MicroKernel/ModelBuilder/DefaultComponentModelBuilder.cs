// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.ModelBuilder
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.MicroKernel.ModelBuilder.Inspectors;

	/// <summary>
	/// Summary description for DefaultComponentModelBuilder.
	/// </summary>
	[Serializable]
	public class DefaultComponentModelBuilder : IComponentModelBuilder
	{
		private readonly IKernel kernel;
		private readonly List<IContributeComponentModelConstruction> contributors;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultComponentModelBuilder"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public DefaultComponentModelBuilder(IKernel kernel)
		{
			this.kernel = kernel;
			contributors = new List<IContributeComponentModelConstruction>();

			InitializeContributors();
		}

		/// <summary>
		/// Constructs a new ComponentModel by invoking
		/// the registered contributors.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		/// <returns></returns>
		public ComponentModel BuildModel(String key, Type service, Type classType, IDictionary extendedProperties)
		{
			ComponentModel model = new ComponentModel(key, service, classType);
			
			if (extendedProperties != null)
			{
				model.ExtendedProperties = extendedProperties;
			}

			foreach(IContributeComponentModelConstruction contributor in contributors)
			{
				contributor.ProcessModel( kernel, model );
			}
			
			return model;
		}

		/// <summary>
		/// Gets the contributors.
		/// </summary>
		/// <value>The contributors.</value>
		public IContributeComponentModelConstruction[] Contributors
		{
			get { return contributors.ToArray(); }
		}

		/// <summary>
		/// "To give or supply in common with others; give to a
		/// common fund or for a common purpose". The contributor
		/// should inspect the component, or even the configuration
		/// associated with the component, to add or change information
		/// in the model that can be used later.
		/// </summary>
		/// <param name="contributor"></param>
		public void AddContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Add(contributor);
		}

		/// <summary>
		/// Removes the specified contributor
		/// </summary>
		/// <param name="contributor"></param>
		public void RemoveContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Remove(contributor);
		}

		/// <summary>
		/// Initializes the default contributors.
		/// </summary>
		protected virtual void InitializeContributors()
		{
			AddContributor(new GenericInspector());
			AddContributor(new ConfigurationModelInspector());
			AddContributor(new ConfigurationParametersInspector());
			AddContributor(new LifestyleModelInspector());
			AddContributor(new ConstructorDependenciesModelInspector());
			AddContributor(new PropertiesDependenciesModelInspector());
			AddContributor(new LifecycleModelInspector());
			AddContributor(new InterceptorInspector());
			AddContributor(new ComponentActivatorInspector());
			AddContributor(new ComponentProxyInspector());
		}
	}
}
