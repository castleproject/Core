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
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.XPath;
	using System.Xml.Xsl;

	public class XmlContextBase : XsltContext, IXmlNamespaceSource
	{
		private readonly XmlContextBase parent;
		private Dictionary<string, string> rootNamespaces;
		private bool hasNamespaces;
		private XPathContext xPathContext;
		private Dictionary<XmlName, IXsltContextVariable> variables;
		private Dictionary<XmlName, IXsltContextFunction> functions;

		public XmlContextBase()
			: base(new NameTable())
		{
			AddNamespace(Xsd .Namespace);
			AddNamespace(Xsi .Namespace);
			AddNamespace(Wsdl.Namespace);
			AddNamespace(XRef.Namespace);
		}

		protected XmlContextBase(XmlContextBase parent)
		    : base(GetNameTable(parent))
		{
			this.parent = parent;
		}

		private static NameTable GetNameTable(XmlContextBase parent)
		{
			return parent.NameTable as NameTable ?? new NameTable();
		}

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
			if (string.IsNullOrEmpty(namespaceUri)) // was: namespaceUri == node.Name.NamespaceUri
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

			namespaceUri = node.LookupNamespaceUri(prefix);
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

		public void AddVariable(string prefix, string name, IXsltContextVariable variable)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			AddVariable(key, variable);
		}

		public void AddFunction(string prefix, string name, IXsltContextFunction function)
		{
			var key = new XmlName(name, prefix ?? string.Empty);
			AddFunction(key, function);
		}

		public void AddVariable(XPathVariableAttribute attribute)
		{
			AddVariable(attribute.Name, attribute);
		}

		public void AddFunction(XPathFunctionAttribute attribute)
		{
			AddFunction(attribute.Name, attribute);
		}

		public void AddVariable(XmlName name, IXsltContextVariable variable)
		{
			EnsureVariables()[name] = variable;
		}

		public void AddFunction(XmlName name, IXsltContextFunction function)
		{
			EnsureFunctions()[name] = function;
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

		private Dictionary<XmlName, IXsltContextFunction> EnsureFunctions()
		{
			return functions ??
			(
				functions = (parent != null)
					? new Dictionary<XmlName, IXsltContextFunction>(parent.EnsureFunctions())
					: new Dictionary<XmlName, IXsltContextFunction>()
			);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return
				variables != null ? ResolveVariableCore   (prefix, name) :
				parent    != null ? parent.ResolveVariable(prefix, name) :
				null;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			return
				functions != null ? ResolveFunctionCore   (prefix, name, argTypes) :
				parent    != null ? parent.ResolveFunction(prefix, name, argTypes) :
				null;
		}

		private IXsltContextVariable ResolveVariableCore(string prefix, string name)
		{
			IXsltContextVariable variable;
			var key = new XmlName(name, prefix ?? string.Empty);
			variables.TryGetValue(key, out variable);
			return variable;
		}

		private IXsltContextFunction ResolveFunctionCore(string prefix, string name, XPathResultType[] argTypes)
		{
			IXsltContextFunction function;
			var key = new XmlName(name, prefix ?? string.Empty);
			functions.TryGetValue(key, out function);
			return function;
		}

		public void Enlist(CompiledXPath path)
		{
			path.SetContext(XPathContext);
		}
	}
}
#endif
