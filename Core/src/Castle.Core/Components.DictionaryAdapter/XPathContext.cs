// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using System.Xml.Xsl;

	public class XPathContext : XsltContext
	{
		private readonly XPathContext parent;
		private XmlMetadata xmlMeta;
		private string qualifiedNamespace;
		private Dictionary<string, Func<IXsltContextFunction>> functions;
		private Dictionary<string, string> rootNamespaces;
		private List<IXPathSerializer> serializers;
		private int prefixCount;

		public const string Prefix = "castle-da";
		public const string NamespaceUri = "urn:castleproject.org:da";
		public const string IgnoreNamespace = "_";
		private const string Xsd = "http://www.w3.org/2001/XMLSchema";
		private const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";

		public XPathContext() 
			: this(new NameTable())
		{
		}

		public XPathContext(NameTable nameTable) 
			: base(nameTable)
		{
			Arguments = new XsltArgumentList();
			AddNamespace("xsi", Xsi);
			AddNamespace("xsd", Xsd);
			AddNamespace(Prefix, NamespaceUri);
			AddFunction(Prefix, "match", MatchFunction.Instance);
		}

		public XPathContext(XPathContext parent) 
			: base((NameTable)parent.NameTable)
		{
			this.parent = parent;
			Arguments = new XsltArgumentList();
		}

		public override string DefaultNamespace
		{
			get
			{
				var defaultNamespace = base.DefaultNamespace;
				if (string.IsNullOrEmpty(defaultNamespace) && parent != null)
					defaultNamespace = parent.DefaultNamespace;
				return defaultNamespace;
			}
		}

		public bool IsNullable
		{
			get
			{
				if (xmlMeta != null && xmlMeta.IsNullable.HasValue)
					return xmlMeta.IsNullable.Value;
				return (parent != null) ? parent.IsNullable : false;
			}
		}

		public XsltArgumentList Arguments { get; private set; }

		public IEnumerable<XmlArrayItemAttribute> ListItemMeta { get; private set; }

		public IEnumerable<IXPathSerializer> Serializers
		{
			get
			{
				var mine = serializers ?? Enumerable.Empty<IXPathSerializer>();
				var parents = (parent != null) ? parent.Serializers : Enumerable.Empty<IXPathSerializer>();
				return mine.Union(parents);
			}
		}

		public XPathContext ApplyBehaviors(XmlMetadata xmlMeta, IEnumerable behaviors)
		{
			if (this.xmlMeta == null && xmlMeta != null)
			{
				this.xmlMeta = xmlMeta;
				if (string.IsNullOrEmpty(xmlMeta.XmlType.Namespace) == false)
				{
					if (xmlMeta.Qualified.GetValueOrDefault(false))
					{
						AddNamespace(xmlMeta.XmlType.Namespace);
						qualifiedNamespace = xmlMeta.XmlType.Namespace;
					}
					else
					{
						AddNamespace(string.Empty, xmlMeta.XmlType.Namespace);
					}
				}
			}

			new BehaviorVisitor()
				.OfType<XmlNamespaceAttribute>(attrib =>
				{
					AddNamespace(attrib.Prefix, attrib.NamespaceUri);
					if (attrib.Default)
						AddNamespace(string.Empty, attrib.NamespaceUri);
					if (attrib.Root)
						AddRootNamespace(attrib.NamespaceUri, attrib.Prefix);
				})
				.OfType<XmlArrayItemAttribute>(attrib =>
				{
					ListItemMeta = ListItemMeta ?? new List<XmlArrayItemAttribute>();
					((List<XmlArrayItemAttribute>)ListItemMeta).Add(attrib);
				})
				.OfType<XPathFunctionAttribute>(attrib => AddFunction(attrib.Prefix, attrib.Name, attrib.Function))
				.OfType<IXPathSerializer>(attrib => AddSerializer(attrib))
				.Apply(behaviors);
			return this;
		}

		public XPathContext CreateChild(XmlMetadata xmlMeta, IEnumerable behaviors)
		{
			return new XPathContext(this).ApplyBehaviors(xmlMeta, behaviors);
		}

		public XPathContext CreateChild(XmlMetadata xmlMeta, params object[] behaviors)
		{
			return CreateChild(xmlMeta, (IEnumerable)behaviors);
		}

		public override bool HasNamespace(string prefix)
		{
			return base.HasNamespace(prefix) || (parent != null && parent.HasNamespace(prefix));
		}

		public override string LookupNamespace(string prefix)
		{
			var uri = base.LookupNamespace(prefix);
			if (uri == null && parent != null)
				uri = parent.LookupNamespace(prefix);
			return uri;
		}

		public override string LookupPrefix(string uri)
		{
			var prefix = base.LookupPrefix(uri);
			if (string.IsNullOrEmpty(prefix) && parent != null)
				prefix = parent.LookupPrefix(uri);
			return prefix;
		}

		public string AddNamespace(string namespaceUri)
		{
			var prefix = LookupPrefix(namespaceUri);
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = GetUniquePrefix();
				AddNamespace(prefix, namespaceUri);
			}
			return prefix;
		}

		public XPathContext AddSerializer(IXPathSerializer serializer)
		{
			serializers = serializers ?? new List<IXPathSerializer>();
			serializers.Insert(0, serializer);
			return this;
		}

		public XPathContext AddFunction(string prefix, string name, IXsltContextFunction function)
		{
			if (functions == null)
				functions = new Dictionary<string, Func<IXsltContextFunction>>();
			functions[GetQualifiedName(prefix, name)] = () => function;
			return this;
		}

		public XPathContext AddFunction(string prefix, string name, Func<IXsltContextFunction> function)
		{
			if (functions == null)
				functions = new Dictionary<string, Func<IXsltContextFunction>>();
			functions[GetQualifiedName(prefix, name)] = function;
			return this;
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
		{
			if (functions != null)
			{
				Func<IXsltContextFunction> function;
				if (functions.TryGetValue(GetQualifiedName(prefix, name), out function))
					return function();
			}
			return (parent != null) ? parent.ResolveFunction(prefix, name, argTypes) : null;
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return new XPathVariable(name);
		}

		public bool Evaluate(XPathExpression xpath, XPathNavigator source, out object result)
		{
			xpath = (XPathExpression)xpath.Clone();
			xpath.SetContext(new XPathSemantics(this));
			result = source.Evaluate(xpath);
			if (xpath.ReturnType == XPathResultType.NodeSet)	
			{
				if (((XPathNodeIterator)result).Count == 0)
					result = null;
			}
			return result != null;
		}

		public XPathNavigator SelectSingleNode(XPathExpression xpath, XPathNavigator source)
		{
			xpath = (XPathExpression)xpath.Clone();
			xpath.SetContext(new XPathSemantics(this));
			return source.SelectSingleNode(xpath);
		}

		public bool Matches(XPathExpression xpath, XPathNavigator source)
		{
			xpath = (XPathExpression)xpath.Clone();
			xpath.SetContext(new XPathSemantics(this));
			return source.Matches(xpath);
		}

		public void AddStandardNamespaces(XPathNavigator source)
		{
			CreateNamespace("xsd", Xsd, source);
		}

		public string CreateNamespace(string prefix, string namespaceUri, XPathNavigator source)
		{
			if (string.IsNullOrEmpty(namespaceUri) == false)
			{
				source = source.Clone();
				source.MoveToRoot();
				source.MoveToChild(XPathNodeType.Element);

				if (string.IsNullOrEmpty(prefix))
					prefix = AddNamespace(namespaceUri);

				var existing = source.GetNamespace(prefix);
				if (existing == namespaceUri) return prefix;
				if (string.IsNullOrEmpty(existing) == false)
					return null;

				source.CreateAttribute("xmlns", prefix, "", namespaceUri);
			}
			return prefix;
		}

		private bool IsRootNamespace(string namespaceUri, out string prefix)
		{
			prefix = null;
			if (string.IsNullOrEmpty(namespaceUri))
				return false;
			if (parent != null)
				return parent.IsRootNamespace(namespaceUri, out prefix);
			if (rootNamespaces == null)
				return false;
			return rootNamespaces.TryGetValue(namespaceUri, out prefix);
		}

		private void AddRootNamespace(string namespaceUri, string prefix)
		{
			if (parent != null)
				parent.AddRootNamespace(namespaceUri, prefix);
			else
			{
				if (rootNamespaces == null)
					rootNamespaces = new Dictionary<string, string>();
				rootNamespaces.Add(namespaceUri, prefix);
			}
		}

		public XPathNavigator CreateAttribute(string name, string namespaceUri, XPathNavigator source)
		{
			string prefix;
			name = XmlConvert.EncodeLocalName(name);
			if (IsRootNamespace(namespaceUri, out prefix))
				prefix = CreateNamespace(prefix, namespaceUri, source);
			source.CreateAttribute(prefix, name, namespaceUri, "");
			source.MoveToAttribute(name, namespaceUri ?? "");
			return source;
		}

		public XPathNavigator AppendElement(string name, string namespaceUri, XPathNavigator source)
		{
			string prefix;
			name = XmlConvert.EncodeLocalName(name);
			namespaceUri = GetEffectiveNamespace(namespaceUri);
			if (IsRootNamespace(namespaceUri, out prefix))
				prefix = CreateNamespace(prefix, namespaceUri, source);
			source.AppendChildElement(prefix ?? LookupPrefix(namespaceUri), name, namespaceUri, null);
			return source.SelectSingleNode("*[position()=last()]");
		}

		public void SetXmlType(string name, string namespaceUri, XPathNavigator source)
		{
			namespaceUri = GetEffectiveNamespace(namespaceUri);
			var prefix = CreateNamespace(null, namespaceUri, source);
			source.CreateAttribute("xsi", "type", Xsi, GetQualifiedName(prefix, name));
		}

		public XmlQualifiedName GetXmlType(XPathNavigator source)
		{
			var qualifiedType = source.GetAttribute("type", Xsi);
			if (string.IsNullOrEmpty(qualifiedType) == false)
			{
				string name, namespaceUri = null;
				var prefix = SplitQualifiedName(qualifiedType, out name);
				if (prefix != null)
					namespaceUri = source.GetNamespace(prefix);
				return new XmlQualifiedName(name, namespaceUri);
			}
			return null;
		}

		public bool IsNil(XPathNavigator source)
		{
			return source.NodeType == XPathNodeType.Element && 
				source.GetAttribute("nil", Xsi).Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		public bool MakeNil(XPathNavigator source)
		{
			if (source.NodeType == XPathNodeType.Element && IsNil(source) == false)
			{
				if (source.LookupPrefix(Xsi) != "xsi")
					CreateNamespace("xsi", Xsi, source);
				source.CreateAttribute("xsi", "nil", Xsi, "true");
				return true;
			}
			return false;
		}

		public string GetEffectiveNamespace(string namespaceUri)
		{
			return namespaceUri ?? qualifiedNamespace ?? DefaultNamespace;
		}

		public override int CompareDocument(string baseUri, string nextbaseUri)
		{
			return 0;
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}

		public override bool Whitespace
		{
			get { return true; }
		}

		private string GetUniquePrefix()
		{
			if (parent != null)
				return parent.GetUniquePrefix();
			return "da" + ++prefixCount;
		}

		private static string GetQualifiedName(string prefix, string name)
		{
			name = XmlConvert.EncodeLocalName(name);
			if (string.IsNullOrEmpty(prefix))
				return name;
			return String.Format("{0}:{1}", prefix, name);
		}

		private static string SplitQualifiedName(string qualifiedName, out string name)
		{
			var parts = qualifiedName.Split(':');
			if (parts.Length == 1)
			{
				name = parts[0];
				return null;
			}
			else if (parts.Length == 2)
			{
				name = parts[1];
				return parts[0];
			}
			throw new ArgumentException(string.Format(
				"Invalid qualified name {0}.  Expected [prefix:]name format", qualifiedName));
		}

		#region Nested Type: XPathSemantics

		class XPathSemantics : XsltContext
		{
			private readonly XPathContext xpathContext;

			public XPathSemantics(XPathContext xpathContext)
			{
				this.xpathContext = xpathContext;
			}

			public override string DefaultNamespace
			{
				get { return string.Empty; }
			}

			public override string LookupNamespace(string prefix)
			{
				return (prefix.Length > 0) ? xpathContext.LookupNamespace(prefix) : string.Empty;
			}

			public XsltArgumentList Arguments
			{
				get { return xpathContext.Arguments; }
			}

			public override int CompareDocument(string baseUri, string nextbaseUri)
			{
				return xpathContext.CompareDocument(baseUri, nextbaseUri);
			}

			public override bool PreserveWhitespace(XPathNavigator node)
			{
				return xpathContext.PreserveWhitespace(node);
			}

			public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
			{
				return xpathContext.ResolveFunction(prefix, name, argTypes);
			}

			public override IXsltContextVariable ResolveVariable(string prefix, string name)
			{
				return xpathContext.ResolveVariable(prefix, name);
			}

			public override bool Whitespace
			{
				get { return xpathContext.Whitespace; }
			}
		}

		#endregion

		#region Nested Type: XPathVariable

		public class XPathVariable : IXsltContextVariable
		{
			private readonly string name;

			public XPathVariable(string name)
			{
				this.name = name;
			}

			public bool IsLocal
			{
				get { return false; }
			}

			public bool IsParam
			{
				get { return false; }
			}

			public XPathResultType VariableType
			{
				get { return XPathResultType.Any; }
			}

			public object Evaluate(XsltContext xsltContext)
			{
				var args = ((XPathSemantics)xsltContext).Arguments;
				return args.GetParam(name, null);
			}
		}

		#endregion

		#region Nested Type: MatchFunction

		public class MatchFunction : IXsltContextFunction
		{
			public static readonly MatchFunction Instance = new MatchFunction();
			
			protected MatchFunction()
			{
			}

			public int Minargs
			{
				get { return 1; }
			}

			public int Maxargs
			{
				get { return 2; }
			}

			public XPathResultType[] ArgTypes
			{
				get { return new[] { XPathResultType.String, XPathResultType.String }; }
			}

			public XPathResultType ReturnType
			{
				get { return XPathResultType.Boolean; }
			}

			public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
			{
				var key = XmlConvert.EncodeLocalName((string)args[0]);
				if (key.Equals(docContext.LocalName, StringComparison.OrdinalIgnoreCase) == false)
				{
					return false;
				}

				if (args.Length > 1 && args[1].Equals(IgnoreNamespace) == false)
				{
					var ns = (string)args[1];
					if (ns.Equals(docContext.NamespaceURI, StringComparison.OrdinalIgnoreCase) == false)
					{
						return false;
					}
				}

				return true;
			}
		}

		#endregion
	}
#endif
}