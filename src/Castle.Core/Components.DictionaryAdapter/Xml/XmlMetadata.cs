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

	public class XmlMetadata : XmlType, IXmlTypeMap, IXmlType
	{
		private readonly Type type;
		private readonly bool? qualified;
		private readonly bool? isNullable;
		private readonly string rootLocalName;
		private readonly string rootNamespaceUri;
		private readonly string childNamespaceUri;
		private readonly XmlKnownTypeSet knownTypes;
		private readonly XmlContext context;
#if !SL3
		private readonly ICompiledPath path;
#endif
		
		public XmlMetadata(DictionaryAdapterMeta meta)
		{
			type       = meta.Type;
			knownTypes = new XmlKnownTypeSet(type);
			context    = new XmlContext();

			var xmlRoot      = null as XmlRootAttribute;
			var xmlType      = null as XmlTypeAttribute;
			var xmlDefaults  = null as XmlDefaultsAttribute;
			var xmlNamespace = null as XmlNamespaceAttribute;
			var xmlInclude   = null as XmlIncludeAttribute;
#if !SL3
			var xPath        = null as XPathAttribute;
#endif

			foreach (var behavior in meta.Behaviors)
			{
				if      (TryCast(behavior, ref xmlDefaults )) { }
				else if (TryCast(behavior, ref xmlRoot     )) { }
				else if (TryCast(behavior, ref xmlType     )) { }
				else if (TryCast(behavior, ref xmlNamespace)) { context.AddNamespace(xmlNamespace); }
				else if (TryCast(behavior, ref xmlInclude  )) { AddXmlInclude(xmlInclude); }
#if !SL3
				else if (TryCast(behavior, ref xPath       )) { }
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

		public override Type   ClrType      { get { return type; } }
		public override string LocalName    { get { return rootLocalName; } }
		public override string NamespaceUri	{ get { return rootNamespaceUri; } }
		public override string XsiType      { get { return null; } }

		public bool? Qualified              { get { return qualified; } }
		public bool? IsNullable             { get { return isNullable; } }
		public string ChildNamespaceUri	    { get { return childNamespaceUri; } }
		public IXmlTypeMap KnownTypes       { get { return knownTypes; } }
		public XmlContext Context           { get { return context; } }
#if !SL3
		public ICompiledPath Path           { get { return path; } }
#endif

		public IXmlCursor SelectBase(IXmlNode node)
		{
#if !SL3
			//if (path != null)
			//    return node.Select(path, CursorFlags.Elements | CursorFlags.Mutable);
#endif
			return node.SelectChildren(this, CursorFlags.Elements | CursorFlags.Mutable);
		}

		private void AddXmlInclude(XmlIncludeAttribute xmlInclude)
		{
			var clrType = xmlInclude.Type;
			var xmlType = new XmlIncludedType(XmlExtensions.GetLocalName(clrType), clrType);
			knownTypes.Add(xmlType);
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
