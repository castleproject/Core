// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public class SysXmlNode : IXmlNode,
		ILazy<XmlNode>,
		ILazy<XPathNavigator>
	{
		protected XmlNode node;
		protected Type    type;

		protected SysXmlNode() { }

		public SysXmlNode(XmlNode node, Type type)
		{
			if (node == null)
				throw Error.ArgumentNull("node");
			if (type == null)
				throw Error.ArgumentNull("type");

			this.node = node;
			this.type = type;
		}

		public virtual bool Exists
		{
			get { return true; }
		}

		public virtual XmlName Name
		{
			get { return new XmlName(node.LocalName, node.NamespaceURI); }
		}

		public virtual XmlName XsiType
		{
			get { return this.GetXsiType(); }
		}

		public virtual Type ClrType
		{
			get { return type; }
		}

		public virtual bool IsElement
		{
			get { return node.NodeType == XmlNodeType.Element; }
		}

		public virtual bool IsAttribute
		{
			get { return node.NodeType == XmlNodeType.Attribute; }
		}

		public virtual bool IsRoot
		{
			get { return node.NodeType == XmlNodeType.Document; }
		}

		public virtual bool IsNil
		{
			get { return this.IsXsiNil(); }
		}

		public virtual string Value
		{
			get { return node.InnerText; }
			set { node.InnerText = value; }
		}

		public virtual string Xml
		{
			get { return node.OuterXml; }
		}

		public string GetAttribute(XmlName name)
		{
			var element = node as XmlElement;
			if (element == null)
				return null;

			var attribute = element.GetAttributeNode(name.LocalName, name.NamespaceUri);
			if (attribute == null)
				return null;

			var value = attribute.Value;
			if (string.IsNullOrEmpty(value))
				return null;

			return value;
		}

		public string LookupPrefix(string namespaceUri)
		{
			return node.GetPrefixOfNamespace(namespaceUri);
		}

		public string LookupNamespaceUri(string prefix)
		{
			return node.GetNamespaceOfPrefix(prefix);
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			var target = GetNamespaceTargetElement();
			if (target == null)
				throw Error.InvalidOperation();

			if (root)
				target = target.FindRoot();	

			target.DefineNamespace(prefix, namespaceUri);
		}

		private XmlElement GetNamespaceTargetElement()
		{
			var element = node as XmlElement;
			if (element != null)
				return element;

			var attribute = node as XmlAttribute;
			if (attribute != null)
				return attribute.OwnerElement;

			var document = node as XmlDocument;
			if (document != null)
				return document.DocumentElement;

			return null;
		}

		public bool PositionEquals(IXmlNode node)
		{
			var sysXmlNode = node as ILazy<XmlNode>;
			if (sysXmlNode != null)
				return sysXmlNode.HasValue
					&& sysXmlNode.Value == this.node;

			var xPathNode = node as ILazy<XPathNavigator>;
			if (xPathNode != null)
				return xPathNode.HasValue
					&& xPathNode.Value.UnderlyingObject == this.node;

			return false;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return new SysXmlCursor(this, knownTypes, namespaces, flags);
		}

		public XmlReader ReadSubtree()
		{
			return node.CreateNavigator().ReadSubtree();
		}

		public XmlWriter WriteAttributes()
		{
			return node.CreateNavigator().CreateAttributes();
		}

		public XmlWriter WriteChildren()
		{
			return node.CreateNavigator().AppendChild();
		}

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor) new XPathMutableCursor (this, path, includedTypes, namespaces, flags)
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, includedTypes, flags);
		}

		public object Evaluate(CompiledXPath path)
		{
			return node.CreateNavigator().Evaluate(path.Path);
		}

		public XmlNode GetNode()
		{
			return node;
		}

		public virtual void Realize()
		{
			// Default nodes are realized already
		}

		public void Clear()
		{
			if (IsElement)
				ClearAttributes();
			ClearChildren();
		}

		private void ClearChildren()
		{
			XmlNode next;
			for (var n = node.FirstChild; n != null; n = next)
			{
				next = n.NextSibling;
				node.RemoveChild(node);
			}
		}

		private void ClearAttributes()
		{
			var attributes = node.Attributes;
			var count = attributes.Count;
			while (count > 0)
			{
				var attribute = attributes[--count];
				if (!attribute.IsNamespace() && !attribute.IsXsiType())
					attributes.RemoveAt(count);
			}
		}

		public bool HasValue
		{
			get { return Exists; }
		}

		XmlNode ILazy<XmlNode>.Value
		{
			get { Realize(); return node; }
		}

		XPathNavigator ILazy<XPathNavigator>.Value
		{
			get { Realize(); return node.CreateNavigator(); }
		}
	}
}
#endif
