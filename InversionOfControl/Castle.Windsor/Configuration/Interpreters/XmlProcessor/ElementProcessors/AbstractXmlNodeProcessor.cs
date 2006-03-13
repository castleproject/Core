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
	using System.Xml;

	public abstract class AbstractXmlNodeProcessor : IXmlNodeProcessor
	{
		private static readonly XmlNodeType[] acceptNodes = new XmlNodeType[] {XmlNodeType.Element};

		public AbstractXmlNodeProcessor()
		{
		}

		public abstract String Name { get; }

		protected virtual bool IgnoreNode(XmlNode node)
		{
			return node.NodeType == XmlNodeType.Comment ||
				node.NodeType == XmlNodeType.Entity ||
				node.NodeType == XmlNodeType.EntityReference;
		}

		public virtual XmlNodeType[] AcceptNodeTypes
		{
			get { return acceptNodes; }
		}

		/// <summary>
		/// Accepts the specified node.
		/// Check if node has the same name as the processor and the node.NodeType
		/// is in the AcceptNodeTypes List
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public virtual bool Accept(XmlNode node)
		{
			return node.Name == Name && Array.IndexOf(AcceptNodeTypes, node.NodeType) != -1;
		}

		public abstract void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine);

		/// <summary>
		/// Convert and return child parameter into an XmlElement
		/// An exception will be throw in case the child node cannot be converted
		/// </summary>
		/// <param name="element">Parent node</param>
		/// <param name="child">Node to be converted</param>
		/// <returns>child node as XmlElement</returns>
		protected XmlElement GetNodeAsElement(XmlElement element, XmlNode child)
		{
			XmlElement result = child as XmlElement;

			if (result == null)
			{
				throw new XmlProcessorException("{0} expects XmlElement found {1}", element.Name, child.NodeType);
			}

			return result;
		}

		protected String GetRequiredAttribute(XmlElement element, String attribute)
		{
			String attValue = element.GetAttribute(attribute).Trim();

			if (attValue == "")
			{
				throw new XmlProcessorException("'{0}' requires a non empty '{1}' attribute", element.Name, attribute);
			}

			return attValue;
		}

		protected XmlNode ImportNode(XmlNode targetElement, XmlNode node)
		{
			return targetElement.OwnerDocument == node.OwnerDocument ?
				node : targetElement.OwnerDocument.ImportNode(node, true);
		}

		protected void AppendChild(XmlNode element, XmlNodeList nodes)
		{
			DefaultXmlProcessorNodeList childNodes = new DefaultXmlProcessorNodeList(nodes);

			while(childNodes.MoveNext())
			{
				AppendChild(element, childNodes.Current);
			}
		}

		protected void AppendChild(XmlNode element, string text)
		{
			AppendChild(element, CreateText(element, text));
		}

		protected void AppendChild(XmlNode element, XmlNode child)
		{
			element.AppendChild(ImportNode(element, child));
		}

		protected void ReplaceNode(XmlNode element, XmlNode newNode, XmlNode oldNode)
		{
			if (newNode == oldNode) return;

			XmlNode importedNode = ImportNode(element, newNode);

			element.ReplaceChild(importedNode, oldNode);
		}

		protected XmlText CreateText(XmlNode node, string content)
		{
			return node.OwnerDocument.CreateTextNode(content);
		}

		protected XmlDocumentFragment CreateFragment(XmlNode parentNode)
		{
			return parentNode.OwnerDocument.CreateDocumentFragment();
		}

		protected bool IsTextNode(XmlNode node)
		{
			return node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA;
		}

		protected void ReplaceItself(XmlNode newNode, XmlNode oldNode)
		{
			ReplaceNode(oldNode.ParentNode, newNode, oldNode);
		}

		protected void RemoveItSelf(XmlNode node)
		{
			node.ParentNode.RemoveChild(node);
		}

		protected void MoveChildNodes(XmlDocumentFragment fragment, XmlElement element)
		{
			while(element.ChildNodes.Count > 0)
			{
				fragment.AppendChild(element.ChildNodes[0]);
			}
		}
	}
}
