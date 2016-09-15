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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class XmlKnownTypeSet : IXmlKnownTypeMap, IEnumerable<IXmlKnownType>
	{
		private readonly Dictionary<IXmlIdentity, IXmlKnownType> itemsByXmlIdentity;
		private readonly Dictionary<Type,         IXmlKnownType> itemsByClrType;
		private readonly Type defaultType;

		public XmlKnownTypeSet(Type defaultType)
		{
			if (defaultType == null)
				throw Error.ArgumentNull("defaultType");

			itemsByXmlIdentity = new Dictionary<IXmlIdentity, IXmlKnownType>(XmlIdentityComparer.Instance);
			itemsByClrType     = new Dictionary<Type,         IXmlKnownType>();
			this.defaultType   = defaultType;
		}

		public IXmlKnownType Default
		{
			get
			{
				IXmlKnownType knownType;
				if (defaultType == null || !TryGet(defaultType, out knownType))
					throw Error.NoDefaultKnownType();
				return knownType;
			}
		}

		public void Add(IXmlKnownType knownType, bool overwrite)
		{
			// All XmlTypes are present here
			if (overwrite || !itemsByXmlIdentity.ContainsKey(knownType))
				itemsByXmlIdentity[knownType] = knownType;

			// Only contains the default XmlType for each ClrType
			var clrType = knownType.ClrType;
			if (overwrite || !itemsByClrType.ContainsKey(clrType))
				itemsByClrType[clrType] = knownType;
		}

		public void AddXsiTypeDefaults()
		{
			// If there is only one xsi:type possible for a known local name and namespace URI,
			// add another XmlType to recognize nodes that don't provide the xsi:type.

			var bits = new Dictionary<IXmlKnownType, bool>(
				itemsByXmlIdentity.Count,
				XmlKnownTypeNameComparer.Instance);

			foreach (var knownType in itemsByXmlIdentity.Values)
			{
				bool bit;
				bits[knownType] = bits.TryGetValue(knownType, out bit)
					? false                               // another by same name; can't add a default
					: knownType.XsiType != XmlName.Empty; // first   by this name; can   add a default, if not already in default form
			}

			foreach (var pair in bits)
			{
				if (pair.Value)
				{
					var template  = pair.Key;
					var knownType = new XmlKnownType(template.Name, XmlName.Empty, template.ClrType);
					Add(knownType, true);
				}
			}
		}

		public bool TryGet(IXmlIdentity xmlIdentity, out IXmlKnownType knownType)
		{
			return itemsByXmlIdentity.TryGetValue(xmlIdentity, out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return itemsByClrType.TryGetValue(clrType, out knownType);
		}

		public IXmlKnownType[] ToArray()
		{
			return itemsByXmlIdentity.Values.ToArray();
		}

		public IEnumerator<IXmlKnownType> GetEnumerator()
		{
			return itemsByXmlIdentity.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return itemsByXmlIdentity.Values.GetEnumerator();
		}

		private sealed class XmlIdentityComparer : IEqualityComparer<IXmlIdentity>
		{
			public static readonly XmlIdentityComparer
				Instance = new XmlIdentityComparer();

			private XmlIdentityComparer() { }

			public bool Equals(IXmlIdentity x, IXmlIdentity y)
			{
				var nameX = x.Name;
				var nameY = y.Name;

				if (!NameComparer.Equals(nameX.LocalName, nameY.LocalName))
					return false;

				if (!XsiTypeComparer.Equals(x.XsiType, y.XsiType))
					return false;

				return nameX.NamespaceUri == null
					|| nameY.NamespaceUri == null
					|| NameComparer.Equals(nameX.NamespaceUri, nameY.NamespaceUri);
			}

			public int GetHashCode(IXmlIdentity name)
			{
				var code = NameComparer.GetHashCode(name.Name.LocalName);

				if (name.XsiType != XmlName.Empty)
					code = (code << 7 | code >> 25)
					     ^ XsiTypeComparer.GetHashCode(name.XsiType);

				// DO NOT include NamespaceUri in hash code.
				// That would break 'null means any' behavior.

				return code;
			}
		}

		private sealed class XmlKnownTypeNameComparer : IEqualityComparer<IXmlKnownType>
		{
			public static readonly XmlKnownTypeNameComparer
				Instance = new XmlKnownTypeNameComparer();

			private XmlKnownTypeNameComparer() { }

			public bool Equals(IXmlKnownType knownTypeA, IXmlKnownType knownTypeB)
			{
				return XmlNameComparer.IgnoreCase.Equals(knownTypeA.Name, knownTypeB.Name);
			}

			public int GetHashCode(IXmlKnownType knownType)
			{
				return XmlNameComparer.IgnoreCase.GetHashCode(knownType.Name);
			}
		}

		private static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;

		private static readonly XmlNameComparer
			XsiTypeComparer = XmlNameComparer.Default;
	}
}
#endif
