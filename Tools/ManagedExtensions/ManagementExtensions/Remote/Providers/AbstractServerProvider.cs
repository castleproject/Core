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

	using Castle.ManagementExtensions.Remote.Server;
	using Castle.ManagementExtensions.Remote.Client;

	/// <summary>
	/// Summary description for AbstractServerProvider.
	/// </summary>
	public abstract class AbstractServerProvider : MServerProvider, MProvider
	{
		private static readonly char[] SEPARATOR = new char[] { ':' };

		public AbstractServerProvider()
		{
		}

		#region MServerProvider Members

		public bool Accepts(String url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url", "null url for MProvider");
			}

			String[] parts = StripUrl(url);

			String channel   = parts[1];
			String formatter = parts[2];

			return AcceptsChannel(channel) && AcceptsFormatter(formatter);
		}

		public abstract MConnectorServer CreateServer(String url, System.Collections.Specialized.NameValueCollection properties, MServer server);

		#endregion

		#region MServerProvider Members
		
		public abstract MConnector Connect(String url, System.Collections.Specialized.NameValueCollection properties);

		#endregion

		protected abstract bool AcceptsChannel(String channel);

		protected abstract bool AcceptsFormatter(String formatter);

		protected String[] StripUrl(String url)
		{
			String[] parts = url.Split( SEPARATOR );

			if (parts.Length != 4)
			{
				throw new InvalidUrlException(url);
			}
			if (parts[0] == null || !"provider".EndsWith(parts[0]))
			{
				throw new InvalidUrlException(url);
			}

			return parts;
		}

		protected int GetPort(NameValueCollection properties)
		{
			String port = null;

			if (properties != null)
			{
				port = properties["port"];
			}
			
			if (port == null || port == String.Empty)
			{
				port = "3334";
			}

			return Convert.ToInt32(port);
		}

		protected String GetHost(NameValueCollection properties)
		{
			String host = null;

			if (properties != null)
			{
				host = properties["host"];
			}
			
			if (host == null || host == String.Empty)
			{
				host = "localhost";
			}

			return host;
		}
	}
}
