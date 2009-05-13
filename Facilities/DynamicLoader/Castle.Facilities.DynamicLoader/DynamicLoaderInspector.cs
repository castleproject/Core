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

namespace Castle.Facilities.DynamicLoader
{
	using System;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;

	/// <summary>
	/// Inspects component configuration nodes, looking for <c>domain</c>
	/// attributes. When found, register a custom activator: <see cref="DynamicLoaderActivator"/>.
	/// </summary>
	public class DynamicLoaderInspector : IContributeComponentModelConstruction
	{
		private readonly DynamicLoaderRegistry registry;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DynamicLoaderInspector(DynamicLoaderRegistry registry)
		{
			this.registry = registry;
		}

		/// <summary>
		/// Performs the inspection on the model.
		/// </summary>
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null)
				return;

			string domainId = model.Configuration.Attributes["domain"];
			if (String.IsNullOrEmpty(domainId))
				return;

			model.ExtendedProperties.Add("dynamicLoader.loader", registry.GetLoader(domainId));
			model.CustomComponentActivator = typeof(DynamicLoaderActivator);
		}
	}
}