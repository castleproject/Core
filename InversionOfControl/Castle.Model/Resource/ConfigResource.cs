// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model.Resource
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Configuration;


	public class ConfigResource : AbstractResource
	{
		private readonly XmlNode configSectionNode;

		public ConfigResource() : this("castle")
		{
		}

		public ConfigResource(CustomUri uri) : this(uri.Host)
		{
		}

		public ConfigResource(String sectionName)
		{
			XmlNode node = (XmlNode) ConfigurationSettings.GetConfig(sectionName);

			if (node == null)
			{
				String message = String.Format(
					"Could not find section '{0}' in the configuration file associated with this domain.", sectionName);
				throw new ConfigurationException(message);
			}

			// TODO: Check whether it's CData section
			configSectionNode = node;
		}

		public override TextReader GetStreamReader()
		{
			return new StringReader( configSectionNode.OuterXml );
		}

		public override TextReader GetStreamReader(Encoding encoding)
		{
			throw new NotSupportedException("Encoding is not supported");
		}

		public override IResource CreateRelative(String relativePath)
		{
			return new ConfigResource(relativePath);
		}

		public override void Dispose()
		{
			
		}
	}
}
