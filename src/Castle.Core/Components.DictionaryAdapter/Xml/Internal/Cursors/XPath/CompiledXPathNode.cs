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

#if FEATURE_DICTIONARYADAPTER_XML
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
		private CompiledXPathNode previous;
		private IList<CompiledXPathNode> dependencies;

		internal CompiledXPathNode() { }

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

		public bool IsSelfReference
		{
			get { return localName == null; }
		}

		public bool IsSimple
		{
			get { return next == null && HasNoRealDependencies(); }
		}

		public XPathExpression Value
		{
			get { return value ?? GetSelfReferenceValue(); }
			internal set { this.value = value; }
		}

		public CompiledXPathNode NextNode
		{
			get { return next; }
			internal set { next = value; }
		}

		public CompiledXPathNode PreviousNode
		{
			get { return previous; }
			internal set { previous = value; }
		}

		public IList<CompiledXPathNode> Dependencies
		{
			get { return dependencies ?? (dependencies = new List<CompiledXPathNode>()); }
		}

		private static readonly IList<CompiledXPathNode>
			NoDependencies = Array.AsReadOnly(new CompiledXPathNode[0]);

		private bool HasNoRealDependencies()
		{
			return
			(
				dependencies == null ||
				dependencies.Count == 0 ||
				(
					dependencies.Count == 1 &&
					dependencies[0].IsSelfReference
				)
			);
		}

		private XPathExpression GetSelfReferenceValue()
		{
			return dependencies != null
				&& dependencies.Count == 1
				&& dependencies[0].IsSelfReference
				 ? dependencies[0].value
				 : null;
		}

		internal virtual void Prepare()
		{
			dependencies = (dependencies != null)
				? Array.AsReadOnly(dependencies.ToArray())
				: NoDependencies;

			foreach (var child in dependencies)
				child.Prepare();

			if (next != null)
				next.Prepare();
		}

		internal virtual void SetContext(XsltContext context)
		{
			if (value != null)
				value.SetContext(context);

			foreach (var child in dependencies)
				child.SetContext(context);

			if (next != null)
				next.SetContext(context);
		}
	}
}
#endif
