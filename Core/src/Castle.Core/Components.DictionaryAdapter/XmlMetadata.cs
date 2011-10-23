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
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class XmlMetadata
	{
		public XmlMetadata(Type type, bool? qualified, bool? isNullable, XmlTypeAttribute xmlType,
						   XmlRootAttribute xmlRoot, IEnumerable<Type> xmlIncludes)
		{
			Type = type;
			Qualified = qualified;
			IsNullable = isNullable;
			XmlType = xmlType;
			XmlRoot = xmlRoot;
			XmlIncludes = xmlIncludes;
		}

		public Type Type { get; private set; }

		public bool? Qualified { get; private set; }

		public bool? IsNullable { get; private set; }

		public XmlTypeAttribute XmlType { get; private set; }

		public XmlRootAttribute XmlRoot { get; private set; }

		public IEnumerable<Type> XmlIncludes { get; private set; }
	}
}
