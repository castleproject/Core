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
	using System.Runtime.Remoting.Channels.Http;

	using Castle.ManagementExtensions.Remote.Server;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for HttpChannelProvider.
	/// </summary>
	public class HttpChannelProvider : AbstractServerProvider
	{
		public HttpChannelProvider()
		{
		}

		protected override bool AcceptsChannel(String channel)
		{
			return "http".EndsWith(channel);
		}

		protected override bool AcceptsFormatter(String formatter)
		{
			return "binary".EndsWith(formatter) || "soap".EndsWith(formatter);
		}

		#region MProvider Members
		
		public override MConnector Connect(String url, System.Collections.Specialized.NameValueCollection properties)
		{
			String[] parts = StripUrl(url);

			String formatter = parts[2];
			String objectUri = parts[3];
			String objectUrl = null;
			
			HttpClientChannel channel = new HttpClientChannel();

			try
			{
				ChannelServices.RegisterChannel( channel );
			}
			catch(RemotingException)
			{
				// Already registered
			}

			objectUrl = String.Format("{0}://{1}:{2}/{3}", 
				"http", GetHost(properties), GetPort(properties), objectUri);

			object ret = RemotingServices.Connect( typeof(MServer), objectUrl, null );

			return new MConnector( (MServer) ret, channel );
		}

		#endregion

		#region MServerProvider Members

		public override MConnectorServer CreateServer(String url, NameValueCollection properties, MServer server)
		{
			String[] parts = StripUrl(url);
			String formatter = parts[2];
			String objectUri = parts[3];
			
			HttpChannel channel = CreateChannel(formatter, properties, true);

			MConnectorServer connServer = null;

			if (server != null)
			{
				connServer = new MConnectorServer(server, objectUri);
			}
			else
			{
				connServer = new MConnectorServer(objectUri);
			}

			//connServer.Channel = channel;

			return connServer;
		}

		#endregion

		private HttpChannel CreateChannel(String formatter, NameValueCollection properties, bool createAsServer)
		{
			HttpChannel httpChannel = null;

			int portNum = GetPort(properties);
			
			bool alreadyRegistered = false;

			foreach(IChannel channel in ChannelServices.RegisteredChannels)
			{
				if (channel.ChannelName.Equals("http"))
				{
					HttpChannel item = (HttpChannel) channel;
					ChannelDataStore dataStore = (ChannelDataStore) item.ChannelData;
					// TODO: Check if is the same channel as the url specify
					
					httpChannel = item;
					
					alreadyRegistered = true;
					break;
				}
			}

			if (!alreadyRegistered)
			{
				if (createAsServer)
				{
					httpChannel = new HttpChannel(portNum);
				}
				else
				{
					httpChannel = new HttpChannel();
				}

				ChannelServices.RegisterChannel( httpChannel );
			}

			return httpChannel;
		}
	}
}
