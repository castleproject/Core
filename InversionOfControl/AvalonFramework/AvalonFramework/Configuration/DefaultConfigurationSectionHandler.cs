// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;
	using System.Configuration; 
	using System.Xml;

	/// <summary>
	/// This section handler interprets and processes the settings defined in XML tags
	/// within a specific portion of a assembly config file and 
	/// returns an appropriate <see cref="DefaultConfiguration"/> instance 
	/// based on the configuration settings. 
	/// <seealso cref="IConfigurationSectionHandler"/>
	/// </summary>
	public class DefaultConfigurationSectionHandler: IConfigurationSectionHandler
	{
		/// <summary>
		/// Implemented by all configuration section handlers to parse 
		/// the XML of the configuration section. The returned object is added to 
		/// the configuration collection and is accessed by 
		/// <see cref="ConfigurationSettings.GetConfig(string)"/>.
		/// <seealso cref="IConfigurationSectionHandler"/>
		/// </summary>
		/// <param name="parent">
		/// The Configuration settings in a corresponding parent configuration section.
		/// </param>
		/// <param name="configContext">
		/// <see cref="IConfigurationSectionHandler"/>.
		/// </param>
		/// <param name="section">
		/// Contains the configuration information from the configuration file.
		/// </param>
		/// <returns>
		/// A <see cref="DefaultConfiguration"/> instance based on a configuration section.
		/// </returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			return DefaultConfigurationSerializer.Deserialize(section);  
		}
	}
}
