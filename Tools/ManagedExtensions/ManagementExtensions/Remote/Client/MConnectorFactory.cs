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
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for MConnectorFactory.
	/// </summary>
	public sealed class MConnectorFactory
	{
		private static ArrayList providers;

		private MConnectorFactory()
		{
		}

		public static MConnector CreateConnector(String url, NameValueCollection properties)
		{
			if (providers == null)
			{
				providers = new ArrayList();

				// TODO: Search for providers instead of hard code it
				providers.Add (new Providers.HttpChannelProvider());
				providers.Add (new Providers.TcpChannelProvider());
			}

			foreach(MProvider provider in providers)
			{
				if (provider.Accepts(url))
				{
					return provider.Connect(url, properties);
				}
			}

			throw new MProviderNotFoundException(url);
		}
	}
}
