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
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using IBindingList         = System.ComponentModel.IBindingList;
	using ListChangedEventArgs = System.ComponentModel.ListChangedEventArgs;
	using ListChangedType      = System.ComponentModel.ListChangedType;
	using System.Collections;

	public class XmlAdapter : DictionaryBehaviorAttribute,
		IDictionaryInitializer,
		IDictionaryPropertyGetter,
		IDictionaryPropertySetter,
		IDictionaryCreateStrategy,
		IDictionaryCopyStrategy
	{
		private IXmlNode node;
		private object source;
		private XmlMetadata primaryXmlMeta;
		private Dictionary<Type, XmlMetadata> secondaryXmlMetas;

#if !SILVERLIGHT
		public XmlAdapter()
		    : this(new XmlDocument()) { }

		public XmlAdapter(XmlNode node)
		{
		    if (node == null)
		        throw Error.ArgumentNull("node");
			this.source = node;
		}
#endif
		public XmlAdapter(IXmlNode node)
		{
		    if (node == null)
		        throw Error.ArgumentNull("node");
			this.node = node;
		}

		public IXmlNode Node
		{
			get { return node; }
		}

		object IDictionaryCreateStrategy.Create(IDictionaryAdapter parent, Type type, IDictionary dictionary)
		{
#if !SILVERLIGHT
			var adapter = new XmlAdapter(new XmlDocument());
#endif
#if SILVERLIGHT
			// TODO: Create XNode-based XmlAdapter
#endif
			return parent.CreateChildAdapter(type, adapter, dictionary);
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			if (primaryXmlMeta == null)
				InitializePrimary  (dictionaryAdapter.Meta, dictionaryAdapter);
			else
				InitializeSecondary(dictionaryAdapter.Meta);
		}

		private void InitializePrimary(DictionaryAdapterMeta meta, IDictionaryAdapter dictionaryAdapter)
		{
			if (!meta.HasXmlMeta())
				throw Error.NoXmlMetadata(meta.Type);

			primaryXmlMeta = meta.GetXmlMeta();

			InitializeBaseTypes(meta);
			InitializeStrategies(dictionaryAdapter);

			if (node == null)
				node = GetBaseNode();
		}

		private void InitializeSecondary(DictionaryAdapterMeta meta)
		{
			if (secondaryXmlMetas == null)
				secondaryXmlMetas = new Dictionary<Type, XmlMetadata>();

			XmlMetadata item;
			if (!secondaryXmlMetas.TryGetValue(meta.Type, out item))
				secondaryXmlMetas[meta.Type] = meta.GetXmlMeta();
		}

		private void InitializeBaseTypes(DictionaryAdapterMeta meta)
		{
			foreach (var type in meta.Type.GetInterfaces())
			{
				var ns = type.Namespace;
				var ignore
					=  ns == "Castle.Components.DictionaryAdapter"
					|| ns == "System.ComponentModel";
				if (ignore) continue;

				var baseMeta = meta.GetDictionaryAdapterMeta(type);
				InitializeSecondary(baseMeta);
			}
		}

		private void InitializeStrategies(IDictionaryAdapter dictionaryAdapter)
		{
			var instance = dictionaryAdapter.This;

			if (instance.CreateStrategy == null)
			{
				instance.CreateStrategy = this;
				instance.AddCopyStrategy(this);
			}
		}

		bool IDictionaryCopyStrategy.Copy(IDictionaryAdapter source, IDictionaryAdapter target, ref Func<PropertyDescriptor, bool> selector)
		{
			if (selector == null)
				selector = property => HasProperty(property.PropertyName, source);
			return false;
		}

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			XmlAccessor accessor;
			if (TryGetAccessor(dictionaryAdapter, property, null != storedValue, out accessor))
			{
				storedValue = accessor.GetPropertyValue(node, dictionaryAdapter, !ifExists);
				if (null != storedValue)
				{
					AttachObservers(storedValue, dictionaryAdapter, property);
					dictionaryAdapter.StoreProperty(property, key, storedValue);
				}
			}
			return storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			XmlAccessor accessor;
			if (TryGetAccessor(dictionaryAdapter, property, false, out accessor))
			{
				if (value != null && dictionaryAdapter.ShouldClearProperty(property, value))
					value = null;
				accessor.SetPropertyValue(node, dictionaryAdapter, ref value);
			}
			return true;
		}

		private IXmlNode GetBaseNode()
		{
			var node = GetSourceNode();

			if (node.IsElement)
				return node;
			if (node.IsAttribute)
				throw Error.NotSupported();
			// must be root
			
			var cursor = primaryXmlMeta.SelectBase(node);
			return cursor.MoveNext()
				? cursor.Save()
				: cursor;
		}

		private IXmlNode GetSourceNode()
		{
#if !SILVERLIGHT
			var xmlNode = source as XmlNode;
			if (xmlNode != null)
				return new SysXmlNode(xmlNode, primaryXmlMeta.ClrType);
#endif

			throw Error.NotSupported();
		}

		private bool TryGetAccessor(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, bool requireVolatile, out XmlAccessor accessor)
		{
			accessor = property.HasAccessor()
				? property.GetAccessor()
				: CreateAccessor(dictionaryAdapter, property);

			if (accessor.IsIgnored)
			    return Try.Failure(out accessor);
			if (requireVolatile && !accessor.IsVolatile)
			    return Try.Failure(out accessor);
			return true;
		}

		private XmlAccessor CreateAccessor(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var accessor   = null as XmlAccessor;
			var isVolatile = false;

			foreach (var behavior in property.Behaviors)
			{
				if (IsIgnoreBehavior(behavior))
					return XmlIgnoreBehaviorAccessor.Instance;
				else if (IsVolatileBehavior(behavior))
					isVolatile = true;
				TryApplyBehavior(dictionaryAdapter, property, behavior, ref accessor);
			}

			if (accessor == null)
			{
				var factory = XmlDefaultBehaviorAccessor.Factory;
				accessor = CreateAccessor(factory, dictionaryAdapter, property);
			}

			accessor.ConfigureVolatile(isVolatile);
			accessor.Prepare();
			property.SetAccessor(accessor);
			return accessor;
		}

		private bool TryApplyBehavior(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object behavior, ref XmlAccessor accessor)
		{	
			return
				TryApplyBehavior<XmlElementAttribute, XmlElementBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlElementBehaviorAccessor.Factory)
				||
				TryApplyBehavior<KeyAttribute, XmlArrayBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XmlArrayAttribute, XmlArrayBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XmlArrayItemAttribute, XmlArrayBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlArrayBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XmlAttributeAttribute, XmlAttributeBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlAttributeBehaviorAccessor.Factory)
