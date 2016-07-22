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

	// DEPTH:               ROOT STATES:
    // 0: <?xml>            [Start, Prolog]
    // 1: <Root>            [Element, Attribute]
    // 2:   <Foo>...</Foo>  [Content]

    public class XmlSubtreeWriter : XmlWriter
    {
        private readonly IXmlNode node;
        private XmlWriter rootWriter;
        private XmlWriter childWriter;
        private WriteState state;
        private int depth;

        public XmlSubtreeWriter(IXmlNode node)
        {
            if (node == null)
                throw Error.ArgumentNull("node");

            this.node = node;
        }

        protected override void Dispose(bool managed)
        {
            try
            {
                if (managed)
                {
                    Reset(WriteState.Closed);
                    DisposeWriter(ref rootWriter);
                    DisposeWriter(ref childWriter);
                }
            }
            finally
			{
				base.Dispose(managed);
			}
        }

        private void DisposeWriter(ref XmlWriter writer)
        {
            var value = Interlocked.Exchange(ref writer, null);
            if (null != value) value.Close();
        }

        private XmlWriter RootWriter
        {
            get { return rootWriter ?? (rootWriter = node.WriteAttributes()); }
        }

        private XmlWriter ChildWriter
        {
            get { return childWriter ?? (childWriter = node.WriteChildren()); }
        }

        private bool IsInRootAttribute
        {
            get { return state == WriteState.Attribute; }
        }

        private bool IsInRoot
        {
            get { return depth > 0; }
        }

        private bool IsInChild
        {
            get { return depth > 1; }
        }

        public override WriteState WriteState
        {
            get { return (IsInRoot && state == WriteState.Content) ? childWriter.WriteState : state; }
        }

        public override void WriteStartDocument(bool standalone)
        {
            WriteStartDocument();
        }

        public override void WriteStartDocument()
        {
            RequireState(WriteState.Start);
            state = WriteState.Prolog;
            // (do not write anything)
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            RequireState(WriteState.Start, WriteState.Prolog);
            state = WriteState.Prolog;
            // (do not write anything)
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            try
            {
                if (IsInRoot)
                {
                    ChildWriter.WriteStartElement(prefix, localName, ns);
                    state = WriteState.Content;
                }
                else // is in prolog
                {
                    RequireState(WriteState.Start, WriteState.Prolog);
                    node.Clear();
                    state = WriteState.Element;
                }
                depth++;
            }
            catch { Reset(WriteState.Error); throw; }
        }

        private void WriteEndElement(Action<XmlWriter> action)
        {
            try
            {
                if (IsInChild)
                {
                    action(ChildWriter);
                    state = WriteState.Content;
                }
                else // is in root (or prolog)
                {
                    RequireState(WriteState.Element, WriteState.Content);
                    state = WriteState.Prolog;
                }
                depth--;
            }
            catch { Reset(WriteState.Error); throw; }
        }

        public override void WriteEndElement()
        {
            WriteEndElement(w => w.WriteEndElement());
        }

        public override void WriteFullEndElement()
        {
            WriteEndElement(w => w.WriteFullEndElement());
        }

        private void WriteAttribute(Action<XmlWriter> action, WriteState entryState, WriteState exitState)
        {
            try
            {
                if (IsInChild)
                {
                    action(ChildWriter);
                }
                else // is in root (or prolog)
                {
                    RequireState(entryState);
                    action(RootWriter);
                    state = exitState;
                }
            }
            catch { Reset(WriteState.Error); throw; }
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            WriteAttribute(w => w.WriteStartAttribute(prefix, localName, ns),
                entryState: WriteState.Element,
                exitState:  WriteState.Attribute);
        }

        public override void WriteEndAttribute()
        {
            WriteAttribute(w => w.WriteEndAttribute(),
                entryState: WriteState.Attribute,
                exitState:  WriteState.Element);
        }

        private void WriteElementOrAttributeContent(Action<XmlWriter> action)
        {
            try
            {
                if (IsInChild)
                    action(ChildWriter);
                else if (IsInRootAttribute)
                    action(RootWriter);
                else // is in root (or prolog)
                {
                    RequireState(WriteState.Element, WriteState.Content);
                    action(ChildWriter);
                    state = WriteState.Content;
                }
            }
            catch { Reset(WriteState.Error); throw; }
        }

        public override void WriteString(string text)
        {
            WriteElementOrAttributeContent(w => w.WriteString(text));
        }

        public override void WriteCharEntity(char ch)
        {
            WriteElementOrAttributeContent(w => w.WriteCharEntity(ch));
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            WriteElementOrAttributeContent(w => w.WriteSurrogateCharEntity(lowChar, highChar));
        }

        public override void WriteEntityRef(string name)
        {
            WriteElementOrAttributeContent(w => w.WriteEntityRef(name));
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            WriteElementOrAttributeContent(w => w.WriteChars(buffer, index, count));
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            WriteElementOrAttributeContent(w => w.WriteBase64(buffer, index, count));
        }

        public override void WriteRaw(string data)
        {
            WriteElementOrAttributeContent(w => w.WriteRaw(data));
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            WriteElementOrAttributeContent(w => w.WriteRaw(buffer, index, count));
        }

        private void WriteElementContent(Action<XmlWriter> action)
        {
            try
            {
                RequireState(WriteState.Element, WriteState.Content);
                action(ChildWriter);
                state = WriteState.Content;
            }
            catch { Reset(WriteState.Error); throw; }
        }

        public override void WriteCData(string text)
        {
            WriteElementContent(w => w.WriteCData(text));
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            WriteElementContent(w => w.WriteProcessingInstruction(name, text));
        }

        public override void WriteComment(string text)
        {
            WriteElementContent(w => w.WriteComment(text));
        }

        public override void WriteWhitespace(string ws)
        {
            WriteElementContent(w => w.WriteWhitespace(ws));
        }

        private void WithWriters(Action<XmlWriter> action, bool worksIfClosed = false, WriteState? resetTo = null)
        {
            try
            {
                if (! worksIfClosed    ) RequireNotClosed();
                if (null != rootWriter ) action(rootWriter);
                if (null != childWriter) action(childWriter);
                if (null != resetTo     ) Reset(resetTo.Value);
            }
            catch { Reset(WriteState.Error); throw; }
        }

        public override void Flush()
        {
            WithWriters(w => w.Flush());
        }

        public override void WriteEndDocument()
        {
            WithWriters(w => w.WriteEndDocument(), resetTo: WriteState.Start);
        }

        public override void Close()
        {
            WithWriters(w => w.Close(), resetTo: WriteState.Closed, worksIfClosed: true);
        }

        public override string LookupPrefix(string ns)
        {
            // This one is the oddball
            try
            {
                string prefix;
                return
                    (  // Try child writer first
                        null != childWriter &&
                        null != (prefix = childWriter.LookupPrefix(ns))
                    ) ? prefix :
                    (  // Try root writer next
                        null != rootWriter &&
                        null != (prefix = rootWriter.LookupPrefix(ns))
                    ) ? prefix :
                    null;
            }
            catch { Reset(WriteState.Error); throw; }
        }

        private void RequireNotClosed()
        {
			if (state == WriteState.Closed || state == WriteState.Error)
				throw Error.InvalidOperation();
        }

        private void RequireState(WriteState state)
        {
            if (this.state != state)
				throw Error.InvalidOperation();
        }

        private void RequireState(WriteState state1, WriteState state2)
        {
            if (state != state1 && state != state2)
				throw Error.InvalidOperation();
        }

        private void Reset(WriteState state)
        {
            this.depth = 0;
            this.state = state;
        }
    }
}
#endif
