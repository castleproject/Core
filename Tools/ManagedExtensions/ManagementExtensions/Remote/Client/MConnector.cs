// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Remote.Client
{
	using System;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;

	/// <summary>
	/// Summary description for MConnector.
	/// </summary>
	[Serializable]
	public sealed class MConnector : IDisposable
	{
		private MServer remoteServer;
		private IChannel channel;

		internal MConnector(MServer remoteServer, IChannel channel)
		{
			if (remoteServer == null)
			{
				throw new ArgumentNullException("remoteServer");
			}

			this.remoteServer = remoteServer;
			this.channel = channel;
		}

		~MConnector()
		{
			Disconnect();
		}

		public MServer ServerConnection
		{
			get
			{
				return remoteServer as MServer;
			}
		}

		public void Disconnect()
		{
			if (remoteServer != null)
			{
				remoteServer = null;
			}

			if (channel != null)
			{
				try
				{
					ChannelServices.UnregisterChannel( channel );
				}
				catch(RemotingException)
				{
					// Somebody removed first
				}
				channel = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			
			Disconnect();
		}

		#endregion
	}
}
