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

namespace Castle.MicroKernel.ModelBuilder
{
	using System;
	using System.Collections;

	using Castle.Model;

	using Castle.MicroKernel.ModelBuilder.Inspectors;

	/// <summary>
	/// Summary description for DefaultComponentModelBuilder.
	/// </summary>
	public class DefaultComponentModelBuilder : IComponentModelBuilder
	{
		private IList _contributors;

		public DefaultComponentModelBuilder()
		{
			_contributors = new ArrayList();

			AddContributor( new LifestyleModelInspector() );
			AddContributor( new ConstructorDependenciesModelInspector() );
			AddContributor( new PropertiesDependenciesModelInspector() );
			AddContributor( new LifecycleModelInspector() );
		}

		#region IComponentModelBuilder Members

		public ComponentModel BuildModel(String key, Type service, Type classType)
		{
			ComponentModel model = new ComponentModel(key, service, classType);

			foreach(IContributeComponentModelConstruction contributor in _contributors)
			{
				contributor.ProcessModel( model );
			}
			
			return model;
		}

		public void AddContributor(IContributeComponentModelConstruction contributor)
		{
			_contributors.Add(contributor);
		}

		public void RemoveContributor(IContributeComponentModelConstruction contributor)
		{
			_contributors.Remove(contributor);
		}

		#endregion
	}
}
