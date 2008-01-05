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
	using Castle.Core.Configuration;
	using Castle.Core.Configuration.Xml;

	/// <summary>
	/// Represents a MonoRail extension configuration entry
	/// </summary>
	public class ExtensionEntry : ISerializedConfig
	{
		private Type extensionType;
		private IConfiguration extensionNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionEntry"/> class.
		/// </summary>
		public ExtensionEntry()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionEntry"/> class.
		/// </summary>
		/// <param name="extensionType">Type of the extension.</param>
		/// <param name="extensionNode">The extension node.</param>
		public ExtensionEntry(Type extensionType, IConfiguration extensionNode)
		{
			this.extensionType = extensionType;
			this.extensionNode = extensionNode;
		}

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlAttribute typeAtt = section.Attributes["type"];

			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				String message = "To add a service, please specify the 'type' attribute. " + 
					"Check the documentation for more information";
				throw new ConfigurationErrorsException(message);
			}
		
			extensionType = TypeLoadUtil.GetType(typeAtt.Value);

			extensionNode = XmlConfigurationDeserializer.GetDeserializedNode(section);
		}
		
		#endregion

		/// <summary>
		/// Gets or sets the type of the extension.
		/// </summary>
		/// <value>The type of the extension.</value>
		public Type ExtensionType
		{
			get { return extensionType; }
			set { extensionType = value; }
		}

		/// <summary>
		/// Gets or sets the extension node.
		/// </summary>
		/// <value>The extension node.</value>
		public IConfiguration ExtensionNode
		{
			get { return extensionNode; }
			set { extensionNode = value; }
		}
	}
}
