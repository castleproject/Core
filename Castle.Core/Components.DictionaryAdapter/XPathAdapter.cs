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
								IDictionaryCreateStrategy
	{
		private readonly Func<XPathNavigator> createRoot;
		private XPathNavigator root;
		private XmlMetadata xmlMeta;

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
			Context = parent.Context.CreateChild();
			root = source.Clone();
		}

		protected XPathAdapter(Func<XPathNavigator> createSource, XPathAdapter parent)
		{
			Parent = parent;
			Context = parent.Context.CreateChild();
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

			dictionaryAdapter.This.CreateStrategy = this;

			Context.ApplyBehaviors(behaviors);
			xmlMeta = dictionaryAdapter.GetXmlMeta();

			if (string.IsNullOrEmpty(xmlMeta.XmlType.Namespace) == false)
			{
				Context.AddNamespace(string.Empty, xmlMeta.XmlType.Namespace);
			}

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

		#endregion

		#region Behaviors

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, 
														  object storedValue, PropertyDescriptor property, bool ifExists)
		{
			if (ShouldIgnoreProperty(property))
			{
				return storedValue;
			}

			var cached = dictionaryAdapter.This.ExtendedProperties[property.PropertyName];
			if (cached != null) return cached;

			var result = EvaluateProperty(key, property, dictionaryAdapter);
			return ReadProperty(result, ifExists, dictionaryAdapter);
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, 
														ref object value, PropertyDescriptor property)
		{
			if (ShouldIgnoreProperty(property))
			{
				return true;
			}

			EnsureOffRoot();

			if (root.CanEdit)
			{
				var result = EvaluateProperty(key, property, dictionaryAdapter);
				if (result.CanWrite)
				{
					WriteProperty(result, value, dictionaryAdapter);
				}
			}

			return false;
		}

		object IDictionaryCreateStrategy.Create(IDictionaryAdapter adapter, Type type, IDictionary dictionary)
		{
			dictionary = dictionary ?? new Hashtable();
			var descriptor = new DictionaryDescriptor();
			adapter.This.Descriptor.CopyBehaviors(descriptor, b => b is XPathAdapter == false);
			descriptor.AddBehavior(XPathBehavior.Instance).AddBehavior(new XPathAdapter(new XmlDocument()));
			return adapter.This.Factory.GetAdapter(type, dictionary, descriptor);
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
			var node = result.GetNavigator(false);
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
			var node = result.GetNavigator(false);
			if (node != null)
			{
				if (result.Type.IsEnum)
				{
					return Enum.Parse(result.Type, node.Value);
				}
				else if (result.Type == typeof(Guid))
				{
					return new Guid(node.Value);
				}

				try
				{
					return node.ValueAs(result.Type);
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

			return null;
		}

		private object ReadComponent(XPathResult result, bool ifExists, IDictionaryAdapter dictionaryAdapter)
		{
			XPathAdapter adapter;
			var source = result.GetNavigator(false);
			if (source == null && ifExists) return null;
		
			var elementType = result.Type;
			if (source != null)
			{
				if (result.XmlMeta != null)
				{
					elementType = result.XmlMeta.Type;
				}
				else
				{
					var xmlType = Context.GetXmlType(source);
					elementType = dictionaryAdapter.GetXmlSubclass(xmlType, elementType) ?? elementType;
				}
				adapter = new XPathAdapter(source, this);
			}
			else
			{
				Func<XPathNavigator> createSource = () => result.GetNavigator(true);
				adapter = new XPathAdapter(createSource, this);
			}

			var component = dictionaryAdapter.This.Factory.GetAdapter(elementType, new Hashtable(), 
				new DictionaryDescriptor().AddBehavior(XPathBehavior.Instance, adapter));

			if (result.Property != null)
				dictionaryAdapter.This.ExtendedProperties[result.Property.PropertyName] = component;

			return component;
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
			var itemType = result.Type.GetElementType();
			var items = result.GetNodes(itemType, type => dictionaryAdapter.GetXmlMeta(type))
				.Select(node => ReadProperty(node, false, dictionaryAdapter)).ToArray();
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

			if (genericDef == typeof(IEnumerable<>) || genericDef == typeof(ICollection<>) ||
				genericDef == typeof(IList<>) || genericDef == typeof(List<>))
			{
				listType = typeof(EditableList<>).MakeGenericType(arguments);
			}
			else if (genericDef == typeof(ISet<>) || genericDef == typeof(HashSet<>))
			{
				listType = typeof(List<>).MakeGenericType(arguments);
			}
			else
			{
				listType = typeof(EditableBindingList<>).MakeGenericType(arguments);
				initializerType = typeof(BindingListInitializer<>).MakeGenericType(arguments);
			}
			
			var list = (IList)Activator.CreateInstance(listType);
			Func<Type, XmlMetadata> getXmlMeta = type => dictionaryAdapter.GetXmlMeta(type);

			foreach (var item in result.GetNodes(itemType, getXmlMeta))
			{
				list.Add(ReadProperty(item, false, dictionaryAdapter));
			}

			if (genericDef == typeof(ISet<>) || genericDef == typeof(HashSet<>))
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

				Action<int, object> addAt = (index, item) =>
				{
					var node = result.CreateNode(itemType, item, getXmlMeta);
					WriteProperty(node, item, dictionaryAdapter);
				};

				Action<int, object> setAt = (index, item) =>
				{
					var node = result.GetNodeAt(itemType, index);
					WriteProperty(node, item, dictionaryAdapter);
				};

				Action<int> removeAt = index => result.RemoveAt(index);

				var initializer = (IValueInitializer)Activator.CreateInstance(
					initializerType, addAt, addNew, setAt, removeAt);
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

		private void WriteProperty(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var propertyType = result.Type;

			if (WriteCustom(result, value, dictionaryAdapter))
				return;

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
				WriteCollection(result, value, dictionaryAdapter);
			}
			else if (propertyType.IsInterface)
			{
				WriteComponent(result, value, dictionaryAdapter);
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
			if (value == null)
			{
				result.Remove();
				if (result.Property != null)
					dictionaryAdapter.This.ExtendedProperties.Remove(result.Property.PropertyName);
				return;
			}

			var node = result.GetNavigator(true);

			if (result.Type.IsEnum)
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
					if (DefaultXmlSerializer.Instance.WriteObject(result, node, value) && result.Property != null)
					{
						dictionaryAdapter.This.ExtendedProperties[result.Property.PropertyName] = value;
					}
				}
			}
		}

		private void WriteComponent(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			if (result.Property != null)
			{
				dictionaryAdapter.This.ExtendedProperties.Remove(result.Property.PropertyName);
			}

			if (value == null)
			{
				result.Remove();
				return;
			}

			var source = value as IDictionaryAdapter;
			if (source != null)
			{
				var node = result.RemoveChildren();
				if (result.Type != source.Meta.Type && result.OmitPolymorphism == false)
				{
					var xmlType = source.GetXmlMeta().XmlType;
					Context.SetXmlType(xmlType.TypeName, xmlType.Namespace, node);
				}
				var element = (IDictionaryAdapter)ReadComponent(result, false, dictionaryAdapter);
				source.CopyTo(element);
			}
		}

		private void WriteCollection(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			result.Remove();

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
			}
		}

		private void WriteArray(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var array = (Array)value;
			var itemType = array.GetType().GetElementType();

			foreach (var item in array)
			{
				var node = result.CreateNode(itemType, item, type => dictionaryAdapter.GetXmlMeta(type));
				WriteProperty(node, item, dictionaryAdapter);
			}
		}

		private void WriteList(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			var arguments = result.Type.GetGenericArguments();
			var itemType = arguments[0];

			foreach (var item in (IEnumerable)value)
			{
				var node = result.CreateNode(itemType, item, type => dictionaryAdapter.GetXmlMeta(type));
				WriteProperty(node, item, dictionaryAdapter);
			}
		}

		private bool WriteCustom(XPathResult result, object value, IDictionaryAdapter dictionaryAdapter)
		{
			if (result.WriteObject(value))
			{
				if (result.Property != null)
				{
					dictionaryAdapter.This.ExtendedProperties[result.Property.PropertyName] = value;
				}
				return true;
			}
			return false;
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

			if (dictionaryAdapter != null)
			{
				XPathAdapter xpathAdapter = null;
				var getters = dictionaryAdapter.This.Descriptor.Getters;
				if (getters != null)
					xpathAdapter = getters.OfType<XPathAdapter>().SingleOrDefault();
				if (xpathAdapter != null) return xpathAdapter;
			}

			return null;
		}

		private XPathResult EvaluateProperty(string key, PropertyDescriptor property, IDictionaryAdapter dictionaryAdapter)
		{
			object result;
			XPathExpression xpath = null;
			object matchingBehavior = null;
			Func<XPathNavigator> create = null;

			object xmlType = dictionaryAdapter.GetXmlMeta(property.Property.DeclaringType).XmlType;
			var keyContext = Context.CreateChild(Enumerable.Repeat(xmlType, 1).Union(property.Behaviors));

			foreach (var behavior in property.Behaviors)
			{
				string name = key, ns = null;
				Func<XPathNavigator> matchingCreate = null;

				if (behavior is XmlElementAttribute)
				{
					xpath = XPathElement;
					var node = root.Clone();
					var attrib = (XmlElementAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.ElementName) == false)
						name = attrib.ElementName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.AppendElement(name, ns, node);
				}
				else if (behavior is XmlAttributeAttribute)
				{
					xpath = XPathAttribute;
					var node = root.Clone();
					var attrib = (XmlAttributeAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.AttributeName) == false)
						name = attrib.AttributeName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.CreateAttribute(name, ns, node);
				}
				else if (behavior is XmlArrayAttribute)
				{
					xpath = XPathElement;
					var node = root.Clone();
					var attrib = (XmlArrayAttribute)behavior;
					if (string.IsNullOrEmpty(attrib.ElementName) == false)
						name = attrib.ElementName;
					if (string.IsNullOrEmpty(attrib.Namespace) == false)
						ns = attrib.Namespace;
					matchingCreate = () => keyContext.AppendElement(name, ns, node);
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
					if (keyContext.Evaluate(xpath, root, out result))
					{
						create = matchingCreate ?? create;
						return new XPathResult(property, result, keyContext, behavior, create);
					}
				}

				matchingBehavior = matchingBehavior ?? behavior;
				create = create ?? matchingCreate;
			}

			if (xpath != null)
				return new XPathResult(property, null, keyContext, matchingBehavior, create);

			keyContext.Arguments.Clear();
			keyContext.Arguments.AddParam("key", "", key);
			keyContext.Arguments.AddParam("ns", "", XPathContext.IgnoreNamespace);
			create = create ?? (() => keyContext.AppendElement(key, null, root));
			keyContext.Evaluate(XPathElementOrAttribute, Root, out result);
			return new XPathResult(property, result, keyContext, null, create);
		}

		private static bool ShouldIgnoreProperty(PropertyDescriptor property)
		{
			return property.Behaviors.Any(behavior => behavior is XmlIgnoreAttribute);
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
				string namespaceUri = "";
				var xmlRoot = xmlMeta.XmlRoot;
				if (xmlRoot != null)
				{
					elementName = xmlRoot.ElementName;
					namespaceUri = xmlRoot.Namespace;
				}
				else
				{
					elementName = xmlMeta.XmlType.TypeName;
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
