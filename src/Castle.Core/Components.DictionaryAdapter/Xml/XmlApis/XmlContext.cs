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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
#if !SL3
	using System.Xml.XPath;
	using System.Xml.Xsl;
#endif

#if !SL3
	public class XmlContext : XsltContext
#else
	public class XmlContext : XmlNamespaceManager
#endif
	{
		private readonly Dictionary<string, string> rootNamespaces;

		public XmlContext()
			: base(new NameTable())
		{
			rootNamespaces = new Dictionary<string, string>();
			AddNamespace(Xsd.Prefix, Xsd.NamespaceUri);
			AddNamespace(Xsi.Prefix, Xsi.NamespaceUri);
#if !SL3
			functions = new Dictionary<XmlName, IXsltContextFunction>();
			variables = new Dictionary<XmlName, IXsltContextVariable>();
#endif
		}

		public void AddNamespace(XmlNamespaceAttribute attribute)
		{
			var prefix       = attribute.Prefix;
			var namespaceUri = attribute.NamespaceUri;

			if (string.IsNullOrEmpty(namespaceUri))
				throw Error.InvalidNamespaceUri();

			if (attribute.Default)
				AddNamespace(string.Empty, namespaceUri);

			if (string.IsNullOrEmpty(prefix))
				return;

			AddNamespace(prefix, namespaceUri);

			if (attribute.Root)
				rootNamespaces.Add(prefix, namespaceUri);
		}

#if !SL3
		private XPathContext xPathContext;
		private readonly Dictionary<XmlName, IXsltContextFunction> functions;
		private readonly Dictionary<XmlName, IXsltContextVariable> variables;

		public XsltContext WithXPathSemantics
		{
			get { return xPathContext ?? (xPathContext = new XPathContext(this)); }
		}

		public override bool Whitespace
		{
			get { return true; }
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}

		public override int CompareDocument(string baseUriA, string baseUriB)
		{
			return StringComparer.Ordinal.Compare(baseUriA, baseUriB);
		}

		public void AddFunction(string prefix, string name, IXsltContextFunction function)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			functions[key] = function;
		}

		public void AddVariable(string prefix, string name, IXsltContextVariable variable)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			variables[key] = variable;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
		{
			IXsltContextFunction function;
			var key = new XmlName(name, prefix ?? string.Empty);
			functions.TryGetValue(key, out function);
			return function;
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			IXsltContextVariable variable;
			var key = new XmlName(name, prefix ?? string.Empty);
			variables.TryGetValue(key, out variable);
			return variable;
		}
#endif
	}
}
