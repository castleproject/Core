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

namespace Castle.MicroKernel.Model
{
	using System;
	using System.Collections;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Holds information necessary for the lifetime of the component.
	/// </summary>
	public interface IComponentModel
	{
		/// <summary>
		/// Component name
		/// </summary>
		String Name { get; }

		/// <summary>
		/// Desired lifecycle
		/// </summary>
        Lifestyle SupportedLifestyle { get; set; }

        /// <summary>
		/// Desired activation policy
		/// </summary>
        Activation ActivationPolicy { get; set; }

        /// <summary>
		/// Service being exposed by the component
		/// </summary>
		Type Service { get; }

		/// <summary>
		/// ILogger implementation
		/// </summary>
		ILogger Logger { get; set; }

		/// <summary>
		/// Configuration node for the component
		/// </summary>
		IConfiguration Configuration { get; set; }

		/// <summary>
		/// Context for the component
		/// </summary>
        IContext Context { get; set; }

        /// <summary>
		/// List of dependencies declared by the component
		/// </summary>
        IDependencyModel[] Dependencies { get; set; }

        /// <summary>
		/// Information to allow the correct construction 
		/// of the component
		/// </summary>
		IConstructionModel ConstructionModel { get; }

		/// <summary>
		/// 
		/// </summary>
		IDictionary Properties { get; }
	}
}