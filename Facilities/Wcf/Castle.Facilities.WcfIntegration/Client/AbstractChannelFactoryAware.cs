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

namespace Castle.Facilities.WcfIntegration.Client
{
	using System.ServiceModel;

	/// <summary>
	/// Abstarct implementation of <see cref="IChannelFactoryAware"/>
	/// </summary>
	public abstract class AbstractChannelFactoryAware : IChannelFactoryAware
	{
		/// <inheritdoc />
		public virtual void Created(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual void Opening(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual void Opened(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual void Closing(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual void Closed(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual void Faulted(ChannelFactory channelFactory)
		{
		}

		/// <inheritdoc />
		public virtual bool ShouldCreateNewChannelWhenInvalid(ChannelFactory channelFactory, 
															   IClientChannel channel)
		{
			return true;
		}
	}
}
