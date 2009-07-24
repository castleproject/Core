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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using Castle.Core;
	using System.ServiceModel;

	/// <summary>
	/// The contract for building service hosts.
	/// </summary>
	/// <typeparam name="M">The <see cref="IWcfServiceModel"/> type.</typeparam>
	public interface IServiceHostBuilder<M> where M : IWcfServiceModel
	{
		/// <summary>
		/// Builds a service host.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="serviceModel">The service model.</param>
		/// <param name="baseAddresses">The base addresses.</param>
		/// <returns>The service host.</returns>
		/// 
		ServiceHost Build(ComponentModel model, M serviceModel, params Uri[] baseAddresses);

		/// <summary>
		///  Builds a service host for a hosted environment.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="baseAddresses">The base addresses.</param>
		/// <returns>The service host.</returns>
		ServiceHost Build(ComponentModel model, params Uri[] baseAddresses);

		/// <summary>
		///  Builds a service host for a hosted environment.
		/// </summary>
		/// <param name="serviceType">The service type.</param>
		/// <param name="baseAddresses">The base addresses.</param>
		/// <returns>The service host.</returns>
		ServiceHost Build(Type serviceType, params Uri[] baseAddresses);
	}
}
