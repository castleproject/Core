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
		private readonly XmlKnownTypeSet knownTypes;
#if !SL3
		private readonly ICompiledPath path;
#endif

		public XmlMetadata(DictionaryAdapterMeta meta)
		{
			type       = meta.Type;
			knownTypes = new XmlKnownTypeSet(type);

			var xmlRoot     = null as XmlRootAttribute;
			var xmlType     = null as XmlTypeAttribute;
			var xmlDefaults = null as XmlDefaultsAttribute;
			var xmlInclude  = null as XmlIncludeAttribute;
#if !SL3
			var xPath       = null as XPathAttribute;
#endif

			foreach (var behavior in meta.Behaviors)
			{
				if      (TryCast(behavior, ref xmlDefaults)) { }
				else if (TryCast(behavior, ref xmlRoot    )) { }
				else if (TryCast(behavior, ref xmlType    )) { }
				else if (TryCast(behavior, ref xmlInclude )) { knownTypes.Add(xmlInclude); }
#if !SL3
				else if (TryCast(behavior, ref xPath      )) { }
#endif
			}

			if (xmlDefaults != null)
			{
				qualified  = xmlDefaults.Qualified;
				isNullable = xmlDefaults.IsNullable;
			}

#if !SL3
			if (xPath != null)
			{
				path = xPath.Path;
			}
#endif

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
		Type IXmlKnownTypeMap.BaseType     { get { return type; } }

		public bool? Qualified             { get { return qualified; } }
		public bool? IsNullable            { get { return isNullable; } }
		public string RootLocalName        { get { return rootLocalName; } }
		public string RootNamespaceUri	   { get { return rootNamespaceUri; } }
		public string ChildNamespaceUri	   { get { return childNamespaceUri; } }
		public IXmlKnownTypeMap KnownTypes { get { return knownTypes; } }
#if !SL3
		public ICompiledPath Path          { get { return path; } }
#endif

		string IXmlKnownType.LocalName     { get { return rootLocalName; } }
		string IXmlKnownType.NamespaceUri  { get { return rootNamespaceUri; } }
		string IXmlKnownType.XsiType       { get { return null; } }
		Type   IXmlKnownType.ClrType       { get { return type; } }

		public bool MoveToBase(ref IXmlNode node, bool create)
		{
			if ( node.IsElement) return true;
			if (!node.IsRoot)    return false;

			var cursor = SelectBase(node);
			if (!Materialize(cursor, create))
				return false;

			node = cursor.Save();
			return true;
		}

		public IXmlCursor SelectBase(IXmlNode node)
		{
#if !SL3
			//if (path != null)
			//    return node.Select(path, CursorFlags.Elements | CursorFlags.Mutable);
#endif
			return node.SelectChildren(this, CursorFlags.Elements | CursorFlags.Mutable);
		}

		private bool Materialize(IXmlCursor cursor, bool create)
		{
			if (cursor.MoveNext())
				return true;
			if (!create)
				return false;

			cursor.Create(type);
			return true;
		}

		public bool TryRecognizeType(IXmlNode node, out Type type)
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
