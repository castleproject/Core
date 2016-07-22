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

    public class XmlSelfCursor : IXmlCursor
    {
        private readonly IXmlNode node;
		private readonly Type clrType;
        private int position;

        public XmlSelfCursor(IXmlNode node, Type clrType)
        {
            this.node    = node;
			this.clrType = clrType;
			Reset();
        }

		public CursorFlags Flags
		{
			get { return node.IsAttribute ? CursorFlags.Attributes : CursorFlags.Elements; }
		}

		public CompiledXPath Path
		{
			get { return node.Path; }
		}

		public XmlName Name
		{
			get { return node.Name; }
		}

		public XmlName XsiType
		{
			get { return node.XsiType; }
		}

		public Type ClrType
		{
			get { return clrType ?? node.ClrType; }
		}

		public bool IsReal
		{
			get { return node.IsReal; }
		}

		public bool IsElement
		{
			get { return node.IsElement; }
		}

		public bool IsAttribute
		{
			get { return node.IsAttribute; }
		}

		public bool IsNil
		{
			get { return node.IsNil; }
			set { throw Error.NotSupported(); }
		}

		public string Value
		{
			get { return node.Value; }
			set { node.Value = value; }
		}

		public string Xml
		{
			get { return node.Xml; }
		}

		public IXmlNode Parent
		{
			get { return node.Parent; }
		}

		public IXmlNamespaceSource Namespaces
		{
			get { return node.Namespaces; }
		}

		public object UnderlyingObject
		{
			get { return node.UnderlyingObject; }
		}

		public bool UnderlyingPositionEquals(IXmlNode node)
		{
			return this.node.UnderlyingPositionEquals(node);
		}

		public IRealizable<T> AsRealizable<T>()
		{
			return node.AsRealizable<T>();
		}

		public void Realize()
		{
			node.Realize();
		}

		public event EventHandler Realized
		{
			add    { node.Realized += value; }
			remove { node.Realized -= value; }
		}

		public string GetAttribute(XmlName name)
		{
			return node.GetAttribute(name);
		}

		public void SetAttribute(XmlName name, string value)
		{
			node.SetAttribute(name, value);
		}

		public string LookupPrefix(string namespaceUri)
		{
			return node.LookupPrefix(namespaceUri);
		}

		public string LookupNamespaceUri(string prefix)
		{
			return node.LookupNamespaceUri(prefix);
		}

		public void DefineNamespace(string prefix, string namespaceUri, bool root)
		{
			node.DefineNamespace(prefix, namespaceUri, root);
		}

        public bool MoveNext()
        {
            return 0 == ++position;
        }

		public void MoveToEnd()
		{
			position = 1;
		}

		public void Reset()
		{
			position = -1;
		}

		public void MoveTo(IXmlNode position)
		{
			if (position != node)
				throw Error.NotSupported();
		}

		public IXmlNode Save()
		{
			return position == 0
				? new XmlSelfCursor(node.Save(), clrType) { position = 0 }
				: this;
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			return new XmlSelfCursor(node, clrType);
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return node.SelectChildren(knownTypes, namespaces, flags);
		}

		public IXmlIterator SelectSubtree()
		{
			return node.SelectSubtree();
		}

		public IXmlCursor Select(CompiledXPath path, IXmlIncludedTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			return node.Select(path, knownTypes, namespaces, flags);
		}

		public object Evaluate(CompiledXPath path)
		{
			return node.Evaluate(path);
		}

		public XmlReader ReadSubtree()
		{
			return node.ReadSubtree();
		}

		public XmlWriter WriteAttributes()
		{
			return node.WriteAttributes();
		}

		public XmlWriter WriteChildren()
		{
			return node.WriteChildren();
		}

		public void MakeNext(Type type)
		{
			if (!MoveNext())
				throw Error.NotSupported();
		}

		public void Create(Type type)
		{
			throw Error.NotSupported();
		}

		public void Coerce(Type type)
		{
			// Do nothing
		}

		public void Clear()
		{
			node.Clear();
		}

		public void Remove()
		{
			// Do nothing
		}

		public void RemoveAllNext()
		{
			// Do nothing
		}
	}
}
#endif
