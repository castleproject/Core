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

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Xml;

	/// <summary>
	/// Source of configuration based on Xml 
	/// source like files, streams or readers.
	/// </summary>
	public class XmlConfigurationSource : InPlaceConfigurationSource
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		protected XmlConfigurationSource()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="xmlFileName">Name of the XML file.</param>
		public XmlConfigurationSource(String xmlFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFileName);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public XmlConfigurationSource(Stream stream)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public XmlConfigurationSource(TextReader reader)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Populate this instance with values from the given XML node
		/// </summary>
		protected void PopulateSource(XmlNode section)
		{
			XmlAttribute isWebAtt = section.Attributes["isWeb"];
			XmlAttribute threadInfoAtt = section.Attributes["threadinfotype"];
			XmlAttribute isDebug = section.Attributes["isDebug"];
			XmlAttribute lazyByDefault = section.Attributes["default-lazy"];
			XmlAttribute pluralize = section.Attributes["pluralizeTableNames"];
			XmlAttribute verifyModelsAgainstDBSchemaAtt = section.Attributes["verifyModelsAgainstDBSchema"];

			SetUpThreadInfoType(isWebAtt != null && "true" == isWebAtt.Value,
			                    threadInfoAtt != null ? threadInfoAtt.Value : String.Empty);

			XmlAttribute sessionfactoryholdertypeAtt =
				section.Attributes["sessionfactoryholdertype"];

			SetUpSessionFactoryHolderType(sessionfactoryholdertypeAtt != null
			                              	?
			                              sessionfactoryholdertypeAtt.Value
			                              	: String.Empty);

			XmlAttribute namingStrategyTypeAtt = section.Attributes["namingstrategytype"];

			SetUpNamingStrategyType(namingStrategyTypeAtt != null ? namingStrategyTypeAtt.Value : String.Empty);

			SetDebugFlag(ConvertBool(isDebug));

			SetIsLazyByDefault(ConvertBool(lazyByDefault));

			SetPluralizeTableNames(ConvertBool(pluralize));

			SetVerifyModelsAgainstDBSchema(verifyModelsAgainstDBSchemaAtt != null && verifyModelsAgainstDBSchemaAtt.Value == "true");

			PopulateConfigNodes(section);
		}

		private void PopulateConfigNodes(XmlNode section)
		{
			const string Config_Node_Name = "config";

			foreach(XmlNode node in section.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				if (!Config_Node_Name.Equals(node.Name))
				{
					String message = String.Format("Unexpected node. Expect '{0}' found '{1}'",
					                               Config_Node_Name, node.Name);

					throw new ConfigurationErrorsException(message);
				}

				Type targetType = typeof(ActiveRecordBase);

				if (node.Attributes.Count != 0)
				{
					XmlAttribute typeNameAtt = node.Attributes["type"];

					if (typeNameAtt == null)
					{
						String message = String.Format("Invalid attribute at node '{0}'. " +
						                               "The only supported attribute is 'type'", Config_Node_Name);

						throw new ConfigurationErrorsException(message);
					}

					String typeName = typeNameAtt.Value;

					targetType = Type.GetType(typeName, false, false);

					if (targetType == null)
					{
						String message = String.Format("Could not obtain type from name '{0}'", typeName);

						throw new ConfigurationErrorsException(message);
					}
				}

				Add(targetType, BuildProperties(node));
			}
		}

		/// <summary>
		/// Builds the configuration properties.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		protected IDictionary<string,string> BuildProperties(XmlNode node)
		{
			Dictionary<string,string> dict = new Dictionary<string,string>();

			foreach(XmlNode addNode in node.SelectNodes("add"))
			{
				XmlAttribute keyAtt = addNode.Attributes["key"];
				XmlAttribute valueAtt = addNode.Attributes["value"];

				if (keyAtt == null || valueAtt == null)
				{
					String message = String.Format("For each 'add' element you must specify 'key' and 'value' attributes");

					throw new ConfigurationErrorsException(message);
				}
				string value = valueAtt.Value;

				dict.Add(keyAtt.Value, value);
			}

			return dict;
		}

		private static bool ConvertBool(XmlNode boolAttrib)
		{
			return boolAttrib != null && "true".Equals(boolAttrib.Value, StringComparison.OrdinalIgnoreCase);
		}
	}
}
