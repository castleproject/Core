// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.XPath;
	using System.Xml.Xsl;

	public class CompiledXPathNode
	{
		private string prefix;
		private string localName;
		private bool isAttribute;
		private XPathExpression value;
		private CompiledXPathNode next;
		private IList<CompiledXPathNode> dependencies;

		internal CompiledXPathNode()
		{
			dependencies = new List<CompiledXPathNode>();
		}

		public string Prefix
		{
			get { return prefix; }
			internal set { prefix = value; }
		}

		public string LocalName
		{
			get { return localName; }
			internal set { localName = value; }
		}

		public bool IsAttribute
		{
			get { return isAttribute; }
			internal set { isAttribute = value; }
		}

		public XPathExpression Value
		{
			get { return value; }
			internal set { this.value = value; }
		}

		public CompiledXPathNode NextNode
		{
			get { return next; }
			internal set { next = value; }
		}

		public IList<CompiledXPathNode> Dependencies
		{
			get { return dependencies; }
		}

		internal virtual void Prepare()
		{
			dependencies = Array.AsReadOnly(dependencies.ToArray());

			foreach (var child in dependencies)
				child.Prepare();
		}

		internal virtual void SetContext(XsltContext context)
		{
			if (value != null)
				value.SetContext(context);
		}
	}
}
#endif
