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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XmlKnownTypeSet : IXmlKnownTypeMap, IEnumerable<IXmlKnownType>
	{
		private readonly List<IXmlKnownType> items;
		private readonly Type baseType;
		private IXmlKnownTypeMap parent;

		public XmlKnownTypeSet(Type baseType)
			: this(baseType, null) { }

		public XmlKnownTypeSet(Type baseType, IXmlKnownTypeMap parent)
		{
			this.items    = new List<IXmlKnownType>();
			this.baseType = baseType;
			Parent = parent;
		}

		public Type BaseType
		{
			get { return baseType; }
		}

		public IXmlKnownTypeMap Parent
		{
			get { return parent; }
			set { parent = value ?? DefaultXmlKnownTypeSet.Instance; }
		}

		public IEnumerator<IXmlKnownType> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public void Add(XmlArrayItemAttribute attribute)
		{
			Add(new XmlKnownType(
				attribute.ElementName,
				attribute.Namespace,
				attribute.Type
			));
		}

		public void Add(XmlElementAttribute attribute)
		{
			Add(new XmlKnownType(
				attribute.ElementName,
				attribute.Namespace,
				attribute.Type
			));
		}

		public void Add(XmlIncludeAttribute attribute)
		{
			Add(new XmlKnownType(
				attribute.Type
			));
		}

		protected void Add(XmlKnownType knownType)
		{
			items.Add(knownType);
		}

		public bool TryRecognizeType(IXmlNode node, out Type type)
		{
			foreach (var item in items)
				if (item.TryRecognizeType(node, out type))
					return true;

			if (parent != null)
				return parent.TryRecognizeType(node, out type);

			return Try.Failure(out type);
		}

		public IXmlKnownType GetXmlKnownType(Type type)
		{
			foreach (var item in items)
				if (item.ClrType == type)
					return item;

			if (parent != null)
				return parent.GetXmlKnownType(type);

			throw Error.NotXmlKnownType();
		}
	}
}
#endif
