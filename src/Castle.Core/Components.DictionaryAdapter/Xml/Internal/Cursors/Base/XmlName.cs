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

	public struct XmlName : IEquatable<XmlName>
	{
		public static readonly XmlName Empty = default(XmlName);

		private readonly string localName;
		private readonly string namespaceUri;

		public XmlName(string localName, string namespaceUri)
		{
			this.localName    = localName;
			this.namespaceUri = namespaceUri;
		}

		public string LocalName
		{
			get { return localName; }
		} 

		public string NamespaceUri
		{
			get { return namespaceUri; }
		} 

		public override int GetHashCode()
		{
			return XmlNameComparer.Default.GetHashCode(this);
		}

		public bool Equals(XmlName other)
		{
			return XmlNameComparer.Default.Equals(this, other);
		}

		public override bool Equals(object obj)
		{
			return obj is XmlName
				&& Equals((XmlName) obj);
		}

		public static bool operator == (XmlName x, XmlName y)
		{
			return XmlNameComparer.Default.Equals(x, y);
		}

		public static bool operator != (XmlName x, XmlName y)
		{
			return ! XmlNameComparer.Default.Equals(x, y);
		}

		public XmlName WithNamespaceUri(string namespaceUri)
		{
			return new XmlName(localName, namespaceUri);
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(localName))
				return string.Empty;
			if (string.IsNullOrEmpty(namespaceUri))
				return localName;
			return string.Concat(namespaceUri, ":", localName);
		}

		public static XmlName ParseQName(string text)
		{
			if (text == null)
				throw Error.ArgumentNull("text");

			var index = text.IndexOf(':');
			if (index == -1)
				return new XmlName(text, null);
			if (index == 0)
				return new XmlName(text.Substring(1), null);
			if (index == text.Length)
				return new XmlName(text.Substring(0, index), null);

			return new XmlName(
				text.Substring(index + 1),
				text.Substring(0, index));
		}
	}
}
#endif
