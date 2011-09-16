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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class XmlKnownTypeSet : IXmlTypeMap, IEnumerable<IXmlType>
	{
		private readonly Dictionary<IXmlName, IXmlType> itemsByXmlName;
		private readonly Dictionary<Type,     IXmlType> itemsByClrType;
		private readonly Type baseType;

		public XmlKnownTypeSet(Type baseType)
		{
			if (baseType == null)
				throw Error.ArgumentNull("baseType");

			itemsByXmlName = new Dictionary<IXmlName, IXmlType>(XmlNameComparer.Instance);
			itemsByClrType = new Dictionary<Type,     IXmlType>();
			this.baseType  = baseType;
		}

		public Type BaseType
		{
			get { return baseType; }
		}

		public void Add(IXmlType xmlType)
		{
			// All XmlTypes are present here
			itemsByXmlName.Add(xmlType, xmlType);

			// Only contains the default XmlType for each ClrType
			itemsByClrType[xmlType.ClrType] = xmlType;
		}

		public void AddXsiTypeDefaults()
		{
			// If there is only one xsi:type possible for a known local name and namespace URI,
			// add another XmlType to recognize nodes that don't provide the xsi:type.

			var bits = new Dictionary<IXmlType, bool>(
				itemsByXmlName.Count,
				XmlNameGroupingComparer.Instance);

			foreach (var xmlType in itemsByXmlName.Values)
			{
				bool bit;
				bits[xmlType] = bits.TryGetValue(xmlType, out bit)
					? false                    // another by same name; can't add a default
					: xmlType.XsiType != null; // first   by this name; can   add a default, if not already in default form
			}

			foreach (var pair in bits)
			{
				if (pair.Value)
				{
					var xmlType = pair.Key;
					Add(new XmlKnownType
					(
						xmlType.LocalName,
						xmlType.NamespaceUri,
						null,
						xmlType.ClrType
					));
				}
			}
		}

		public bool TryGetClrType(IXmlName xmlName, out Type clrType)
		{
			IXmlType xmlType;
			return itemsByXmlName.TryGetValue(xmlName, out xmlType)
				? Try.Success(out clrType, xmlType.ClrType)
				: Try.Failure(out clrType);
		}

		public bool TryGetXmlName(Type clrType, out IXmlName xmlName)
		{
			IXmlType xmlType;
			return itemsByClrType.TryGetValue(clrType, out xmlType)
				? Try.Success(out xmlName, xmlType)
				: Try.Failure(out xmlName);
		}

		public IEnumerator<IXmlType> GetEnumerator()
		{
			return itemsByXmlName.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return itemsByXmlName.Values.GetEnumerator();
		}

		private sealed class XmlNameComparer : IEqualityComparer<IXmlName>
		{
			public static readonly XmlNameComparer
				Instance = new XmlNameComparer();

			private XmlNameComparer() { }

			public bool Equals(IXmlName x, IXmlName y)
			{
				string xNamespaceUri, yNamespaceUri;
				return NameComparer.Equals(x.LocalName, y.LocalName)
					&& NameComparer.Equals(x.XsiType,   y.XsiType)
					&& ((xNamespaceUri = x.NamespaceUri) == null
					 || (yNamespaceUri = y.NamespaceUri) == null
					 || NameComparer.Equals(xNamespaceUri, yNamespaceUri));
			}

			public int GetHashCode(IXmlName name)
			{
				var code = NameComparer.GetHashCode(name.LocalName);				
				var xsiType = name.XsiType;
				if (xsiType != null)
					code = (code << 7 | code >> 25)
					     ^ NameComparer.GetHashCode(xsiType);
				return code;
				// Do not include NamespaceUri in hash code.
				// That would break 'null means any' behavior.
			}
		}

		private sealed class XmlNameGroupingComparer : IEqualityComparer<IXmlName>
		{
			public static readonly XmlNameGroupingComparer
				Instance = new XmlNameGroupingComparer();

			private XmlNameGroupingComparer() { }

			public bool Equals(IXmlName x, IXmlName y)
			{
				return NameComparer.Equals(x.LocalName,    y.LocalName)
					&& NameComparer.Equals(x.NamespaceUri, y.NamespaceUri);
			}

			public int GetHashCode(IXmlName name)
			{
				var code = NameComparer.GetHashCode(name.LocalName);
				var namespaceUri = name.NamespaceUri;
				if (namespaceUri != null)
					code = (code << 7 | code >> 25)
					     ^ NameComparer.GetHashCode(namespaceUri);
				return code;
			}
		}

		private static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
