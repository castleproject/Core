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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XmlMetadata : IXmlKnownTypeMap, IXmlKnownType
	{
		private readonly Type type;
		private readonly bool? qualified;
		private readonly bool? isNullable;
		private readonly string rootLocalName;
		private readonly string rootNamespaceUri;
		private readonly string childNamespaceUri;
		private readonly ICompiledPath path;
		private readonly XmlKnownTypeSet knownTypes;

		public XmlMetadata(DictionaryAdapterMeta meta)
		{
			type       = meta.Type;
			knownTypes = new XmlKnownTypeSet();

			var xmlDefaults = null as XmlDefaultsAttribute;
			var xmlRoot     = null as XmlRootAttribute;
			var xmlType     = null as XmlTypeAttribute;
			var xPath       = null as XPathAttribute;
			var xmlInclude  = null as XmlIncludeAttribute;

			foreach (var behavior in meta.Behaviors)
			{
				if      (TryCast(behavior, ref xmlDefaults)) { }
				else if (TryCast(behavior, ref xmlRoot    )) { }
				else if (TryCast(behavior, ref xmlType    )) { }
				else if (TryCast(behavior, ref xmlInclude )) { knownTypes.Add(xmlInclude); }
				else if (TryCast(behavior, ref xPath      )) { }
			}

			if (xmlDefaults != null)
			{
				qualified  = xmlDefaults.Qualified;
				isNullable = xmlDefaults.IsNullable;
			}

			if (xPath != null)
			{
				path = xPath.Path;
			}

			rootLocalName = XmlConvert.EncodeLocalName
			(
				(xmlRoot == null ? null : xmlRoot.ElementName) ??
				(xmlType == null ? null : xmlType.TypeName   ) ??
				type.GetLocalName()
			);

			rootNamespaceUri =
			(
				(xmlRoot == null ? null : xmlRoot.Namespace) ??
				null
			);

			childNamespaceUri =
			(
				(xmlType == null ? null : xmlType.Namespace) ??
				(xmlRoot == null ? null : xmlRoot.Namespace) ??
				null
			);
		}

		public Type ClrType                { get { return type; } }
		public bool? Qualified             { get { return qualified; } }
		public bool? IsNullable            { get { return isNullable; } }
		public string RootLocalName        { get { return rootLocalName; } }
		public string RootNamespaceUri	   { get { return rootNamespaceUri; } }
		public string ChildNamespaceUri	   { get { return childNamespaceUri; } }
		public ICompiledPath Path          { get { return path; } }
		public IXmlKnownTypeMap KnownTypes { get { return knownTypes; } }

		string IXmlKnownType.LocalName     { get { return rootLocalName; } }
		string IXmlKnownType.NamespaceUri  { get { return rootNamespaceUri; } }
		string IXmlKnownType.XsiType       { get { return null; } }
		Type   IXmlKnownType.ClrType       { get { return type; } }

		public bool MoveToBase(XPathNavigator node, bool create)
		{
			if (node.NodeType == XPathNodeType.Element)
				return true;
			if (node.NodeType != XPathNodeType.Root)
				return false;

			var iterator = SelectBase(node);
			if (!Materialize(iterator, create))
				return false;

			node.MoveTo(iterator.Current.Node);
			return true;
		}

		private XmlIterator SelectBase(XPathNavigator node)
		{
			return path == null
				? (XmlIterator) new XmlElementIterator  (node, this, false)
				: (XmlIterator) new XPathMutableIterator(node, path, false);
		}

		private bool Materialize(XmlIterator iterator, bool create)
		{
			if (iterator.MoveNext())
				return true;
			if (!create)
				return false;

			iterator.Create(type);
			return true;
		}

		public bool TryRecognizeType(XPathNavigator node, out Type type)
		{
			return node.HasNameLike(rootLocalName, rootNamespaceUri)
				? Try.Success(out type, ClrType)
				: Try.Failure(out type);
		}

		public IXmlKnownType GetXmlKnownType(Type type)
		{
			return this;
		}

		private static bool TryCast<T>(object obj, ref T result)
			where T : class
		{
			var value = obj as T;
			if (null == value) return false;

			result = value;
			return true;
		}
	}
}
#endif
