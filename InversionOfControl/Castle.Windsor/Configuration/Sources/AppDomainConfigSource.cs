// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Sources
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Configuration;

	/// <summary>
	/// 
	/// </summary>
	public class AppDomainConfigSource : StaticContentSource
	{
		public AppDomainConfigSource()
		{
			XmlNode node = (XmlNode) ConfigurationSettings.GetConfig("castle");

			// Need to decide if this is going to be considered an error or not
			if (node == null)
			{
				String message = String.Format(
					"Could not find section 'castle' in the configuration file associated with this domain.");
				throw new ConfigurationException(message);
			}

			if (node == null) return;

			// TODO: Check whether its CData section
			_reader = new StringReader( node.OuterXml );
		}
	}
}
