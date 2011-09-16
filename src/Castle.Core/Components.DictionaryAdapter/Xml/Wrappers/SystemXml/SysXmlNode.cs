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
				throw new ArgumentNullException("node");
			if (type == null)
				throw new ArgumentNullException("type");

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

		public virtual string LocalName
		{
			get { return node.LocalName; }
		}

		public virtual string NamespaceUri
		{
			get { return node.NamespaceURI; }
		}

		public virtual string XsiType
		{
			get { return IsElement ? node.GetXsiType() : null; }
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
			get { return IsElement && node.IsXsiNil(); }
			set { RequireElement(); node.SetXsiNil(value); }
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

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(this, clrType);
		}

		public IXmlCursor SelectChildren(IXmlTypeMap knownTypes, CursorFlags flags)
		{
			return new SysXmlCursor(this, knownTypes, flags);
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

		public IXmlCursor Select(ICompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor) new XPathMutableCursor (this, path, knownTypes, flags)
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, knownTypes, flags);
		}

		public object Evaluate(ICompiledPath path)
		{
			return node.CreateNavigator().Evaluate(path.Expression);
		}

		public XmlNode GetNode()
		{
			return node;
		}

		public virtual void Realize()
		{
			// Default nodes are realized already
		}

		public void Coerce(IXmlName knownType)
		{
			if (IsElement)
				CoerceElement  (knownType);
			else
				CoerceAttribute(knownType);
		}

		private void CoerceElement(IXmlName knownType)
		{
			var oldNode = (XmlElement) node;
			var parent  = oldNode.ParentNode;
			var uri     = GetEffectiveNamespaceUri(parent, knownType);

			if (HasName(knownType.LocalName, uri))
				ChangeType(knownType);
			else
			{
				var newNode = CreateElementCore(parent, knownType, uri);
				parent.ReplaceChild(newNode, oldNode);
			}
		}

		private void CoerceAttribute(IXmlName knownType)
		{
			var oldNode = (XmlAttribute) node;
			var parent  = oldNode.OwnerElement;
			var uri     = GetEffectiveNamespaceUri(parent, knownType);

			if (HasName(knownType.LocalName, uri))
				RequireNoXsiType(knownType);
			else
			{
				var newNode    = CreateAttributeCore(parent, knownType, uri);
				var attributes = parent.Attributes;
				attributes.RemoveNamedItem(newNode.LocalName, newNode.NamespaceURI);
				attributes.InsertBefore(newNode, oldNode);
				attributes.Remove(oldNode);
			}
		}

		private static string GetEffectiveNamespaceUri(XmlNode parent, IXmlName knownType)
		{
			var uri = knownType.NamespaceUri;
			return
				(uri    != null) ? uri :
				(parent != null) ? parent.NamespaceURI :
				string.Empty;
		}

		protected void CreateElement(IXmlName knownType, XmlNode position)
		{
			var parent = node;
			var namespaceUri = GetEffectiveNamespaceUri(parent, knownType);
			var element = CreateElementCore(parent, knownType, namespaceUri);
			parent.InsertBefore(element, position);
		}

		protected void CreateAttribute(IXmlName knownType, XmlNode position)
		{
			RequireNoXsiType(knownType);
			var parent = node;
			var namespaceUri = GetEffectiveNamespaceUri(parent, knownType);
			var attribute = CreateAttributeCore(parent, knownType, namespaceUri);
			parent.Attributes.InsertBefore(attribute, (XmlAttribute) position);
		}

		private XmlElement CreateElementCore(XmlNode parent, IXmlName knownType, string namespaceUri)
		{
			var document = parent.OwnerDocument ?? (XmlDocument) parent;
			var prefix   = parent.GetPrefixOfNamespace(namespaceUri);
			var element  = document.CreateElement(prefix, knownType.LocalName, namespaceUri);

			if (knownType.XsiType != null)
				element.SetXsiType(knownType.XsiType);

			node = element;
//			type = knownType.ClrType; // TODO
			return element;
		}

		private XmlAttribute CreateAttributeCore(XmlNode parent, IXmlName knownType, string namespaceUri)
		{
			var document  = parent.OwnerDocument ?? (XmlDocument) parent;
			var prefix    = parent.GetPrefixOfNamespace(namespaceUri);
			var attribute = document.CreateAttribute(prefix, knownType.LocalName, namespaceUri);

			node = attribute;
//			type = knownType.ClrType; // TODO
			return attribute;
		}

		private void ChangeType(IXmlName knownType)
		{
			node.SetXsiType(knownType.XsiType);
//			type = knownType.ClrType; // TODO
		}

		private void RequireNoXsiType(IXmlName knownType)
		{
			if (knownType.XsiType != null)
				throw Error.CannotSetXsiTypeOnAttribute();
//			type = knownType.ClrType; // TODO
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

		private bool HasName(string localName, string namespaceUri)
		{
			return node.LocalName    == localName
				&& node.NamespaceURI == namespaceUri;
		}

		private void RequireElement()
		{
			if (!IsElement)
				throw Error.OperationNotValidOnAttribute();
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
