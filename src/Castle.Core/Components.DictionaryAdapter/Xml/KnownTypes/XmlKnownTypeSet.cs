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
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XmlKnownTypeSet : IXmlTypeMap, IEnumerable<IXmlType>
	{
		private readonly List<IXmlType> items;
		private readonly Type baseType;
		private IXmlTypeMap parent;

		public XmlKnownTypeSet(Type baseType)
			: this(baseType, null) { }

		public XmlKnownTypeSet(Type baseType, IXmlTypeMap parent)
		{
			if (baseType == null)
				throw Error.ArgumentNull("baseType");

			this.items    = new List<IXmlType>();
			this.baseType = baseType;
			this.Parent   = parent;
		}

		public Type BaseType
		{
			get { return baseType; }
		}

		public IXmlTypeMap Parent
		{
			get { return parent; }
			set { parent = value ?? DefaultXmlKnownTypeSet.Instance; }
		}

		public IEnumerator<IXmlType> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		//public void Add(XmlArrayItemAttribute attribute)
		//{
		//    Add(new XmlNamedType(
		//        attribute.ElementName,
		//        attribute.Namespace,
		//        attribute.Type
		//    ));
		//}

		//public void Add(XmlElementAttribute attribute)
		//{
		//    Add(new XmlNamedType(
		//        attribute.ElementName,
		//        attribute.Namespace,
		//        attribute.Type
		//    ));
		//}

		//public void Add(XmlAttributeAttribute attribute)
		//{
		//    Add(new XmlNamedType(
		//        attribute.AttributeName,
		//        attribute.Namespace,
		//        attribute.Type
		//    ));
		//}

		public void Add(IXmlType xmlType)
		{
			items.Add(xmlType);
		}

		public bool TryGetClrType(IXmlType xmlType, out Type clrType)
		{
			foreach (var item in items)
				if (item.TryGetClrType(xmlType, out clrType))
					return true;

			return (parent != null)
				? parent.TryGetClrType(xmlType, out clrType)
				: Try.Failure(out clrType);
		}

		public bool TryGetXmlType(Type clrType, out IXmlType xmlType)
		{
			foreach (var item in items)
				if (item.TryGetXmlType(clrType, out xmlType))
					return true;

			return (parent != null)
				? parent.TryGetXmlType(clrType, out xmlType)
				: Try.Failure(out xmlType);
		}
	}
}
