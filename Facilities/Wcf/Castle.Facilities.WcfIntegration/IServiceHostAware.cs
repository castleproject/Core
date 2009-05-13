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
	using System.ServiceModel;

	/// <summary>
	/// Extension for managing the lifecycle of a ServiceHost.
	/// </summary>
	public interface IServiceHostAware
	{
		/// <summary>
		/// Called when a <see cref="ServiceHost"/> is created.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Created(ServiceHost serviceHost);

		/// <summary>
		/// Called when a <see cref="ServiceHost"/> is opening.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Opening(ServiceHost serviceHost);

		/// <summary>
		/// Called when a <see cref="ServiceHost"/> opened.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Opened(ServiceHost serviceHost);

		/// <summary>
		/// Called when a <see cref="ServiceHost"/> is closing.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Closing(ServiceHost serviceHost);

		/// <summary>
		/// Called when a <see cref="ServiceHost"/> closed.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Closed(ServiceHost serviceHost);

		/// <summary>
		/// Called when a <see cref="ServiceHost"/> faulted.
		/// </summary>
		/// <param name="serviceHost">The service host.</param>
		void Faulted(ServiceHost serviceHost);
	}
}
