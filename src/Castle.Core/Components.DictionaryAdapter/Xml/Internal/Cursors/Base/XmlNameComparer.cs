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
	using System.Collections.Generic;

	public class XmlNameComparer : IEqualityComparer<XmlName>
	{
		public static readonly XmlNameComparer
			Default    = new XmlNameComparer(StringComparer.Ordinal),
			IgnoreCase = new XmlNameComparer(StringComparer.OrdinalIgnoreCase);

		private readonly StringComparer comparer;

		private XmlNameComparer(StringComparer comparer)
		{
			this.comparer = comparer;
		}

		public int GetHashCode(XmlName name)
		{
			var code = (name.LocalName != null)
				? comparer.GetHashCode(name.LocalName)
				: 0;

			if (name.NamespaceUri != null)
				code = (code << 7 | code >> 25)
					 ^ comparer.GetHashCode(name.NamespaceUri);

			return code;
		}

		public bool Equals(XmlName x, XmlName y)
		{
			return comparer.Equals(x.LocalName,    y.LocalName)
				&& comparer.Equals(x.NamespaceUri, y.NamespaceUri);
		}
	}
}
#endif
