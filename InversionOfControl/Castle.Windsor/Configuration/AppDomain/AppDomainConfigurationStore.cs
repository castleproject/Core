// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Windsor.Configuration.AppDomain
{
	using System;
	using System.Xml;
	using System.Configuration;

	using Castle.Windsor.Configuration.Xml;

	/// <summary>
	/// Summary description for AppDomainConfigurationStore.
	/// </summary>
	public class AppDomainConfigurationStore : XmlConfigurationStore
	{
		public AppDomainConfigurationStore()
		{
			XmlNode node = (XmlNode) ConfigurationSettings.GetConfig("castle");

			if (node == null)
			{
				String message = String.Format(
					"Could not find section 'castle' in the configuration file associated with this domain.");
				throw new ConfigurationException(message);
			}

			Deserialize( node );
		}
	}
}
