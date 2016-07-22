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
	using System.Linq;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Serialization;

	public class XmlMetadata : IXmlKnownType, IXmlKnownTypeMap, IXmlIncludedType, IXmlIncludedTypeMap
	{
		private readonly Type   clrType;
		private readonly bool?  qualified;
		private readonly bool?  isNullable;
		private readonly bool?  isReference;
		private readonly string rootLocalName;
		private readonly string rootNamespaceUri;
		private readonly string childNamespaceUri;
		private readonly string typeLocalName;
		private readonly string typeNamespaceUri;

		private readonly HashSet<string>       reservedNamespaceUris;
		private          List<Type>            pendingIncludes;
		private readonly XmlIncludedTypeSet    includedTypes;
		private readonly XmlContext            context;
		private readonly DictionaryAdapterMeta source;
		private readonly CompiledXPath path;
		
		public XmlMetadata(DictionaryAdapterMeta meta, IEnumerable<string> reservedNamespaceUris)
		{
			if (meta == null)
				throw Error.ArgumentNull("meta");
			if (reservedNamespaceUris == null)
				throw Error.ArgumentNull("reservedNamespaceUris");

			source        = meta;
			clrType       = meta.Type;
			context       = new XmlContext(this);
			includedTypes = new XmlIncludedTypeSet();

			this.reservedNamespaceUris
				=  reservedNamespaceUris as HashSet<string>
				?? new HashSet<string>(reservedNamespaceUris);

			var xmlRoot       = null as XmlRootAttribute;
			var xmlType       = null as XmlTypeAttribute;
			var xmlDefaults   = null as XmlDefaultsAttribute;
			var xmlInclude    = null as XmlIncludeAttribute;
			var xmlNamespace  = null as XmlNamespaceAttribute;
			var reference     = null as ReferenceAttribute;
			var xPath         = null as XPathAttribute;
			var xPathVariable = null as XPathVariableAttribute;
			var xPathFunction = null as XPathFunctionAttribute;

			foreach (var behavior in meta.Behaviors)
			{
				if      (TryCast(behavior, ref xmlRoot      )) { }
				else if (TryCast(behavior, ref xmlType      )) { }
				else if (TryCast(behavior, ref xmlDefaults  )) { }
				else if (TryCast(behavior, ref xmlInclude   )) { AddPendingInclude(xmlInclude); }
				else if (TryCast(behavior, ref xmlNamespace )) { context.AddNamespace(xmlNamespace ); }
				else if (TryCast(behavior, ref reference    )) { }
				else if (TryCast(behavior, ref xPath        )) { }
				else if (TryCast(behavior, ref xPathVariable)) { context.AddVariable (xPathVariable); }
				else if (TryCast(behavior, ref xPathFunction)) { context.AddFunction (xPathFunction); }
			}

			if (xmlDefaults != null)
			{
				qualified  = xmlDefaults.Qualified;
				isNullable = xmlDefaults.IsNullable;
			}

			if (reference != null)
			{
				isReference = true;
			}

			typeLocalName = XmlConvert.EncodeLocalName
			(
				(!meta.HasXmlType() ? null : meta.GetXmlType().NonEmpty()) ??
				(xmlType == null    ? null : xmlType.TypeName .NonEmpty()) ??
				GetDefaultTypeLocalName(clrType)
			);

			rootLocalName = XmlConvert.EncodeLocalName
			(
				(xmlRoot == null ? null : xmlRoot.ElementName.NonEmpty()) ??
				typeLocalName
			);

			typeNamespaceUri =
			(
				(xmlType == null ? null : xmlType.Namespace)
			);

			rootNamespaceUri =
			(
				(xmlRoot == null ? null : xmlRoot.Namespace)
			);

			childNamespaceUri =
			(
				typeNamespaceUri ??
				rootNamespaceUri
			);

			if (xPath != null)
			{
				path = xPath.GetPath;
				path.SetContext(context);
			}
		}

		public Type ClrType
		{
			get { return clrType; }
		}

		public bool? Qualified
		{
			get { return qualified; }
		}

		public bool? IsNullable
		{
			get { return isNullable; }
		}

		public bool? IsReference
		{
			get { return isReference; }
		} 

		public XmlName Name
		{
			get { return new XmlName(rootLocalName, rootNamespaceUri); }
		}

		public XmlName XsiType
		{
			get { return new XmlName(typeLocalName, typeNamespaceUri); }
		}

		XmlName IXmlIdentity.XsiType
		{
			get { return XmlName.Empty; }
		}

		public string ChildNamespaceUri
		{
			get { return childNamespaceUri; }
		}

		public IEnumerable<string> ReservedNamespaceUris
		{
			get { return reservedNamespaceUris.ToArray(); }
		}

		public XmlIncludedTypeSet IncludedTypes
		{
			get { ProcessPendingIncludes(); return includedTypes; }
		}

		public IXmlContext Context
		{
			get { return context; }
		}

		public CompiledXPath Path
		{
			get { return path; }
		}

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get { return this; }
		}

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get { return this; }
		}

		public bool IsReservedNamespaceUri(string namespaceUri)
		{
			return reservedNamespaceUris.Contains(namespaceUri);
		}

		public IXmlCursor SelectBase(IXmlNode node) // node is root
		{
			if (path != null)
				return node.Select(path, this, context, RootFlags);

			return node.SelectChildren(this, context, RootFlags);
		}

		private bool IsMatch(IXmlIdentity xmlIdentity)
		{
			var name = xmlIdentity.Name;

			return NameComparer.Equals(rootLocalName, name.LocalName)
				&& (rootNamespaceUri == null || NameComparer.Equals(rootNamespaceUri, name.NamespaceUri));
		}

		private bool IsMatch(Type clrType)
		{
			return clrType == this.clrType;
		}

		public bool TryGet(IXmlIdentity xmlIdentity, out IXmlKnownType knownType)
		{
			return IsMatch(xmlIdentity)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return IsMatch(clrType)
				? Try.Success(out knownType, this)
				: Try.Failure(out knownType);
		}

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			return xsiType == XmlName.Empty || xsiType == this.XsiType
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return clrType == this.clrType
				? Try.Success(out includedType, this)
				: Try.Failure(out includedType);
		}

		private void AddPendingInclude(XmlIncludeAttribute attribute)
		{
			if (pendingIncludes == null)
				pendingIncludes = new List<Type>();
			pendingIncludes.Add(attribute.Type);
		}

		private void ProcessPendingIncludes()
		{
			var clrTypes = pendingIncludes;
			pendingIncludes = null;
			if (clrTypes == null) return;

			foreach (var clrType in clrTypes)
			{
				var xsiType      = GetDefaultXsiType(clrType);
				var includedType = new XmlIncludedType(xsiType, clrType);
				includedTypes.Add(includedType);
			}
		}

		public XmlName GetDefaultXsiType(Type clrType)
		{
			if (clrType == this.clrType)
				return this.XsiType;

			IXmlIncludedType include;
			if (includedTypes.TryGet(clrType, out include))
				return include.XsiType;

			var kind = XmlTypeSerializer.For(clrType).Kind;
			switch (kind)
			{
				case XmlTypeKind.Complex:
					if (!clrType.GetTypeInfo().IsInterface) goto default;
					return GetXmlMetadata(clrType).XsiType;

				case XmlTypeKind.Collection:
					var itemClrType = clrType.GetCollectionItemType();
					var itemXsiType = GetDefaultXsiType(itemClrType);
					return new XmlName("ArrayOf" + itemXsiType.LocalName, null);

				default:
					return new XmlName(clrType.Name, null);
			}
		}

		public IEnumerable<IXmlIncludedType> GetIncludedTypes(Type baseType)
		{
			var queue   = new Queue<XmlMetadata>();
			var visited = new HashSet<Type>();
			XmlMetadata metadata;

			visited.Add(baseType);
			if (TryGetXmlMetadata(baseType, out metadata))
				queue.Enqueue(metadata);
			metadata = this;

			for (;;)
			{
				foreach (var includedType in metadata.IncludedTypes)
				{
					var clrType = includedType.ClrType;
					var relevant
						=  baseType != clrType
						&& baseType.IsAssignableFrom(clrType)
						&& visited.Add(clrType);

					if (!relevant)
						continue;

					yield return includedType;

					if (TryGetXmlMetadata(clrType, out metadata))
						queue.Enqueue(metadata);
				}

				if (queue.Count == 0)
					yield break;

				metadata = queue.Dequeue();
			}
		}

		private bool TryGetXmlMetadata(Type clrType, out XmlMetadata metadata)
		{
			var kind = XmlTypeSerializer.For(clrType).Kind;

			return kind == XmlTypeKind.Complex && clrType.GetTypeInfo().IsInterface
				? Try.Success(out metadata, GetXmlMetadata(clrType))
				: Try.Failure(out metadata);
		}

		private XmlMetadata GetXmlMetadata(Type clrType)
		{
			return source
				.GetAdapterMeta(clrType)
				.GetXmlMeta();
		}

		private string GetDefaultTypeLocalName(Type clrType)
		{
			var name = clrType.Name;
			return IsInterfaceName(name)
				? name.Substring(1)
				: name;
		}

		private static bool IsInterfaceName(string name)
		{
			return name.Length > 1
				&& name[0] == 'I'
				&& char.IsUpper(name, 1);
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
	}
}
#endif
