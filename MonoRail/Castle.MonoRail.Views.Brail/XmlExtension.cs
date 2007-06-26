using System.Collections;
using System.IO;
using System.Xml;
using Boo.Lang;

namespace Castle.MonoRail.Views.Brail
{
    public class XmlExtension : IDslLanguageExtension
    {
        readonly private TextWriter _output = null;
        private readonly XmlWriter writer;

        public XmlExtension(TextWriter output)
        {
            _output = output;
            this.writer = XmlWriter.Create(_output);
        }

        public TextWriter Output
        {
            get { return _output; }
        }

        public void text(string text)
        {
            writer.WriteString(text);
        }

        private void BlockTag(string tag, IDictionary attributes, ICallable block)
        {
            writer.WriteStartElement(tag);

            if (null != attributes)
            {
                foreach (DictionaryEntry entry in attributes)
                {
                    writer.WriteAttributeString((string)entry.Key,(string)entry.Value);
                }
            }

            if (block != null)
            {
                block.Call(null);
            }
            writer.WriteEndElement();
        }

        public void Tag(string name)
        {
            BlockTag(name, null, null);
        }

        public void Tag(string name, ICallable block)
        {
            BlockTag(name, null, block);
        }

        public void Tag(string name, IDictionary attributes, ICallable block)
        {
            BlockTag(name, attributes, block);
        }

        public void Flush()
        {
            writer.Flush();
        }
    }
}