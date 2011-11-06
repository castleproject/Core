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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XPathAdapter : DictionaryBehaviorAttribute, IDictionaryInitializer,
								IDictionaryPropertyGetter, IDictionaryPropertySetter,
								IDictionaryCreateStrategy, IDictionaryCopyStrategy
	{
		private readonly Func<XPathNavigator> createRoot;
		private Dictionary<Type, XPathContext> overlays;
		private XmlMetadata rootXmlMeta;
		private XPathNavigator root;

		public XPathNavigator Root
		{
			get { return EnsureOffRoot(); }
		}

		public XPathAdapter Parent { get; private set; }

		public IXPathNavigable Source { get; private set; }

		public XPathContext Context { get; private set; }

		#region Init

		public XPathAdapter() : this(new XmlDocument())
		{	
		}

		public XPathAdapter(IXPathNavigable source)
		{
			Source = source;
			Context = new XPathContext();
			root = source.CreateNavigator();
		}

		protected XPathAdapter(XPathNavigator source, XPathAdapter parent)
		{
			Parent = parent;
			Context = parent.Context.CreateChild(null);
			root = source.Clone();
		}

		protected XPathAdapter(Func<XPathNavigator> createSource, XPathAdapter parent)
		{
			Parent = parent;
			Context = parent.Context.CreateChild(null);
			createRoot = createSource;
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			var meta = dictionaryAdapter.Meta;
			if (meta.MetaInitializers.OfType<XPathBehavior>().FirstOrDefault() == null)
			{
				throw new InvalidOperationException(string.Format(
					"Interface {0} requested xpath support, but was not configured properly.  " +
					"Did you forget to add an XPathBehavior?", meta.Type.FullName));
			}

			var xmlMeta = dictionaryAdapter.GetXmlMeta();

			if (dictionaryAdapter.This.CreateStrategy == null)
			{
				dictionaryAdapter.This.CreateStrategy = this;
				dictionaryAdapter.This.AddCopyStrategy(this);
			}

			if (rootXmlMeta == null)
			{
				rootXmlMeta = xmlMeta;
				Context.ApplyBehaviors(rootXmlMeta, behaviors);

				if (Parent == null)
				{
					foreach (var behavior in behaviors)
					{
						if (behavior is XPathAttribute)
						{
							var attrib = (XPathAttribute)behavior;
							var compiledExpression = attrib.CompiledExpression;
							if (MoveOffRoot(root, XPathNodeType.Element) == false || Context.Matches(compiledExpression, root))
							{
								break;
							}

							var navigator = Context.SelectSingleNode(compiledExpression, root);
							if (navigator != null)
							{
								root = navigator;
								break;
							}
						}
					}
					MoveOffRoot(root, XPathNodeType.Element);
				}
			}
			else
			{
				if (overlays == null)
					overlays = new Dictionary<Type, XPathContext>();

				XPathContext overlay;
				if (overlays.TryGetValue(meta.Type, out overlay) == false)
				{
					overlay = new XPathContext().ApplyBehaviors(xmlMeta, behaviors);
					overlays.Add(meta.Type, overlay);
				}
			}
		}

		#endregion

		#region Behaviors

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, object storedValue,
														  PropertyDescriptor property, bool ifExists)
		{
			if (ShouldIgnoreProperty(property) == false &&
				(storedValue == null || IsVolatileProperty(dictionaryAdapter, property)))
			{
				var result = EvaluateProperty(key, property, dictionaryAdapter);
				storedValue = ReadProperty(result, ifExists, dictionaryAdapter);
				if (storedValue != null)
				{
					dictionaryAdapter.StoreProperty(property, key, storedValue);
				}
			}
			return storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, ref object value, PropertyDescriptor property)
		{
			if (ShouldIgnoreProperty(property) == false)
			{
				EnsureOffRoot();

				if (root.CanEdit)
				{
					var result = EvaluateProperty(key, property, dictionaryAdapter);
					if (result.CanWrite)
					{
						WriteProperty(result, ref value, dictionaryAdapter);
					}
				}
			}
			return true;
		}

		object IDictionaryCreateStrategy.Create(IDictionaryAdapter adapter, Type type, IDictionary dictionary)
		{
			return Create(adapter, type, dictionary, new XPathAdapter(new XmlDocument()));
		}

		private static object Create(IDictionaryAdapter adapter, Type type, IDictionary dictionary, XPathAdapter xpathAdapter)
		{
			dictionary = dictionary ?? new Hashtable();
			var descriptor = new DictionaryDescriptor(adapter.Meta.Behaviors);
			adapter.This.Descriptor.CopyBehaviors(descriptor);
			descriptor.AddBehavior(xpathAdapter);
			return adapter.This.Factory.GetAdapter(type, dictionary, descriptor);
		}

		bool IDictionaryCopyStrategy.Copy(IDictionaryAdapter source, IDictionaryAdapter target, ref Func<PropertyDescriptor, bool> selector)
		{
			selector = selector ?? (property => XPathAdapter.IsPropertyDefined(property.PropertyName, source, this));
			return false;
		}

		public override IDictionaryBehavior Copy()
		{
			return null;
		}

		#endregion

		#region Reading

		private object ReadProperty(XPathResult result, bool ifExists, IDictionaryAdapter dictionaryAdapter)
		{
			object value;
			var propertyType = result.Type;

			if (ReadCustom(result, out value))
				return value;

			if (propertyType != typeof(string))
			{
				if (typeof(IXPathNavigable).IsAssignableFrom(propertyType))
					return ReadFragment(result);

				if (propertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(propertyType))
					return ReadCollection(result, ifExists, dictionaryAdapter);
				
				if (propertyType.IsInterface)
					return ReadComponent(result, ifExists, dictionaryAdapter);
			}

			return ReadSimple(result);
		}

		private object ReadFragment(XPathResult result)
		{
			XPathNavigator node;
			result.GetNavigator(false, true, out node);
			if (node == null) return null;
			if (result.Type == typeof(XmlElement))
			{
				var document = new XmlDocument();
				document.Load(node.ReadSubtree());
				return document.DocumentElement;
			}
			return node.Clone();
		}

		private object ReadSimple(XPathResult result)
		{
			XPathNavigator node;
			if (result.GetNavigator(false, true, out node))
			{
				if (node != null)
				{
					Type underlyingType;
					if (IsNullableType(result.Type, out underlyingType) == false)
					{
						underlyingType = result.Type;
					}

					if (underlyingType.IsEnum)
					{
						return Enum.Parse(underlyingType, node.Value);
					}
					else if (underlyingType == typeof(Guid))
					{
						return new Guid(node.Value);
					}

					try
					{
						return node.ValueAs(underlyingType);
					}
					catch (InvalidCastException)
					{
						object value;
						if (DefaultXmlSerializer.Instance.ReadObject(result, node, out value))
							return value;
					}
				}
				if (result.Result != null)
					return Convert.ChangeType(result.Result, result.Type);
			}

			return null;
		}

		private object ReadComponent(XPathResult result, bool ifExists, IDictionaryAdapter dictionaryAdapter)
		{
			XPathNavigator source;
			if (result.Property != null)
				ifExists = ifExists || dictionaryAdapter.Meta.Type.IsAssignableFrom(result.Property.PropertyType);

			if (result.GetNavigator(false, true, out source) == false || (source == null && ifExists))
			{
				return null;
			}

			XPathAdapter xpathAdapter;
			var elementType = result.Type;

			if (source != null)
			{
				if (result.XmlMeta != null)
				{
					elementType = result.XmlMeta.Type;
				}
				else
				{
					var xmlType = GetEffectiveContext(dictionaryAdapter).GetXmlType(source);
					elementType = dictionaryAdapter.GetXmlSubclass(xmlType, elementType) ?? elementType;
				}
				xpathAdapter = new XPathAdapter(source, this);
			}
			else
			{
				Func<XPathNavigator> createSource = () => result.GetNavigator(true);
				xpathAdapter = new XPathAdapter(createSource, this);
			}

			return Create(dictionaryAdapter, elementType, null, xpathAdapter);
		}

		private object ReadCollection(XPathResult result, bool ifExists, IDictionaryAdapter dictionaryAdapter)
		{
			if (ifExists && result.Result == null)
				return null;

			if (result.Type.IsArray)
				return ReadArray(result, dictionaryAdapter);

			if (result.Type.IsGenericType)
				return ReadList(result, dictionaryAdapter);

			return null;
		}

		private object ReadArray(XPathResult result, IDictionaryAdapter dictionaryAdapter)
		{
			if (result.Type == typeof(byte[]))
			{
				XPathNavigator node;
				if (result.GetNavigator(false, true, out node) && node != null)
				{
					return Convert.FromBase64String(node.InnerXml);
				}
				return null;
			}
			var itemType = result.Type.GetElementType();
			var itemNodes = result.GetNodes(itemType, type => dictionaryAdapter.GetXmlMeta(type));
			if (itemNodes == null) return null;
			var items = itemNodes.Select(node => ReadProperty(node, false, dictionaryAdapter)).ToArray();
			var array = Array.CreateInstance(itemType, items.Length);
			items.CopyTo(array, 0);
			return array;
		}

		private object ReadList(XPathResult result, IDictionaryAdapter dictionaryAdapter)
		{
			Type listType = null, initializerType = null;
			var arguments = result.Type.GetGenericArguments();
			var genericDef = result.Type.GetGenericTypeDefinition();
			var itemType = arguments[0];

			Func<Type, XmlMetadata> getXmlMeta = type => dictionaryAdapter.GetXmlMeta(type);
			var itemNodes = result.GetNodes(itemType, getXmlMeta);
			if (itemNodes == null) return null;

			if (genericDef == typeof(IEnumerable<>) || genericDef == typeof(ICollection<>) || genericDef == typeof(List<>))
			{
				listType = typeof(EditableList<>).MakeGenericType(arguments);
			}
			else if (
#if !DOTNET35
				//NOTE: what about SortedSet?
				genericDef == typeof(ISet<>) || 
#endif
				genericDef == typeof(HashSet<>))
			{
				listType = typeof(List<>).MakeGenericType(arguments);
			}
			else
			{
				listType = typeof(EditableBindingList<>).MakeGenericType(arguments);
				initializerType = typeof(BindingListInitializer<>).MakeGenericType(arguments);
			}
			
			var list = (IList)Activator.CreateInstance(listType);

			foreach (var item in itemNodes)
			{
				list.Add(ReadProperty(item, false, dictionaryAdapter));
			}

			if (
#if !DOTNET35
				//NOTE: what about SortedSet?
				genericDef == typeof(ISet<>) ||
#endif
				genericDef == typeof(HashSet<>))
			{
				return Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(arguments), list);
			}

			if (initializerType != null)
			{
				Func<object> addNew = () =>
				{
					var node = result.CreateNode(itemType, null, getXmlMeta);
					return ReadProperty(node, false, dictionaryAdapter);
				};

				Func<int, object, object> addAt = (index, item) =>
				{
					var node = result.CreateNode(itemType, item, getXmlMeta);
					WriteProperty(node, ref item, dictionaryAdapter);
					return item;
				};

				Func<int, object, object> setAt = (index, item) =>
				{
					var node = result.GetNodeAt(itemType, index);
					WriteProperty(node, ref item, dictionaryAdapter);
					return item;
				};

				Action<int> removeAt = index =>
				{
					object value = list;
					if (dictionaryAdapter.ShouldClearProperty(result.Property, value))
					{
						result.Remove(true);
						return;
					}
					result.RemoveAt(index);
				};

				Action reset = () =>
				{
					object value = list;
					if (dictionaryAdapter.ShouldClearProperty(result.Property, value))
					{
						result.Remove(true);
						return;
					}
					WriteCollection(result, ref value, dictionaryAdapter);
				};

				var initializer = (IValueInitializer)Activator.CreateInstance(
					initializerType, addAt, addNew, setAt, removeAt, reset);
				initializer.Initialize(dictionaryAdapter, list);
			}

			return list;
		}

		private bool ReadCustom(XPathResult result, out object value)
		{
			return result.ReadObject(out value);
		}

		#endregion

		#region Writing

		private void WriteProperty(XPathResult result, ref object value, IDictionaryAdapter dictionaryAdapter)
		{
			var propertyType = result.Type;
			var shouldRemove = (value == null);

			if (result.Property != null)
			{
				shouldRemove = shouldRemove || dictionaryAdapter.ShouldClearProperty(result.Property, value);
			}

			if (shouldRemove)
			{
				result.Remove(true);
				value = null;
				return;
			}

			if (WriteCustom(result, value, dictionaryAdapter))
			{
				return;
			}

			if (propertyType == typeof(string))
			{
				WriteSimple(result, value, dictionaryAdapter);
			}
			else if (typeof(IXPathNavigable).IsAssignableFrom(propertyType))
			{
				WriteFragment(result, (IXPathNavigable)value);
			}
			else if (propertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				WriteCollection(result, ref value, dictionaryAdapter);
			}
			else if (propertyType.IsInterface)
			{
				WriteComponent(result, ref value, dictionaryAdapter);
			}
			else
			{
				WriteSimple(result, value, dictionaryAdapter);
			}
		}

		private void WriteFragment(XPathResult result, IXPathNavigable value)
		{
			var node = result.GetNavigator(true);
			if (node == null)
			{
				root.AppendChild(value.CreateNavigator());
			}
			else if (value != null)
			{
				node.ReplaceSelf(value.CreateNavigator());
			}
			else
			{
				node.DeleteSelf();
			}
		}

		private void WriteSimple(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var node = result.GetNavigator(true);

			if (result.Type.IsEnum || result.Type == typeof(Guid))
			{
				node.SetTypedValue(value.ToString());
			}
			else
			{
				try
				{
					node.SetTypedValue(value);
				}
				catch (InvalidCastException)
				{
					DefaultXmlSerializer.Instance.WriteObject(result, node, value);
				}
			}
		}

		private void WriteComponent(XPathResult result, ref object value, IDictionaryAdapter dictionaryAdapter)
		{
			var source = value as IDictionaryAdapter;
			if (source != null)
			{
				var sourceAdapter = For(source);
				if (sourceAdapter != null)
				{
					var sourceRoot = sourceAdapter.Root;
					var resultNode = result.GetNavigator(false);
					if (sourceRoot != null && resultNode != null && sourceRoot.IsSamePosition(resultNode))
						return;
				}

				var node = result.RemoveChildren();
				if (result.Type != source.Meta.Type && result.OmitPolymorphism == false)
				{
					var xmlType = source.GetXmlMeta().XmlType;
					var context = GetEffectiveContext(dictionaryAdapter);
					context.SetXmlType(xmlType.TypeName, xmlType.Namespace, node);
				}

				var element = (IDictionaryAdapter)ReadComponent(result, false, dictionaryAdapter);
				source.CopyTo(element);
				value = element;
			}
		}

		private void WriteCollection(XPathResult result, ref object value, IDictionaryAdapter dictionaryAdapter)
		{
			if (value is byte[])
			{
				var node = result.GetNavigator(true);
				node.SetValue(Convert.ToBase64String((byte[])value));
				return;
			}

			result.Remove(value == null);

			if (value != null)
			{
				if (result.Type.IsArray)
				{
					WriteArray(result, value, dictionaryAdapter);
				}
				else if (result.Type.IsGenericType)
				{
					WriteList(result, value, dictionaryAdapter);
				}
				if (result.Property != null)
				{
					value = ((IDictionaryPropertyGetter)this).GetPropertyValue(dictionaryAdapter, result.Key, null, result.Property, false);
				}
			}
		}

		private void WriteArray(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var array = (Array)value;
			var itemType = array.GetType().GetElementType();

			foreach (var item in array)
			{
				var element = item;
				var node = result.CreateNode(itemType, element, type => dictionaryAdapter.GetXmlMeta(type));
				WriteProperty(node, ref element, dictionaryAdapter);
			}
		}

		private void WriteList(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var arguments = result.Type.GetGenericArguments();
			var itemType = arguments[0];

			foreach (var item in (IEnumerable)value)
			{
				var element = item;
				var node = result.CreateNode(itemType, element, type => dictionaryAdapter.GetXmlMeta(type));
				WriteProperty(node, ref element, dictionaryAdapter);
			}
		}

		private bool WriteCustom(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			return result.WriteObject(value);
		}

		#endregion

		#region Helpers

		public static XPathAdapter For(object adapter)
		{
			if (adapter == null)
			{
				throw new ArgumentNullException("adapter");
			}

			var dictionaryAdapter = adapter as IDictionaryAdapter;

			if (dictionaryAdapter != null && dictionaryAdapter.This.Descriptor != null)
			{
				XPathAdapter xpathAdapter = null;
				var getters = dictionaryAdapter.This.Descriptor.Getters;
				if (getters != null)
					xpathAdapter = getters.OfType<XPathAdapter>().SingleOrDefault();
				if (xpathAdapter != null) return xpathAdapter;
			}

			return null;
		}

		public static bool IsPropertyDefined(string propertyName, IDictionaryAdapter dictionaryAdapter)
		{
			var xpath = XPathAdapter.For(dictionaryAdapter);
			return (xpath != null) ? IsPropertyDefined(propertyName, dictionaryAdapter, xpath) : false;
		}

		public static bool IsPropertyDefined(string propertyName, IDictionaryAdapter dictionaryAdapter, XPathAdapter xpath)
		{
			var key = dictionaryAdapter.GetKey(propertyName);
			if (key == null) return false;
			var property = dictionaryAdapter.This.Properties[propertyName];
			var result = xpath.EvaluateProperty(key, property, dictionaryAdapter);
			return result.GetNavigator(false) != null;
		}

		private XPathContext GetEffectiveContext(IDictionaryAdapter dictionaryAdapter)
		{
			if (overlays != null)
			{
				XPathContext context;
				if (overlays.TryGetValue(dictionaryAdapter.Meta.Type, out context))
					return context;
			}
			return Context;
		}

		private XPathResult EvaluateProperty(string key, PropertyDescriptor property, IDictionaryAdapter dictionaryAdapter)
		{
			object result;
			XPathExpression xpath = null;
			object matchingBehavior = null;
			Func<XPathNavigator> create = null;

			var context = GetEffectiveContext(dictionaryAdapter);
			var xmlMeta = dictionaryAdapter.GetXmlMeta(property.Property.DeclaringType);
			var keyContext = context.CreateChild(xmlMeta, property.Behaviors);

			foreach (var behavior in property.Behaviors)
			{
				string name = key, ns = null;
				Func<XPathNavigator> matchingCreate = null;

				if (behavior is XmlElementAttribute)
				{
					xpath = XPathElement;
					var attrib = (XmlElementAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.ElementName) == false)
						name = attrib.ElementName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.AppendElement(name, ns, root.Clone());
				}
				else if (behavior is XmlAttributeAttribute)
				{
					xpath = XPathAttribute;
					var attrib = (XmlAttributeAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.AttributeName) == false)
						name = attrib.AttributeName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.CreateAttribute(name, ns, root.Clone());
				}
				else if (behavior is XmlArrayAttribute)
				{
					xpath = XPathElement;
					var attrib = (XmlArrayAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.ElementName) == false)
						name = attrib.ElementName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.AppendElement(name, ns, root.Clone());
				}
				else if (behavior is XPathAttribute)
				{
					var attrib = (XPathAttribute)behavior;
					xpath = attrib.CompiledExpression;
				}
				else
				{
					continue;
				}

				if (xpath != null)
				{
					keyContext.Arguments.Clear();
					keyContext.Arguments.AddParam("key", "", name);
					keyContext.Arguments.AddParam("ns", "", ns ?? XPathContext.IgnoreNamespace);
					if (keyContext.Evaluate(xpath, Root, out result))
					{
						create = matchingCreate ?? create;
						return new XPathResult(property, key, result, keyContext, behavior, create);
					}
				}

				matchingBehavior = matchingBehavior ?? behavior;
				create = create ?? matchingCreate;
			}

			if (xpath != null)
				return new XPathResult(property, key, null, keyContext, matchingBehavior, create);

			keyContext.Arguments.Clear();
			keyContext.Arguments.AddParam("key", "", key);
			keyContext.Arguments.AddParam("ns", "", XPathContext.IgnoreNamespace);
			create = create ?? (() => keyContext.AppendElement(key, null, root));
			keyContext.Evaluate(XPathElementOrAttribute, Root, out result);
			return new XPathResult(property, key, result, keyContext, null, create);
		}

		private static bool ShouldIgnoreProperty(PropertyDescriptor property)
		{
			return property.Behaviors.Any(behavior => behavior is XmlIgnoreAttribute);
		}

		private static bool IsVolatileProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property)
		{
			return dictionaryAdapter.Meta.Behaviors.Union(property.Behaviors).Any(behavior => behavior is VolatileAttribute);
		}

		private static bool IsNullableType(Type type, out Type underlyingType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				underlyingType = type.GetGenericArguments()[0];
				return true;
			}
			underlyingType = null;
			return false;
		}

		private XPathNavigator EnsureOffRoot()
        {
			if (root == null && createRoot != null)
			{
				root = createRoot().Clone();
			}

            if (root != null && MoveOffRoot(root, XPathNodeType.Element) == false)
            {
				string elementName;
				var namespaceUri = string.Empty;
				var xmlRoot = rootXmlMeta.XmlRoot;
				if (xmlRoot != null)
				{
					elementName = xmlRoot.ElementName;
					namespaceUri = xmlRoot.Namespace;
				}
				else
				{
					elementName = rootXmlMeta.XmlType.TypeName;
				}
				root = Context.AppendElement(elementName, namespaceUri, root);
				Context.AddStandardNamespaces(root);
            }

			return root;
        }

		private static bool MoveOffRoot(XPathNavigator source, XPathNodeType to)
		{
			if (source.NodeType == XPathNodeType.Root)
			{
				return source.MoveToChild(to);
			}
			return true;
		}

		#endregion

		#region XPath

		private static readonly XPathExpression XPathElement = XPathExpression.Compile("*[castle-da:match($key,$ns)]");
		private static readonly XPathExpression XPathAttribute = XPathExpression.Compile("@*[castle-da:match($key,$ns)]");
		private static readonly XPathExpression XPathElementOrAttribute = XPathExpression.Compile("(*|@*)[castle-da:match($key,$ns)]");

		#endregion
	}
#endif
}
