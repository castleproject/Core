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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Xml.Serialization;

    public class XmlSubtreeReader : XmlReader
    {
        private readonly string rootLocalName;
		private readonly string rootNamespaceURI;
		private string underlyingNamespaceURI;
        private XmlReader reader;

		public XmlSubtreeReader(IXmlNode node, XmlRootAttribute root)
			: this(node, root.ElementName, root.Namespace) { }

        public XmlSubtreeReader(IXmlNode node, string rootLocalName, string rootNamespaceUri)
        {
            if (null == node)
                throw Error.ArgumentNull("node");
            if (null == rootLocalName)
                throw Error.ArgumentNull("rootLocalName");

            this.reader           = node.ReadSubtree();
            this.rootLocalName    = reader.NameTable.Add(rootLocalName);
			this.rootNamespaceURI = rootNamespaceUri ?? string.Empty;
        }

        protected override void Dispose(bool managed)
        {
            try { if (managed) DisposeReader(); }
            finally { base.Dispose(managed); }
        }

        private void DisposeReader()
        {
            IDisposable value = Interlocked.Exchange(ref reader, null);
            if (null != value) value.Dispose();
        }

        public bool IsDisposed
        {
            get { return null == reader; }
        }

        private void RequireNotDisposed()
        {
            if (IsDisposed)
                throw Error.ObjectDisposed("XmlSubtreeReader");
        }

        protected XmlReader Reader
        {
            get { RequireNotDisposed(); return reader; }
        }

        public override ReadState ReadState
        {
            get { return IsDisposed ? ReadState.Closed : reader.ReadState; }
        }

        public override int Depth
        {
            get { return Reader.Depth; }
        }

        public override XmlNodeType NodeType
        {
            get { return Reader.NodeType; }
        }

        public bool IsAtRootElement
        {
            get
            {
                RequireNotDisposed();
                return
                    reader.ReadState == ReadState.Interactive &&
                    reader.Depth == 0 &&
                    (
                        reader.NodeType == XmlNodeType.Element ||
                        reader.NodeType == XmlNodeType.EndElement
                    );
            }
        }

        public override bool EOF
        {
            get { return Reader.EOF; }
        }

        public override string Prefix
        {
            get { return Reader.Prefix; }
        }

        public override string LocalName
        {
            get { return IsAtRootElement ? rootLocalName : Reader.LocalName; }
        }

        public override string NamespaceURI
        {
            get { return IsAtRootElement ? CaptureNamespaceUri() : TranslateNamespaceURI(); }
        }

		private string CaptureNamespaceUri()
		{
			if (underlyingNamespaceURI == null)
				underlyingNamespaceURI = Reader.NamespaceURI;
			return rootNamespaceURI;
		}

		private string TranslateNamespaceURI()
		{
			var actualNamespaceURI = Reader.NamespaceURI;
			return actualNamespaceURI == underlyingNamespaceURI
				? rootNamespaceURI
				: actualNamespaceURI;
		}

#if !DOTNET40
		// Virtual in .NET 4.0, abstract in .NET 3.5
		// Use default implementation from .NET 4.0
		public override bool HasValue
		{
			get { return 0UL != (HasValueMask & (1UL << ((int)NodeType & 31))); }
		}
		private const ulong HasValueMask = 0x0002659CU;
#endif

        public override string Value
        {
            get { return Reader.Value; }
        }

        public override bool IsEmptyElement
        {
            get { return Reader.IsEmptyElement; }
        }

        public override int AttributeCount
        {
            get { return Reader.AttributeCount; }
        }

        public override string BaseURI
        {
            get { return Reader.BaseURI; }
        }

        public override XmlNameTable NameTable
        {
            get { return Reader.NameTable; }
        }

        public override bool Read()
        {
            return Reader.Read();
        }

        public override bool MoveToElement()
        {
            return Reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return Reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return Reader.MoveToNextAttribute();
        }

        public override bool MoveToAttribute(string name)
        {
            return Reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return Reader.MoveToAttribute(name, ns);
        }

        public override bool ReadAttributeValue()
        {
            return Reader.ReadAttributeValue();
        }

        public override string GetAttribute(int i)
        {
            return Reader.GetAttribute(i);
        }

        public override string GetAttribute(string name)
        {
            return Reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return Reader.GetAttribute(name, namespaceURI);
        }

        public override string LookupNamespace(string prefix)
        {
            return Reader.LookupNamespace(prefix);
        }

        public override void ResolveEntity()
        {
            Reader.ResolveEntity();
        }

        public override void Close()
        {
            if (!IsDisposed) reader.Close();
        }
    }
}
#endif
