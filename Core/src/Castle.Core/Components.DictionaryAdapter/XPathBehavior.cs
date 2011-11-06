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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class XPathBehavior : DictionaryBehaviorAttribute, IDictionaryMetaInitializer
	{
		public static readonly XPathBehavior Instance = new XPathBehavior();

		void IDictionaryMetaInitializer.Initialize(IDictionaryAdapterFactory factory, DictionaryAdapterMeta dictionaryMeta)
		{
			var type = dictionaryMeta.Type;
			bool? qualified = null;
			bool? isNullable = null;
			string defaultNamespace = null;
			XmlTypeAttribute xmlType = null;
			XmlRootAttribute xmlRoot = null;
			List<Type> xmlIncludes = null;

			new BehaviorVisitor()
				.OfType<XmlTypeAttribute>(attrib => xmlType = attrib)
				.OfType<XmlRootAttribute>(attrib => xmlRoot = attrib)
				.OfType<XmlDefaultsAttribute>(attrib =>
				{
					qualified = attrib.Qualified;
					isNullable = attrib.IsNullable;
				})
				.OfType<XmlNamespaceAttribute>(attrib =>
				{
					if (attrib.Default)
					{
						defaultNamespace = attrib.NamespaceUri;
					}
				})
				.OfType<XmlIncludeAttribute>(attrib =>
				{
					xmlIncludes = xmlIncludes ?? new List<Type>();
					if (type != attrib.Type && type.IsAssignableFrom(attrib.Type))
					{
						xmlIncludes.Add(attrib.Type);
					}
				})
				.Apply(dictionaryMeta.Behaviors);

			if (xmlType == null)
			{
				xmlType = new XmlTypeAttribute();
			}

			if (string.IsNullOrEmpty(xmlType.TypeName))
			{
				xmlType.TypeName = type.Name;
				if (xmlType.TypeName.StartsWith("I"))
				{
					xmlType.TypeName = xmlType.TypeName.Substring(1);
				}
			}

			if (xmlType.Namespace == null)
			{
				xmlType.Namespace = defaultNamespace;
			}

			dictionaryMeta.SetXmlMeta(new XmlMetadata(type, qualified, isNullable, xmlType, xmlRoot, xmlIncludes));
		}
	}
#endif
}