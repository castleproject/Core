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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public class SysXmlNode : XmlNodeBase, IXmlNode,
		IRealizable<XmlNode>,
		IRealizable<XPathNavigator>
	{
		protected XmlNode node;

		protected SysXmlNode(IXmlNamespaceSource namespaces, IXmlNode parent)
			: base(namespaces, parent) { }

		public SysXmlNode(XmlNode node, Type type, IXmlNamespaceSource namespaces)
			: base(namespaces, null)
		{
			if (node == null)
				throw Error.ArgumentNull("node");
			if (type == null)
				throw Error.ArgumentNull("type");

			this.node = node;
			this.type = type;
		}

		public object UnderlyingObject
		{
			get { return node; }
		}

		XmlNode IRealizable<XmlNode>.Value
		{
			get { Realize(); return node; }
		}

		XPathNavigator IRealizable<XPathNavigator>.Value
		{
			get { Realize(); return node.CreateNavigator(); }
		}

		public virtual XmlName Name
		{
			get { return new XmlName(node.LocalName, node.NamespaceURI); }
		}

		public virtual XmlName XsiType
		{
			get { return this.GetXsiType(); }
		}

		public virtual bool IsElement
		{
			get { return node.NodeType == XmlNodeType.Element; }
		}

		public virtual bool IsAttribute
		{
			get { return node.NodeType == XmlNodeType.Attribute; }
		}

		public virtual bool IsNil
		{
			get { return this.IsXsiNil(); }
			set { this.SetXsiNil(value); }
		}

		public virtual string Value
		{
			get { return node.InnerText; }
			set { var nil = (value == null); IsNil = nil; if (!nil) node.InnerText = value; }
		}

		public virtual string Xml
		{
			get { return node.OuterXml; }
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
			var sysXmlNode = node.AsRealizable<XmlNode>();
			if (sysXmlNode != null)
				return sysXmlNode.IsReal
					&& sysXmlNode.Value == this.node;

			var xPathNode = node.AsRealizable<XPathNavigator>();
			if (xPathNode != null)
				return xPathNode.IsReal
					&& xPathNode.Value.UnderlyingObject == this.node;

			return false;
		}

		public string GetAttribute(XmlName name)
		{
			if (!IsReal)
				return null;

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

		public void SetAttribute(XmlName name, string value)
		{
			if (string.IsNullOrEmpty(value))
				ClearAttribute(name);
			else
				SetAttributeCore(name, value);
		}

		private void SetAttributeCore(XmlName name, string value)
		{
			if (!IsElement)
				throw Error.CannotSetAttribute(this);

			Realize();

			var element = node as XmlElement;
			if (element == null)
				throw Error.CannotSetAttribute(this);

			var attribute = element.GetAttributeNode(name.LocalName, name.NamespaceUri);
			if (attribute == null)
			{
				var prefix = Namespaces.GetAttributePrefix(this, name.NamespaceUri);
				attribute = element.OwnerDocument.CreateAttribute(prefix, name.LocalName, name.NamespaceUri);
				element.SetAttributeNode(attribute);
			}
			attribute.Value = value;
		}

		private void ClearAttribute(XmlName name)
		{
			if (!IsReal)
				return;

			var element = node as XmlElement;
			if (element == null)
				return;

			element.RemoveAttribute(name.LocalName, name.NamespaceUri);
			return;
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

		public virtual IXmlNode Save()
		{
			return this;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return new SysXmlCursor(this, knownTypes, namespaces, flags);
		}

		public IXmlIterator SelectSubtree()
		{
			return new SysXmlSubtreeIterator(this, Namespaces);
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
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, includedTypes, namespaces, flags);
		}

		public virtual object Evaluate(CompiledXPath path)
		{
			return node.CreateNavigator().Evaluate(path.Path);
		}

		public XmlNode GetNode()
		{
			return node;
		}

		public void Clear()
		{
			if (IsElement)
			{
				ClearAttributes();
			}
			else if (IsAttribute)
			{
				Value = string.Empty;
				return;
			}
			ClearChildren();
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

		private void ClearChildren()
		{
			XmlNode next;
			for (var child = node.FirstChild; child != null; child = next)
			{
				next = child.NextSibling;
				node.RemoveChild(child);
			}
		}
	}
}
#endif
