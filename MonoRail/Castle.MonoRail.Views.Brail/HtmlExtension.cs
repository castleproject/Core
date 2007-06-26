using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Boo.Lang;
using System.Collections;

namespace Castle.MonoRail.Views.Brail
{
    public class HtmlExtension : IDslLanguageExtension
    {
        readonly private TextWriter _output = null;
        public HtmlExtension(TextWriter output)
        {
            _output = output;
        }

        public TextWriter Output
        {
            get { return _output; }
        }

        private void BlockTag(string tag, IDictionary attributes, ICallable block)
        {
            Output.Write("<{0}", tag);

            List<string> attributeValues = new List<string>();

            if (null != attributes)
            {
                foreach (DictionaryEntry entry in attributes)
                {
                    attributeValues.Add(string.Format("{0}=\"{1}\"", entry.Key, entry.Value));
                }
            }

            if (0 != attributeValues.Count)
            {
                Output.Write(" ");
                Output.Write(string.Join(" ", attributeValues.ToArray()));
            }

            Output.Write(">");
            if(block!=null)
            {
                block.Call(null);
            }
            Output.Write("</{0}>", tag);
        }

        public void html(ICallable block)
        {
            BlockTag("html", null, block);
        }

        public void text(string value)
        {
            Output.Write(value);
        }

        public void p(ICallable block)
        {
            p(null, block);
        }

        public void p(IDictionary attributes, ICallable block)
        {
            BlockTag("p", attributes, block);
        }

        public void Tag(string name)
        {
            BlockTag(name,null,null);
        }

        public void Tag(string name, ICallable block)
        {
            BlockTag(name, null, block);
        }

        public void Tag(string name, IDictionary attributes, ICallable block)
        {
            BlockTag(name, attributes,block);
        }

        public void Flush()
        {
            //no op
        }
    }
}
