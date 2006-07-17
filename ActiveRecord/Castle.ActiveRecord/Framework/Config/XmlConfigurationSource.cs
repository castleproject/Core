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

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;

	/// <summary>
	/// Source of configuration based on Xml 
	/// source like files, streams or readers.
	/// </summary>
	public class XmlConfigurationSource : InPlaceConfigurationSource
	{
		protected XmlConfigurationSource()
		{
		}

		public XmlConfigurationSource(String xmlFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFileName);
			PopulateSource(doc.DocumentElement);
		}

		public XmlConfigurationSource(Stream stream)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			PopulateSource(doc.DocumentElement);
		}

		public XmlConfigurationSource(TextReader reader)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			PopulateSource(doc.DocumentElement);
		}

		protected void PopulateSource(XmlNode section)
		{
			XmlAttribute isWebAtt = section.Attributes["isWeb"];
			XmlAttribute threadInfoAtt = section.Attributes["threadinfotype"];
			XmlAttribute isDebug = section.Attributes["isDebug"];

			SetUpThreadInfoType(isWebAtt != null && "true" == isWebAtt.Value,
			                    threadInfoAtt != null ? threadInfoAtt.Value : String.Empty);

			XmlAttribute sessionfactoryholdertypeAtt =
				section.Attributes["sessionfactoryholdertype"];

			SetUpSessionFactoryHolderType(sessionfactoryholdertypeAtt != null
			                              	?
			                              sessionfactoryholdertypeAtt.Value
			                              	: String.Empty);

			SetDebugFlag(isDebug != null && "true" == isDebug.Value);

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

#if DOTNET2
					throw new System.Configuration.ConfigurationErrorsException(message);
#else
					throw new ConfigurationException(message);
#endif
				}

				Type targetType = typeof(ActiveRecordBase);

				if (node.Attributes.Count != 0)
				{
					XmlAttribute typeNameAtt = node.Attributes["type"];

					if (typeNameAtt == null)
					{
						String message = String.Format("Invalid attribute at node '{0}'. " +
						                               "The only supported attribute is 'type'", Config_Node_Name);

#if DOTNET2
						throw new System.Configuration.ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}

					String typeName = typeNameAtt.Value;

					targetType = Type.GetType(typeName, false, false);

					if (targetType == null)
					{
						String message = String.Format("Could not obtain type from name '{0}'", typeName);

#if DOTNET2
						throw new System.Configuration.ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}
				}

				Add(targetType, BuildProperties(node));
			}
		}

		protected IDictionary BuildProperties(XmlNode node)
		{
			HybridDictionary dict = new HybridDictionary();
#if DOTNET2
			System.Text.RegularExpressions.Regex connectionStringRegex = new System.Text.RegularExpressions.
				Regex(@"ConnectionString\s*=\s*\$\{(?<ConnectionStringName>[\d\w_-]+)\}");
			string ConnectionStringKey = "hibernate.connection.connection_string";
#endif
			foreach(XmlNode addNode in node.SelectNodes("add"))
			{
				XmlAttribute keyAtt = addNode.Attributes["key"];
				XmlAttribute valueAtt = addNode.Attributes["value"];

				if (keyAtt == null || valueAtt == null)
				{
					String message = String.Format("For each 'add' element you must specify 'key' and 'value' attributes");

#if DOTNET2
					throw new System.Configuration.ConfigurationErrorsException(message);
#else
					throw new ConfigurationException(message);
#endif
				}
				string value = valueAtt.Value;
#if DOTNET2

				if (keyAtt.Value == ConnectionStringKey
				    && connectionStringRegex.IsMatch(value))
				{
					string connectionStringName = connectionStringRegex.Match(value).
						Groups["ConnectionStringName"].Value;
					value = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
				}
#endif
				dict.Add(keyAtt.Value, value);
			}

			return dict;
		}
	}
}