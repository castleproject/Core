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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;

	using Castle.Core;

	using Castle.MicroKernel.ModelBuilder;

	/// <summary>
	/// Implementors must construct a populated
	/// instance of ComponentModel by inspecting the component
	/// and|or the configuration.
	/// </summary>
	public interface IComponentModelBuilder
	{
		/// <summary>
		/// Constructs a new ComponentModel by invoking
		/// the registered contributors.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		/// <returns></returns>
		ComponentModel BuildModel(String key, Type service, Type classType, IDictionary extendedProperties);

		/// <summary>
		/// "To give or supply in common with others; give to a 
		/// common fund or for a common purpose". The contributor
		/// should inspect the component, or even the configuration
		/// associated with the component, to add or change information
		/// in the model that can be used later.
		/// </summary>
		void AddContributor(IContributeComponentModelConstruction contributor);

		/// <summary>
		/// Removes the specified contributor
		/// </summary>
		/// <param name="contributor"></param>
		void RemoveContributor(IContributeComponentModelConstruction contributor);

		IContributeComponentModelConstruction[] Contributors { get; }
	}
}
