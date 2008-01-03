// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using System.Collections;
	using System.IO;
	using System.Xml;
	using Boo.Lang;

	public class XmlExtension : IDslLanguageExtension
	{
		private readonly TextWriter output = null;
		private readonly XmlWriter writer;

		public XmlExtension(TextWriter output)
		{
			this.output = output;
			writer = XmlWriter.Create(this.output);
		}

		public TextWriter Output
		{
			get { return output; }
		}

		#region IDslLanguageExtension Members

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

		#endregion

		public void text(string text)
		{
			writer.WriteString(text);
		}

		private void BlockTag(string tag, IDictionary attributes, ICallable block)
		{
			writer.WriteStartElement(tag);

			if (null != attributes)
			{
				foreach(DictionaryEntry entry in attributes)
				{
					writer.WriteAttributeString((string) entry.Key, (string) entry.Value);
				}
			}

			if (block != null)
			{
				block.Call(null);
			}
			writer.WriteEndElement();
		}
	}
}