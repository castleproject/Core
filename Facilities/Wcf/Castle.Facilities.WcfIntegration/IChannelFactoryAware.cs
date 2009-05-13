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
	/// Extension for managing the lifecycle of a ChannelFactory.
	/// </summary>
	public interface IChannelFactoryAware
	{
		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> is created.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Created(ChannelFactory channelFactory);

		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> is opening.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Opening(ChannelFactory channelFactory);

		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> opened.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Opened(ChannelFactory channelFactory);

		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> is closing.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Closing(ChannelFactory channelFactory);

		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> closed.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Closed(ChannelFactory channelFactory);

		/// <summary>
		/// Called when a <see cref="ChannelFactory"/> faulted.
		/// </summary>
		/// <param name="channelFactory">The channel factory.</param>
		void Faulted(ChannelFactory channelFactory);
	}
}
