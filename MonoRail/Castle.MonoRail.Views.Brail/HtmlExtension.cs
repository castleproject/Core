// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Boo.Lang;

	public class HtmlExtension : IDslLanguageExtension
	{
		private readonly TextWriter _output = null;

		public HtmlExtension(TextWriter output)
		{
			_output = output;
		}

		public TextWriter Output
		{
			get { return _output; }
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
			//no op
		}

		#endregion

		private void BlockTag(string tag, IDictionary attributes, ICallable block)
		{
			Output.Write("<{0}", tag);

			System.Collections.Generic.List<string> attributeValues = new System.Collections.Generic.List<string>();

			if (null != attributes)
			{
				foreach(DictionaryEntry entry in attributes)
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
			if (block != null)
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
	}
}