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
	public class XmlContextBase : XsltContext
#else
	public class XmlContextBase : XmlNamespaceManager
#endif
	{
		private readonly XmlContextBase parent;
		private Dictionary<string, string> rootNamespaces;
		private bool hasNamespaces;
#if !SL3
		private XPathContext xPathContext;
		private Dictionary<XmlName, IXsltContextFunction> functions;
		private Dictionary<XmlName, IXsltContextVariable> variables;
#endif

		public XmlContextBase()
			: base(new NameTable())
		{
			AddNamespace(Xsd .Attribute);
			AddNamespace(Xsi .Attribute);
			AddNamespace(Wsdl.Attribute);
		}

		protected XmlContextBase(XmlContextBase parent)
		    : base(GetNameTable(parent))
		{
			this.parent = parent;
		}

#if !SL3
		private static NameTable GetNameTable(XmlContextBase parent)
		{
			return parent.NameTable as NameTable ?? new NameTable();
		}
#else
		private static XmlNameTable GetNameTable(XmlContext parent)
		{
			return parent.NameTable;
		}
#endif

		public void AddNamespace(XmlNamespaceAttribute attribute)
		{
			var prefix = attribute.Prefix;
			var uri    = attribute.NamespaceUri;

			if (string.IsNullOrEmpty(uri))
				throw Error.InvalidNamespaceUri();

			if (attribute.Default)
				AddNamespace(string.Empty, uri);

			if (string.IsNullOrEmpty(prefix))
				return;

			AddNamespace(prefix, uri);

			if (attribute.Root)
				EnsureRootNamespaces().Add(prefix, uri);
		}

		public override void AddNamespace(string prefix, string uri)
		{
			base.AddNamespace(prefix, uri);
			hasNamespaces = true;
		}

		private Dictionary<string, string> EnsureRootNamespaces()
		{
			return rootNamespaces ??
			(
				rootNamespaces = parent != null
					? new Dictionary<string, string>(parent.EnsureRootNamespaces())
					: new Dictionary<string, string>()
			);
		}

		public override string LookupNamespace(string prefix)
		{
			return hasNamespaces
				? base  .LookupNamespace(prefix)
				: parent.LookupNamespace(prefix);
		}

		public override string LookupPrefix(string uri)
		{
			return hasNamespaces
				? base  .LookupPrefix(uri)
				: parent.LookupPrefix(uri);
		}

		public string GetElementPrefix(IXmlNode node, string namespaceUri)
		{
			string prefix;
			if (namespaceUri == node.LookupNamespaceUri(string.Empty))
				return string.Empty;
			if (TryGetDefinedPrefix(node, namespaceUri, out prefix))
				return prefix;
			if (!TryGetPreferredPrefix(node, namespaceUri, out prefix))
				return string.Empty;
			if (!ShouldDefineOnRoot(prefix, namespaceUri))
				return string.Empty;

			node.DefineNamespace(prefix, namespaceUri, true);
			return prefix;
		}

		public string GetAttributePrefix(IXmlNode node, string namespaceUri)
		{
			string prefix;
			if (namespaceUri == node.Name.NamespaceUri)
			    return string.Empty;
			if (TryGetDefinedPrefix(node, namespaceUri, out prefix))
				return prefix;
			if (!TryGetPreferredPrefix(node, namespaceUri, out prefix))
				prefix = GeneratePrefix(node);

			var root = ShouldDefineOnRoot(prefix, namespaceUri);
			node.DefineNamespace(prefix, namespaceUri, root);
			return prefix;
		}

		private static bool TryGetDefinedPrefix(IXmlNode node, string namespaceUri, out string prefix)
		{
			var definedPrefix = node.LookupPrefix(namespaceUri);
			return string.IsNullOrEmpty(definedPrefix)
				? Try.Failure(out prefix)
				: Try.Success(out prefix, definedPrefix);
		}

		private bool TryGetPreferredPrefix(IXmlNode node, string namespaceUri, out string prefix)
		{
			prefix = this.LookupPrefix(namespaceUri);
			if (string.IsNullOrEmpty(prefix))
				return Try.Failure(out prefix); // No preferred prefix

			namespaceUri = node.LookupPrefix(prefix);
			return string.IsNullOrEmpty(namespaceUri)
				? true                     // Can use preferred prefix
				: Try.Failure(out prefix); // Preferred prefix already in use
		}

		private static string GeneratePrefix(IXmlNode node)
		{
			for (var i = 0; ; i++)
			{
				var prefix = "p" + i;
				var namespaceUri = node.LookupNamespaceUri(prefix);
				if (string.IsNullOrEmpty(namespaceUri))
					return prefix;
			}
		}

		private bool ShouldDefineOnRoot(string prefix, string uri)
		{
			return rootNamespaces != null
				? ShouldDefineOnRootCore   (prefix, uri)
				: parent.ShouldDefineOnRoot(prefix, uri);
		}

		private bool ShouldDefineOnRootCore(string prefix, string uri)
		{
			string candidate;
			return rootNamespaces.TryGetValue(prefix, out candidate)
				&& candidate == uri;
		}

#if !SL3
		private XPathContext XPathContext
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
			EnsureFunctions()[key] = function;
		}

		public void AddVariable(string prefix, string name, IXsltContextVariable variable)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			EnsureVariables()[key] = variable;
		}

		private Dictionary<XmlName, IXsltContextFunction> EnsureFunctions()
		{
			return functions ??
			(
				functions = (parent != null)
					? new Dictionary<XmlName, IXsltContextFunction>(parent.EnsureFunctions())
					: new Dictionary<XmlName, IXsltContextFunction>()
			);
		}

		private Dictionary<XmlName, IXsltContextVariable> EnsureVariables()
		{
			return variables ??
			(
				variables = (parent != null)
					? new Dictionary<XmlName, IXsltContextVariable>(parent.EnsureVariables())
					: new Dictionary<XmlName, IXsltContextVariable>()
			);
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			return
				functions != null ? ResolveFunctionCore   (prefix, name, argTypes) :
				parent    != null ? parent.ResolveFunction(prefix, name, argTypes) :
				null;
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return
				variables != null ? ResolveVariableCore   (prefix, name) :
				parent    != null ? parent.ResolveVariable(prefix, name) :
				null;
		}

		private IXsltContextFunction ResolveFunctionCore(string prefix, string name, XPathResultType[] argTypes)
		{
			IXsltContextFunction function;
			var key = new XmlName(name, prefix ?? string.Empty);
			functions.TryGetValue(key, out function);
			return function;
		}

		private IXsltContextVariable ResolveVariableCore(string prefix, string name)
		{
			IXsltContextVariable variable;
			var key = new XmlName(name, prefix ?? string.Empty);
			variables.TryGetValue(key, out variable);
			return variable;
		}

		public void Enlist(CompiledXPath path)
		{
			path.SetContext(XPathContext);
		}
#endif
	}
}
