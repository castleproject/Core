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

namespace Castle.MicroKernel
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel.ModelBuilder;

	/// <summary>
	/// Summary description for IComponentModelBuilder.
	/// </summary>
	public interface IComponentModelBuilder
	{
		ComponentModel BuildModel( String key, Type service, Type classType );

		/// <summary>
		/// "To give or supply in common with others; give to a 
		/// common fund or for a common purpose"
		/// </summary>
		void AddContributor( IContributeComponentModelConstruction contributor );

		void RemoveContributor( IContributeComponentModelConstruction contributor );
	}
}
