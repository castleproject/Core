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
	using System.ServiceModel;
	using System.ServiceModel.Description;

	/// <summary>
	/// Event to indicate an endpoint was created.
	/// </summary>
	public class EndpointCreatedArgs : EventArgs
	{
		private readonly ServiceEndpoint endpoint;

		/// <summary>
		/// Creates a new <see cref="EndpointCreatedArgs"/>.
		/// </summary>
		/// <param name="endpoint">The created endpoint.</param>
		public EndpointCreatedArgs(ServiceEndpoint endpoint)
		{
			this.endpoint = endpoint;
		}

		/// <summary>
		/// Gets the newly created endpoint.
		/// </summary>
		public ServiceEndpoint Endpoint
		{
			get { return endpoint; }
		}
	}

	/// <summary>
	/// Contract extension for <see cref="ServiceHost"/>.
	/// </summary>
	public interface IWcfServiceHost
	{
		/// <summary>
		/// Raised when a service host creates an endpoint.
		/// </summary>
		event EventHandler<EndpointCreatedArgs> EndpointCreated;
	}
}
