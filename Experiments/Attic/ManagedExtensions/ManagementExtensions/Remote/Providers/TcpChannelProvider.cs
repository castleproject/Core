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

namespace Castle.ManagementExtensions.Remote.Providers
{
	using System;
	using System.Collections.Specialized;

	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Tcp;

	using Castle.ManagementExtensions.Remote.Server;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for TcpChannelProvider.
	/// </summary>
	public class TcpChannelProvider : AbstractServerProvider
	{
		public TcpChannelProvider()
		{
		}
	
		protected override bool AcceptsChannel(String channel)
		{
			return "tcp".Equals(channel);
		}
	
		protected override bool AcceptsFormatter(String formatter)
		{
			return true;
		}
	
		public override MConnector Connect(String url, NameValueCollection properties)
		{
			String[] parts = StripUrl(url);

			String formatter = parts[2];
			String objectUri = parts[3];
			String objectUrl = null;
			
			TcpChannel channel = CreateChannel(formatter, properties, false);

			objectUrl = String.Format("{0}://{1}:{2}/{3}", 
				"tcp", GetHost(properties), GetPort(properties), objectUri);

			MServer proxy = (MServer) RemotingServices.Connect( typeof(MServerProxy), objectUrl );

			return new MConnector( (MServer) proxy, channel );
		}
	
		public override MConnectorServer CreateServer(String url, NameValueCollection properties, MServer server)
		{
			String[] parts = StripUrl(url);
			String formatter = parts[2];
			String objectUri = parts[3];
			
			TcpChannel channel = CreateChannel(formatter, properties, true);

			MConnectorServer connServer = null;

			if (server != null)
			{
				connServer = new MConnectorServer(server, objectUri);
			}
			else
			{
				connServer = new MConnectorServer(objectUri);
			}

			return connServer;
		}

		private TcpChannel CreateChannel(String formatter, NameValueCollection properties, bool createAsServer)
		{
			TcpChannel tcpChannel = null;

			int portNum = GetPort(properties);
			
			bool alreadyRegistered = false;

			foreach(IChannel channel in ChannelServices.RegisteredChannels)
			{
				if (channel.ChannelName.Equals("tcp"))
				{
					TcpChannel item = (TcpChannel) channel;
					ChannelDataStore dataStore = (ChannelDataStore) item.ChannelData;
					// TODO: Check if is the same channel as the url specify
					
					tcpChannel = item;
					
					alreadyRegistered = true;
					break;
				}
			}

			if (!alreadyRegistered)
			{
				if (createAsServer)
				{
					tcpChannel = new TcpChannel(portNum);
				}
				else
				{
					tcpChannel = new TcpChannel(0);
				}

				ChannelServices.RegisterChannel( tcpChannel );
			}

			return tcpChannel;
		}
	}
}
