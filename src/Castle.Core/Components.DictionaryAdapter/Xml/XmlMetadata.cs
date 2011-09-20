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
	using System.Xml.Serialization;

	public class XmlMetadata : IXmlKnownType, IXmlKnownTypeMap, IXmlIncludedType, IXmlIncludedTypeMap, IXmlAccessorContext
	{
		private readonly Type clrType;
		private readonly bool? qualified;
		private readonly bool? isNullable;
		private readonly string rootLocalName;
		private readonly string rootNamespaceUri;
		private readonly string childNamespaceUri;
		private readonly XmlIncludedTypeSet includedTypes;
		private readonly XmlContext context;
#if !SL3
		private readonly CompiledXPath path;
#endif
		
		public XmlMetadata(DictionaryAdapterMeta meta)
		{
			clrType       = meta.Type;
			context       = new XmlContext();
			includedTypes = new XmlIncludedTypeSet();

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
				path.SetContext(context);
			}
#endif
			rootLocalName = XmlConvert.EncodeLocalName
			(
				(xmlRoot == null ? null : xmlRoot.ElementName) ??
				(xmlType == null ? null : xmlType.TypeName   ) ??
				clrType.GetLocalName()
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

		public Type ClrType
		{
			get { return clrType; }
		}

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get { return this; }
		}

		public string LocalName
		{
			get { return rootLocalName; }
		}

		public string NamespaceUri
		{
			get { return rootNamespaceUri; }
		}

		public string XsiType
		{
			get { return null; }
		}

		public bool? Qualified
		{
			get { return qualified; }
		}

		public bool? IsNullable
		{
			get { return isNullable; }
		}

		public string ChildNamespaceUri
		{
			get { return childNamespaceUri; }
		}

		public XmlIncludedTypeSet IncludedTypes
		{
			get { return includedTypes; }
		}

		public XmlContext XmlContext
		{
			get { return context; }
		}
#if !SL3
		public CompiledXPath Path
		{
			get { return path; }
		}
#endif
		public IXmlCursor SelectBase(IXmlNode node) // node is root
		{
#if !SL3
			if (path != null)
				return node.Select(path, this, RootFlags);
#endif
			return node.SelectChildren(this, RootFlags);
		}

		private bool IsMatch(IXmlName xmlName)
		{
			return NameComparer.Equals(rootLocalName, xmlName.LocalName)
				&& (rootNamespaceUri == null || NameComparer.Equals(rootNamespaceUri, xmlName.NamespaceUri));
		}

		private bool IsMatch(Type clrType)
		{
			return clrType == this.clrType;
		}

		public bool TryGet(IXmlName xmlName, out IXmlKnownType knownType)
		{
			return IsMatch(xmlName)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return IsMatch(clrType)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(string xsiType, out IXmlIncludedType includedType)
		{
			return xsiType == null
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return clrType == this.clrType
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		private void AddXmlInclude(XmlIncludeAttribute attribute)
		{
			var clrType      = attribute.Type;
			var includedType = new XmlIncludedType(clrType.GetLocalName(), clrType);
			includedTypes.Add(includedType);
		}

		private static bool TryCast<T>(object obj, ref T result)
			where T : class
		{
			var value = obj as T;
			if (null == value) return false;

			result = value;
			return true;
		}

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;

		private const CursorFlags RootFlags
			= CursorFlags.Elements
			| CursorFlags.Mutable;

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get { throw new NotImplementedException(); }
		}
	}
}
