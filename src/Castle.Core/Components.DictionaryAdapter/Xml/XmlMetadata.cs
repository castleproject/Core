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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
using System.Xml.XPath;

	public class XmlMetadata
	{
		private readonly Type type;
		private readonly XmlDefaultsAttribute xmlDefaults;
		private readonly XmlRootAttribute xmlRoot;
		private readonly XmlTypeAttribute xmlType;
		private readonly XPathAttribute xPath;
		private readonly Type[] includes;

		public XmlMetadata(DictionaryAdapterMeta meta)
		{
			type = meta.Type;

			XmlIncludeAttribute xmlInclude;
			var includes = new List<Type>();

			foreach (var behavior in meta.Behaviors)
			{
				if      (TryCast(behavior, out xmlDefaults)) { /* NOP */ }
				else if (TryCast(behavior, out xmlRoot    )) { /* NOP */ }
				else if (TryCast(behavior, out xmlType    )) { /* NOP */ }
				else if (TryCast(behavior, out xmlInclude )) { includes.Add(xmlInclude.Type); }
				else if (TryCast(behavior, out xPath      )) { /* NOP */ }
			}

			this.includes = includes.ToArray();
		}

		private static bool TryCast<T>(object obj, out T behavior)
			where T : class
		{
			return null != (behavior = obj as T);
		}

		public Type Type
		{
			get { return type; }
		}

		public bool? Qualified
		{
			get { return xmlDefaults == null ? null as bool? : xmlDefaults.Qualified; }
		}

		public bool? IsNullable
		{
			get { return xmlDefaults == null ? null as bool? : xmlDefaults.IsNullable; }
		}

		public string RootLocalName
		{
			get
			{
				return (xmlRoot == null ? null : xmlRoot.ElementName)
					?? (xmlType == null ? null : xmlType.TypeName)
					?? type.Name;
			}
		}

		public string RootNamespaceUri
		{
			get
			{
				return (xmlRoot == null ? null : xmlRoot.Namespace)
					?? string.Empty;
			}
		}

		public string ChildNamespaceUri
		{
			get
			{
				return (xmlType == null ? null : xmlType.Namespace)
					?? (xmlRoot == null ? null : xmlRoot.Namespace)
					?? string.Empty;
			}
		}

		public ICompiledPath Path
		{
			get { return xPath == null ? null : xPath.Path; }
		}

		public Type[] Includes
		{
			get { return includes; }
		}
	}
}
#endif
	