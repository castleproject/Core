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
	using System.Xml.XPath;
	using System.Xml.Xsl;

	internal class XPathContext : XsltContext
	{
		private readonly XsltContext context;

		public XPathContext(XsltContext xpathContext)
		{
			this.context = xpathContext;
		}

		public override string DefaultNamespace
		{
			// Must be empty, or XPath evaluation will break
			get { return string.Empty; }
		}

		public override string LookupNamespace(string prefix)
		{
			// Must return empty uri for empty prefix, or XPath evaluation will break
			return string.IsNullOrEmpty(prefix)
				? string.Empty
				: context.LookupNamespace(prefix);
		}

		public override bool Whitespace
		{
			get { return context.Whitespace; }
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return context.PreserveWhitespace(node);
		}

		public override int CompareDocument(string baseUri, string nextbaseUri)
		{
			return context.CompareDocument(baseUri, nextbaseUri);
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			return context.ResolveFunction(prefix, name, argTypes);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return context.ResolveVariable(prefix, name);
		}
	}
}
#endif
