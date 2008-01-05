// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// The MonoRail section handler
	/// </summary>
	public class MonoRailSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailSectionHandler"/> class.
		/// </summary>
		public MonoRailSectionHandler()
		{
		}

		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section"></param>
		/// <returns>The created section handler object.</returns>
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			MonoRailConfiguration config = new MonoRailConfiguration(section);
			
			Deserialize(section, config);
			
			return config;
		}

		/// <summary>
		/// Deserializes the config section.
		/// </summary>
		/// <param name="section">The section.</param>
		/// <param name="config">The config.</param>
		private void Deserialize(XmlNode section, MonoRailConfiguration config)
		{
			config.Deserialize(section);
		}
	}
}