#if !SL3
				||
				TryApplyBehavior<XPathAttribute, XmlXPathBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlXPathBehaviorAccessor.Factory)
				||
				TryApplyBehavior<XPathFunctionAttribute, XmlXPathBehaviorAccessor>
					(dictionaryAdapter, property, behavior, ref accessor, XmlXPathBehaviorAccessor.Factory)
#endif
				;
		}

		private bool TryApplyBehavior<TBehavior, TAccessor>(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object behavior, ref XmlAccessor accessor,
			XmlAccessorFactory<TAccessor> factory)
			where TBehavior : class
			where TAccessor : XmlAccessor, IConfigurable<TBehavior>
		{
			var typedBehavior = behavior as TBehavior;
			if (typedBehavior == null)
				return false;

			if (accessor == null)
				accessor = CreateAccessor(factory, dictionaryAdapter, property);

			var typedAccessor = accessor as TAccessor;
			if (typedAccessor == null)
				throw Error.AttributeConflict(property);

			typedAccessor.Configure(typedBehavior);
			return true;
		}

		private TAccessor CreateAccessor<TAccessor>(
			XmlAccessorFactory<TAccessor> factory,
			IDictionaryAdapter dictionaryAdapter,
			PropertyDescriptor property)
			where TAccessor : XmlAccessor
		{
			var xmlMeta = GetXmlMetadata(property.Property.DeclaringType /* dictionaryAdapter.Meta.Type */);
			return factory(property, xmlMeta);
		}

		private XmlMetadata GetXmlMetadata(Type type)
		{
			if (type == primaryXmlMeta.ClrType)
				return primaryXmlMeta;

			XmlMetadata xmlMeta;
			if (secondaryXmlMetas.TryGetValue(type, out xmlMeta))
				return xmlMeta;

			throw Error.UnrecognizedType(type);
		}

		private static bool IsIgnoreBehavior(object behavior)
		{
			return behavior is XmlIgnoreAttribute;
		}

		private static bool IsVolatileBehavior(object behavior)
		{
			return behavior is VolatileAttribute;
		}

		private void AttachObservers(object value, IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var bindingList = value as IBindingList;
			if (bindingList != null)
				bindingList.ListChanged += (s,e) => HandleListChanged(s, e, dictionaryAdapter, property);
		}

		private void HandleListChanged(object value, ListChangedEventArgs args, IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			var change = args.ListChangedType;
			var changed
				=  change == ListChangedType.ItemAdded
				|| change == ListChangedType.ItemDeleted
				|| change == ListChangedType.ItemMoved
				|| change == ListChangedType.Reset;

			if (changed && dictionaryAdapter.ShouldClearProperty(property, value))
			{
				value = null;
				dictionaryAdapter.SetProperty(property.PropertyName, ref value);
			}
		}

		public override IDictionaryBehavior Copy()
		{
			return null;
		}

		public static XmlAdapter For(object obj)
		{
			return For(obj, true);
		}

		public static XmlAdapter For(object obj, bool required)
		{
			if (obj == null)
				if (!required) return null;
				else throw Error.ArgumentNull("obj");

			var dictionaryAdapter = obj as IDictionaryAdapter;
			if (dictionaryAdapter == null)
				if (!required) return null;
				else throw Error.ArgumentNotDictionaryAdapter("obj");

			var descriptor = dictionaryAdapter.This.Descriptor;
			if (descriptor == null)
				if (!required) return null;
				else throw Error.NoInstanceDescriptor();

			var getters = descriptor.Getters;
			if (getters == null)
				if (!required) return null;
				else throw Error.NoGetterOnInstanceDescriptor();

			var xmlAdapter = getters.OfType<XmlAdapter>().SingleOrDefault();
			if (xmlAdapter == null)
				if (!required) return null;
				else throw Error.NoXmlAdapter();

			return xmlAdapter;
		}

		public static bool IsPropertyDefined(string propertyName, IDictionaryAdapter dictionaryAdapter)
		{
			var xmlAdapter = XmlAdapter.For(dictionaryAdapter, true);
			return xmlAdapter != null
				&& xmlAdapter.HasProperty(propertyName, dictionaryAdapter);
		}

		public bool HasProperty(string propertyName, IDictionaryAdapter dictionaryAdapter)
		{
			var key = dictionaryAdapter.GetKey(propertyName);
			if (key == null)
				return false;

			PropertyDescriptor property;
			XmlAccessor accessor;
			return dictionaryAdapter.This.Properties.TryGetValue(propertyName, out property)
				&& TryGetAccessor(dictionaryAdapter, property, false, out accessor)
				&& accessor.IsPropertyDefined(node);
		}
	}
}
