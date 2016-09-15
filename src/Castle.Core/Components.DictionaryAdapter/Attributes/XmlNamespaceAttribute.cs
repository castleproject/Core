// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;

	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class XmlNamespaceAttribute : Attribute
	{
		public XmlNamespaceAttribute(string namespaceUri, string prefix)
		{
			NamespaceUri = namespaceUri;
			Prefix = prefix;
		}

		public bool Root { get; set; }

		public bool Default { get; set; }

		public string NamespaceUri { get; private set; }

		public string Prefix { get; private set; }
	}
}
