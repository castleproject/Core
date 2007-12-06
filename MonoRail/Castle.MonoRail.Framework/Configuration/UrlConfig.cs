// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// Represents the url node configuration
	/// </summary>
	public class UrlConfig : ISerializedConfig
	{
		private bool useExtensions = true;

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlNode urlNode = section.SelectSingleNode("url");

			if (urlNode != null)
			{
				XmlAttribute useExtensionsAtt = urlNode.Attributes["useExtensions"];

				if (useExtensionsAtt != null && useExtensionsAtt.Value != String.Empty)
				{
					useExtensions = String.Compare(useExtensionsAtt.Value, "true", true) == 0;
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets whether url should have an extension.
		/// </summary>
		public bool UseExtensions
		{
			get { return useExtensions; }
			set { useExtensions = value; }
		}
	}
}
