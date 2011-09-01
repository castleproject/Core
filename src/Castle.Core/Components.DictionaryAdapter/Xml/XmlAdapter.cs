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
		private XPathNavigator node;
		private XmlIterator source;
		private XmlMetadata primaryXmlMeta;
		private Dictionary<Type, XmlMetadata> secondaryXmlMetas;

		public XmlAdapter()
			: this(new XmlDocument()) { }

		public XmlAdapter(IXPathNavigable storage)
		{
			if (storage == null)
				throw new ArgumentNullException("storage");

			this.node = storage.CreateNavigator();
		}

		internal XmlAdapter(XmlIterator source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (source.HasCurrent)
				throw Error.NotSupported();

			this.source = source;
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
				return node.Clone();
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

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			XmlAccessor accessor;
			return SeekNode(false) && TryGetAccessor(property, null != storedValue, out accessor)
				? accessor.GetPropertyValue(node, dictionaryAdapter, !ifExists)
				: storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			XmlAccessor accessor;
			if (TryGetAccessor(property, false, out accessor) && SeekNode(true))
				accessor.SetPropertyValue(node, value);
			return true;
		}

		private bool SeekNode(bool create)
		{
			return Materialize(create)
				&& primaryXmlMeta.MoveToBase(node, create);
		}

		private bool Materialize(bool create)
		{
			if (node != null)
				return true;
			if (!create)
				return false;
			node = source.Create(primaryXmlMeta.ClrType);
			return true;
		}

		private bool TryGetAccessor(PropertyDescriptor property, bool requireVolatile, out XmlAccessor accessor)
		{
			if (property.HasAccessor())
				return Try.Success(out accessor, property.GetAccessor());

			accessor = null;

			foreach (var behavior in property.Behaviors)
			{
				if (IsIgnoreBehavior(behavior))
					return false;
				else if (IsVolatileBehavior(behavior))
					requireVolatile = false;
				TryApplyBehavior(property, behavior, ref accessor);
			}

			if (requireVolatile)
				return false;
			if (accessor == null)
				accessor = new XmlDefaultBehaviorAccessor(property, primaryXmlMeta.KnownTypes);

			accessor.Prepare();
			property.SetAccessor(accessor);
			return true;
		}

		private bool TryApplyBehavior(PropertyDescriptor property, object behavior, ref XmlAccessor accessor)
		{	
			return
				TryApplyBehavior<XmlElementAttribute, XmlElementBehaviorAccessor>
					(property, behavior, ref accessor, XmlElementBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XmlArrayAttribute, XmlArrayBehaviorAccessor>
					(property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XmlArrayItemAttribute, XmlArrayBehaviorAccessor>
					(property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				;
		}

		private bool TryApplyBehavior<TBehavior, TAccessor>(PropertyDescriptor property, object behavior, ref XmlAccessor accessor,
			XmlAccessorFactory<TAccessor> factory)
			where TBehavior : class
			where TAccessor : XmlAccessor, IConfigurable<TBehavior>
		{
			var typedBehavior = behavior as TBehavior;
			if (typedBehavior == null)
				return false;

			if (accessor == null)
				accessor = factory(property, primaryXmlMeta.KnownTypes);

			var typedAccessor = accessor as TAccessor;
			if (typedAccessor == null)
				throw Error.AttributeConflict(property);

			typedAccessor.Configure(typedBehavior);
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

		public override IDictionaryBehavior Copy()
		{
			return null;
		}
	}
}
#endif
