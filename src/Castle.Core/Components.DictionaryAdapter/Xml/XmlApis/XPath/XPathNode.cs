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

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public class XPathNode : IXmlNode, ILazy<XPathNavigator> //, IXmlTypeMap
#if !SILVERLIGHT
		, ILazy<XmlNode>
#endif
	{
		protected XPathNavigator node;
		protected Type           type;

		protected XPathNode() { }

		public XPathNode(XPathNavigator node, Type type)
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

		public virtual Type ClrType
		{
			get { return type; }
		}

		public virtual XmlName Name
		{
			get { return new XmlName(node.LocalName, node.NamespaceURI); }
		}

		public virtual XmlName XsiType
		{
			get { return IsElement ? node.GetXsiType() : XmlName.Empty; }
		}

		public virtual bool IsElement
		{
			get { return node.NodeType == XPathNodeType.Element; }
		}

		public virtual bool IsAttribute
		{
			get { return node.NodeType == XPathNodeType.Attribute; }
		}

		public virtual bool IsRoot
		{
			get { return node.NodeType == XPathNodeType.Root; }
		}

		public virtual bool IsNil
		{
			get { return IsElement && node.IsXsiNil(); }
			set { RequireElement(); node.SetXsiNil(value); }
		}

		public virtual string Value
		{
			get { return node.Value; }
			set { node.SetValue(value); }
		}

		public virtual string Xml
		{
			get { return node.OuterXml; }
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
			if (root)
			{
				node = node.Clone();
				node.MoveToRoot();
				node.MoveToFirstChild();
			}

			node.CreateAttribute(Xmlns.Prefix, prefix, Xmlns.NamespaceUri, namespaceUri);
		}

		public bool PositionEquals(IXmlNode node)
		{
			var xPathNode = node as ILazy<XPathNavigator>;
			if (xPathNode != null)
				return xPathNode.HasValue
					&& xPathNode.Value.IsSamePosition(this.node);
#if !SILVERLIGHT
			var sysXmlNode = node as ILazy<XmlNode>;
			if (sysXmlNode != null)
				return sysXmlNode.HasValue
					&& sysXmlNode.Value == this.node.UnderlyingObject;
#endif
			// TODO: XNode-based

			return false;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
#if !SILVERLIGHT
			return new SysXmlCursor(this, knownTypes, namespaces, flags);
#else
			// TODO: XNode-based
#endif
		}

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor) new XPathMutableCursor (this, path, includedTypes, flags)
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, includedTypes, flags);
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

		public virtual void Realize()
		{
			// Default nodes are realized already
		}

		public virtual void Clear()
		{
			node.DeleteChildren();
		}

		private void RequireElement()
		{
			if (!IsElement)
				throw Error.CannotSetXsiNilOnAttribute(this);
		}

		bool ILazy<XPathNavigator>.HasValue
		{
			get { return Exists; }
		}

		bool ILazy<XmlNode>.HasValue
		{
			get { return Exists; }
		}

		XPathNavigator ILazy<XPathNavigator>.Value
		{
			get { Realize(); return node; }
		}

		XmlNode ILazy<XmlNode>.Value
		{
			get { Realize(); return (XmlNode) node.UnderlyingObject; }
		}
	}
}
#endif
