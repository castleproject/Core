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
// limitations under the License

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Security;

	/// <summary>
	/// Policy to recover from a <see cref="CommunicationException"/>
	/// by refreshing the channel and trying again.  This policy will
	/// handle situations in which a connection has been reset on the
	/// server which invalidates the the client channel.
	/// </summary>
	public class ChannelReconnectPolicy : AbstractWcfPolicy, IChannelActionPolicy
	{
		/// <inheritdoc />
		public bool Perform(IWcfChannelHolder channelHolder, MethodInfo method, Action action)
		{
			bool reconnect = false;

			try
			{
				action();
			}
			catch (ChannelTerminatedException)
			{
				reconnect = true;
			}
			catch (CommunicationObjectFaultedException)
			{
				reconnect = true;
			}
			catch (CommunicationObjectAbortedException)
			{
				reconnect = true;
			} 
			catch (MessageSecurityException)
			{
				reconnect = true;
			}
			catch (CommunicationException cex)
			{
				if (cex.GetType() != typeof(CommunicationException))
				{
					throw;
				}
				reconnect = true;
			}

			if (reconnect)
			{
				channelHolder.RefreshChannel();
				action();
			}

			return true;
		}
	}
}
