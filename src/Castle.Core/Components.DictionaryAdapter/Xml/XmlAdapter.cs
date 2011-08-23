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
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.XPath;
	using System.Xml.Serialization;

	public class XmlAdapter : DictionaryBehaviorAttribute,
		IDictionaryInitializer,
		IDictionaryPropertyGetter,
		IDictionaryPropertySetter,
		IXPathNavigable
	{
		private readonly ILazy<XPathNavigator> node;
//		private readonly XmlAdapter parent;
		private XmlMetadata primaryXmlMeta;
		private Dictionary<Type, XmlMetadata> secondaryXmlMetas;

		public XmlAdapter()
			: this(new XmlDocument()) { }

		public XmlAdapter(IXPathNavigable storage)
			: this(GetNode(storage), null) { }

		internal XmlAdapter(ILazy<XPathNavigator> node)
			: this(node, null) { }

		protected XmlAdapter(ILazy<XPathNavigator> node, XmlAdapter parent)
		{
			this.node   = node;
//			this.parent = parent;
		}

		private static ILazy<XPathNavigator> GetNode(IXPathNavigable storage)
		{
			if (storage == null)
				throw new ArgumentNullException("storage");

			var node = storage.CreateNavigator();
			var lazy = new SingleIterator<XPathNavigator>(node);
			lazy.MoveNext();
			return lazy;
		}

		public static XmlAdapter For(object obj)
		{
			if (obj == null)
				throw Error.ArgumentNull("obj");

			var dictionaryAdapter = obj as IDictionaryAdapter;
			if (dictionaryAdapter == null)
				throw Error.ArgumentNotDictionaryAdapter("obj");

			var descriptor = dictionaryAdapter.This.Descriptor;
			if (descriptor == null)
				throw Error.NoInstanceDescriptor();

			var getters = descriptor.Getters;
			if (getters == null)
				throw Error.NoGetterOnInstanceDescriptor();

			var xmlAdapter = getters.OfType<XmlAdapter>().SingleOrDefault();
			if (xmlAdapter == null)
				throw Error.NoXmlAdapter();

			return xmlAdapter;
		}

		public XPathNavigator CreateNavigator()
		{
			if (SeekNode(false))
				return node.Value.Clone();
			else
				throw Error.NoCurrentItem();
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			if (primaryXmlMeta == null)
				InitializePrimary(dictionaryAdapter.Meta, behaviors);
			else
				InitializeSecondary(dictionaryAdapter.Meta, behaviors);
		}

		private void InitializePrimary(DictionaryAdapterMeta meta, object[] behaviors)
		{
			if (!meta.HasXmlMeta())
				throw Error.NoXmlMetadata(meta.Type);

			primaryXmlMeta = meta.GetXmlMeta();
		}

		private void InitializeSecondary(DictionaryAdapterMeta meta, object[] behaviors)
		{
			if (secondaryXmlMetas == null)
				secondaryXmlMetas = new Dictionary<Type, XmlMetadata>();

			XmlMetadata item;
			if (!secondaryXmlMetas.TryGetValue(meta.Type, out item))
				secondaryXmlMetas[meta.Type] = meta.GetXmlMeta();
		}

		public override IDictionaryBehavior Copy()
		{
			return null;
		}

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			XmlPropertyAccessor accessor;
			return SeekNode(false) && TryGetAccessor(property, null != storedValue, out accessor)
				? accessor.GetPropertyValue(node.Value, dictionaryAdapter, ifExists)
				: storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			XmlPropertyAccessor accessor;
			if (TryGetAccessor(property, false, out accessor) && SeekNode(true))
				accessor.SetPropertyValue(node.Value, value);
			return true;
		}

		// TODO: Get this out of here ... it's a cohesion problem
		private bool SeekNode(bool required)
		{
			if (!node.HasValue && !required)
				return false;

			var n = node.Value;
			if (n.NodeType == XPathNodeType.Element)
				return true;
			if (n.NodeType != XPathNodeType.Root)
				return false;
			if (n.MoveToChild(XPathNodeType.Element))
				return true;
			if (!required)
				return false;

			var accessor = new XmlElementPropertyAccessor(
				typeof(string),
			    primaryXmlMeta.RootLocalName,
			    primaryXmlMeta.RootNamespaceUri);

			var newNode = accessor.EnsurePropertyNode(n);
			n.MoveTo(newNode);
			return true;
		}

		private bool TryGetAccessor(PropertyDescriptor property, bool requireVolatile, out XmlPropertyAccessor accessor)
		{
			accessor = null;
			var count = 0;

			foreach (var behavior in property.Behaviors)
			{
				if (IsIgnoreBehavior(behavior))
					return false;
				else if (IsVolatileBehavior(behavior))
					requireVolatile = false;
				else if (TryGetAccessor(property, behavior, ref accessor))
					count++;
			}

			if (count > 1)
				throw Error.AttributeConflict(property);
			if (requireVolatile)
				return false;
			if (count == 0)
				accessor = new XmlDefaultPropertyAccessor(property.PropertyType, property.PropertyName);
			return true;
		}

		private bool TryGetAccessor(PropertyDescriptor property, object behavior, ref XmlPropertyAccessor accessor)
		{
			return TryGetXmlElementAccessor(property, behavior, ref accessor);
		}

		private bool TryGetXmlElementAccessor(PropertyDescriptor property, object behavior, ref XmlPropertyAccessor accessor)
		{
			var attribute = behavior as XmlElementAttribute;
			if (attribute == null) return false;

			var localName = string.IsNullOrEmpty(attribute.ElementName)
				? property.PropertyName
				: attribute.ElementName;

			var namespaceUri = string.IsNullOrEmpty(attribute.Namespace)
				? null
				: attribute.Namespace;				

			accessor = new XmlElementPropertyAccessor(property.PropertyType, localName, namespaceUri);
			return true;
		}

		private static bool IsIgnoreBehavior(object behavior)
		{
			return behavior is XmlIgnoreAttribute;
		}

		private static bool IsVolatileBehavior(object behavior)
		{
			return behavior is VolatileAttribute;
		}
	}
}
#endif
