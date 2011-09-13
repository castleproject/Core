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

	public class XPathNode : XmlType, IXmlNode, ILazy<XPathNavigator>
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

		public override Type ClrType
		{
			get { return type; }
		}

		Type IXmlTypeMap.BaseType
		{
			get { return ClrType; }
		}

		public override string LocalName
		{
			get { return node.LocalName; }
		}

		public override string NamespaceUri
		{
			get { return node.NamespaceURI; }
		}

		public override string XsiType
		{
			get { return IsElement ? node.GetXsiType() : null; }
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

		public IXmlCursor SelectSelf()
		{
			return new XmlSelfCursor(this);
		}

		public IXmlCursor SelectChildren(IXmlTypeMap knownTypes, CursorFlags flags)
		{
#if !SILVERLIGHT
			return new SysXmlCursor(this, knownTypes, flags);
#else
			// TODO: XNode-based
#endif
		}

		public IXmlCursor Select(ICompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			return flags.SupportsMutation()
				? (IXmlCursor) new XPathMutableCursor (this, path, knownTypes, flags)
				: (IXmlCursor) new XPathReadOnlyCursor(this, path, knownTypes, flags);
		}

		public virtual object Evaluate(ICompiledPath path)
		{
			return node.Evaluate(path.Expression);
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

		public virtual void Coerce(IXmlType xmlType)
		{
			var xsiType = (xmlType.ClrType == ClrType)
				? null
				: xmlType.XsiType;

			node.SetXsiType(xsiType);
		}

		public virtual void Clear()
		{
			node.DeleteChildren();
		}

		private void RequireElement()
		{
			if (!IsElement)
				throw Error.OperationNotValidOnAttribute();
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
