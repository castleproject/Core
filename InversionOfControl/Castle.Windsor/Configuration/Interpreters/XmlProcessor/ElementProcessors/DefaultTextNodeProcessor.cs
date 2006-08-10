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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Text.RegularExpressions;
	using System.Xml;

	public class DefaultTextNodeProcessor : AbstractXmlNodeProcessor
	{
		private static readonly XmlNodeType[] acceptNodes = new XmlNodeType[] {XmlNodeType.CDATA, XmlNodeType.Text};
        public const string ExtraInformationAttributeName = "castle.extra.information";
		/// <summary>
		/// Properties names can contain a-zA-Z0-9_. 
		/// i.e. #!{ my_node_name } || #{ my.node.name } || #{ my.node.obj @ name }
		/// spaces are trimmed
		/// </summary>
        private static readonly Regex PropertyValidationRegExp = new Regex(@"(\#!?\{\s*(?<key>(?:\w|\.)+)\s*(@\s*(?<extraInfo>(?:\w+)))?\s*\})", RegexOptions.Compiled);

		public DefaultTextNodeProcessor()
		{
		}

		public override String Name
		{
			get { return "#text"; }
		}

		public override XmlNodeType[] AcceptNodeTypes
		{
			get { return acceptNodes; }
		}

		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlCharacterData node = nodeList.Current as XmlCharacterData;

			ProcessString(node, node.Value, engine);
		}

		/// <summary>
		/// Processes the string.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="value">The value.</param>
		/// <param name="engine">The context.</param>
		public void ProcessString(XmlNode node, string value, IXmlProcessorEngine engine)
		{
			XmlDocumentFragment fragment = CreateFragment(node);

			Match match;
			int pos = 0;
			while((match = PropertyValidationRegExp.Match(value, pos)).Success)
			{
				if (pos < match.Index)
				{
					AppendChild(fragment, value.Substring(pos, match.Index - pos));
				}

				string propRef = match.Groups[1].Value; // #!{ propKey }
				string propKey = match.Groups["key"].Value; // propKey
                string extraInfo = null;
                if (match.Groups["extraInfo"].Success)
                    extraInfo = match.Groups["extraInfo"].Value;
				XmlNode prop = engine.GetProperty(propKey);

				if (prop != null)
				{
					// When node has a parentNode (not an attribute)
					// we copy any attributes for the property into the parentNode
					if (node.ParentNode != null)
					{
						MoveAttributes(node.ParentNode as XmlElement, prop as XmlElement);
					}

				    // sets an attribute 'castle.extra.information' with the value of everything
				    // after the @ in the prop.
                    SetExtraInformationForConfiguration(node, prop, extraInfo);

				    AppendChild(fragment, prop.ChildNodes);
				}
				else if (IsRequiredProperty(propRef))
				{
					throw new XmlProcessorException(String.Format("Required configuration property {0} not found", propKey));
				}

				pos = match.Index + match.Length;
			}

			// Appending anything left
			if (pos > 0 && pos < value.Length)
			{
				AppendChild(fragment, value.Substring(pos, value.Length - pos));
			}

			// we only process when there was at least one match
			// even when the framents contents is empty since
			// that could mean that there was a match but the property
			// reference was a silent property
			if (pos > 0)
			{
				if (node.NodeType == XmlNodeType.Attribute)
				{
					node.Value = fragment.InnerText.Trim();
				}
				else
				{
					ReplaceNode(node.ParentNode, fragment, node);
				}
			}
		}

	    private static void SetExtraInformationForConfiguration(XmlNode node, XmlNode prop, string propMember)
	    {
	        if(propMember!=null)
	        {
                XmlAttribute propMemberAttribute = prop.OwnerDocument.CreateAttribute(ExtraInformationAttributeName);
	            propMemberAttribute.Value = propMember;
	            XmlAttribute xmlAttribute = node as XmlAttribute;
	            if (xmlAttribute != null)
	                xmlAttribute.OwnerElement.Attributes.Append(propMemberAttribute);
	            else
	                node.ParentNode.Attributes.Append(propMemberAttribute);
	        }
	    }

	    private bool IsRequiredProperty(string propRef)
		{
			return propRef.StartsWith("#{");
		}

		private void MoveAttributes(XmlElement targetElement, XmlElement srcElement)
		{
			for(int i = srcElement.Attributes.Count - 1; i > -1; i--)
			{
				XmlAttribute importedAttr = ImportNode(targetElement, srcElement.Attributes[i]) as XmlAttribute;
				targetElement.Attributes.Append(importedAttr);
			}
		}
	}
}