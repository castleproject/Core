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

	internal struct XmlQualifiedName : IEquatable<XmlQualifiedName>
	{
		private readonly string prefix;
		private readonly string localName;

		public XmlQualifiedName(string prefix, string localName)
		{
			if (localName == null)
				throw Error.ArgumentNull("localName");

			this.prefix    = prefix ?? string.Empty;
			this.localName = localName;
		}

		public override int GetHashCode()
		{
			int code;
			code  = prefix.GetHashCode();
			code  = (code << 7 | code >> 25);
			code ^= localName.GetHashCode();
			return code;
		}

		public override bool Equals(object obj)
		{
			return obj is XmlQualifiedName
				&& Equals((XmlQualifiedName) obj);
		}

		public bool Equals(XmlQualifiedName other)
		{
			return prefix    == other.prefix
				&& localName == other.localName;
		}
	}
}
