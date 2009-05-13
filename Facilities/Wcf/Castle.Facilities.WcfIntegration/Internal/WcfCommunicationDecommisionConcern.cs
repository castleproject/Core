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

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System;
	using System.ServiceModel;
	using Castle.Core;
	using Castle.MicroKernel.LifecycleConcerns;

	/// <summary>
	/// Ensure that the communication channel is properly disposed.
	/// </summary>
	[Serializable]
	internal class WcfCommunicationDecomissionConcern : ILifecycleConcern
	{
		private readonly WcfClientExtension clients;

		public WcfCommunicationDecomissionConcern(WcfClientExtension clients)
		{
			this.clients = clients;
		}

		/// <summary>
		/// Performs the cleanup necessary to properly release a client channel.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="component">The component instance.</param>
		public void Apply(ComponentModel model, object component)
		{
			var comm = component as ICommunicationObject;
			WcfUtils.ReleaseCommunicationObject(comm, clients.CloseTimeout);
		}
	}
}
