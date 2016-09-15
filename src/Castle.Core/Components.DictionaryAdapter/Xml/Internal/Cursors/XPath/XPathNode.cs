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

	public class XPathNode : XmlNodeBase, IXmlNode, IRealizable<XPathNavigator>
		, IRealizable<XmlNode>
	{
		protected XPathNavigator node;
		protected readonly CompiledXPath xpath;

		protected XPathNode(CompiledXPath path, IXmlNamespaceSource namespaces, IXmlNode parent)
			: base(namespaces, parent)
		{
			this.xpath = path;
		}

		public XPathNode(XPathNavigator node, Type type, IXmlNamespaceSource namespaces)
			: this(null, namespaces, null)
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

		XPathNavigator IRealizable<XPathNavigator>.Value
		{
			get { Realize(); return node; }
		}

		XmlNode IRealizable<XmlNode>.Value
		{
			get { Realize(); return (XmlNode) node.UnderlyingObject; }
		}

		public override CompiledXPath Path
		{
			get { return xpath; }
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
			get { return node.NodeType == XPathNodeType.Element; }
		}

		public virtual bool IsAttribute
		{
			get { return node.NodeType == XPathNodeType.Attribute; }
		}

		public virtual bool IsNil
		{
			get { return this.IsXsiNil(); }
			set { this.SetXsiNil(value); }
		}

		public virtual string Value
		{
			get { return node.Value; }
			set { var nil = (value == null); IsNil = nil; if (!nil) node.SetValue(value); }
		}

		public virtual string Xml
		{
			get { return node.OuterXml; }
		}

		public string GetAttribute(XmlName name)
		{
			if (!IsReal || !node.MoveToAttribute(name.LocalName, name.NamespaceUri))
				return null;

			var value = node.Value;
			node.MoveToParent();
			return string.IsNullOrEmpty(value) ? null : value;
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

			if (node.MoveToAttribute(name.LocalName, name.NamespaceUri))
			{
				node.SetValue(value);
				node.MoveToParent();
			}
			else
			{
				var prefix = Namespaces.GetAttributePrefix(this, name.NamespaceUri);
				node.CreateAttribute(prefix, name.LocalName, name.NamespaceUri, value);
			}
		}

		private void ClearAttribute(XmlName name)
		{
			if (IsReal && node.MoveToAttribute(name.LocalName, name.NamespaceUri))
				node.DeleteSelf();
		}

		public string LookupPrefix(string namespaceUri)
		{
			return node.LookupPrefix(namespaceUri);
		}

		public string LookupNamespaceUri(string prefix)
		{
			return node.LookupNamespace(prefix);
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			var target
				= root        ? node.GetRootElement()
				: IsElement   ? node
				: IsAttribute ? node.GetParent()
				: node.GetRootElement();

			target.CreateAttribute(Xmlns.Prefix, prefix, Xmlns.NamespaceUri, namespaceUri);
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
			var sysXmlNode = node.AsRealizable<XmlNode>();
			if (sysXmlNode != null)
				return sysXmlNode.IsReal
					&& sysXmlNode.Value == this.node.UnderlyingObject;

			var xPathNode = node.AsRealizable<XPathNavigator>();
			if (xPathNode != null)
				return xPathNode.IsReal
					&& xPathNode.Value.IsSamePosition(this.node);

			return false;
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

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor) new XPathMutableCursor (this, path, includedTypes, namespaces, flags)
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, includedTypes, namespaces, flags);
		}

		public virtual object Evaluate(CompiledXPath path)
		{
			return node.Evaluate(path.Path);
		}

		public virtual XmlReader ReadSubtree()
		{
			return node.ReadSubtree();
		}

		public virtual XmlWriter WriteAttributes()
		{
			return node.CreateAttributes();
		}

		public virtual XmlWriter WriteChildren()
		{
			return node.AppendChild();
		}

		public virtual void Clear()
		{
			node.DeleteChildren();
		}
	}
}
#endif
