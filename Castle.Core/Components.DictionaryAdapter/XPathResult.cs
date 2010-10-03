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
#if !SILVERLIGHT
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XPathResult
	{
		public readonly bool CanWrite;
		public readonly PropertyDescriptor Property;
		private readonly object matchingBehavior;
		private readonly Func<XPathNavigator> create;

		public XPathResult(PropertyDescriptor property, object result,
						   XPathContext context, object matchingBehavior)
			: this(property, result, context, matchingBehavior, null)
		{
		}

		public XPathResult(Type type, object result, XPathContext context, object matchingBehavior)
			: this(null, result, context, matchingBehavior, null)
		{
			Type = type;
		}

		public XPathResult(PropertyDescriptor property, object result, XPathContext context,
						   object matchingBehavior, Func<XPathNavigator> create)
		{
			Result = result;
			Property = property;
			Type = (property != null) ? Property.PropertyType : null;
			Context = context;
			this.create = create;
			this.matchingBehavior = matchingBehavior;
			CanWrite = (create != null || result is XPathNavigator);
		}

		public bool IsContainer
		{
			get { return matchingBehavior == null || matchingBehavior is XmlArrayAttribute; }
		}

		public Type Type { get; private set; }

		public object Result { get; private set; }

		public XPathContext Context { get; private set; }

		public XPathNavigator Container { get; private set; }

		public XmlMetadata XmlMeta { get; private set; }

		public bool OmitPolymorphism { get; private set; }

		public XPathNavigator GetNavigator(bool demand)
		{
			if (Result is XPathNavigator)
			{
				return (XPathNavigator)Result;
			}
			else if (Result is XPathNodeIterator)
			{
				return ((XPathNodeIterator)Result).Cast<XPathNavigator>().FirstOrDefault();
			}
			if (demand && create != null)
			{
 				var result = create();
				Result = result;
				return result;
			}	 
			return null;
		}

		public XPathResult GetNodeAt(Type type, int index)
		{
			var node = Container;
			if (IsContainer)
			{
				if (node != null)
				{
					node = Container.SelectSingleNode(String.Format("*[position()={0}]", index + 1));
				}
			}
			else if (Result is XPathNodeIterator)
			{
				var nodes = ((XPathNodeIterator)Result).Cast<XPathNavigator>().ToArray();
				node = nodes[index];
			}
			return new XPathResult(type, node, Context, matchingBehavior);
		}

		public IEnumerable<XPathResult> GetNodes(Type type, Func<Type, XmlMetadata> getXmlMeta)
		{
			Container = null;
			var nodes = Result as XPathNodeIterator;
			var results = Enumerable.Empty<XPathNavigator>();

			if (nodes == null)
			{
				Container = Result as XPathNavigator;
				if (IsContainer && Container != null)
				{
					if (Context.ListItemMeta != null)
					{
						return Context.ListItemMeta.SelectMany(item =>
						{
							string name, namespaceUri;
							var xmlMeta = GetItemQualifedName(type, item, getXmlMeta, out name, out namespaceUri);
							return Container.SelectChildren(name, namespaceUri).Cast<XPathNavigator>()
								.Select(r => new XPathResult(item.Type ?? type, r, Context, matchingBehavior)
								{
									XmlMeta = xmlMeta
								});
						}).OrderBy(r => (XPathNavigator)r.Result, XPathPositionComparer.Instance);
					}
					else
					{
						results = Container.SelectChildren(XPathNodeType.Element).Cast<XPathNavigator>();
					}
				}
			}
			else if (IsContainer == false)
			{
				results = nodes.Cast<XPathNavigator>();
			}
			else
			{
				var parents = nodes.Cast<XPathNavigator>().ToList();
				Container = parents.FirstOrDefault();

				if (Context.ListItemMeta != null)
				{
					return Context.ListItemMeta.SelectMany(item =>
					{
						string name, namespaceUri;
						var xmlMeta = GetItemQualifedName(type, item, getXmlMeta, out name, out namespaceUri);
						return parents.SelectMany(p => p.SelectChildren(name, namespaceUri).Cast<XPathNavigator>())
							.Select(r => new XPathResult(item.Type ?? type, r, Context, matchingBehavior)
							{
								XmlMeta = xmlMeta
							});
					}).OrderBy(r => (XPathNavigator)r.Result, XPathPositionComparer.Instance);
				}
				else
				{
					results = parents.SelectMany(p => p.SelectChildren(XPathNodeType.Element)
						.Cast<XPathNavigator>());
				}
			}
			return results.Select(r => new XPathResult(type, r, Context, matchingBehavior));
		}

		public bool ReadObject(out object value)
		{
			var node = GetNavigator(false);
			foreach (var serializer in Context.Serializers)
			{
				if (serializer.ReadObject(this, node, out value))
					return true;
			}
			value = null;
			return false;
		}

		public bool WriteObject(object value)
		{
			var node = GetNavigator(true);
			foreach (var serializer in Context.Serializers)
			{
				if (serializer.WriteObject(this, node, value))
					return true;
			}
			return false;
		}

		private XmlMetadata GetItemQualifedName(Type type, XmlArrayItemAttribute item, Func<Type, XmlMetadata> getXmlMeta,
												out string name, out string namespaceUri)
		{
			name = item.ElementName;
			namespaceUri = item.Namespace;
			type = item.Type ?? type;
			var xmlMeta = getXmlMeta(type);
			
			if (string.IsNullOrEmpty(name))
			{
				if (xmlMeta != null)
				{
					name = xmlMeta.XmlType.TypeName;
				}
				else
				{
					name = GetDataType(type);
				}
				namespaceUri = null;
			}
			namespaceUri = Context.GetEffectiveNamespace(namespaceUri);
			return xmlMeta;
		}

		public XPathResult CreateNode(Type type, object value, Func<Type, XmlMetadata> getXmlMeta)
		{
			string name = null;
			string namespaceUri = null;
			string typeNamespaceUri = null;
			bool omitPolymorphism = false;
			Type baseType = type;
			var xmlMeta = getXmlMeta(type);

			if (xmlMeta != null)
			{
				name = xmlMeta.XmlType.TypeName;
				typeNamespaceUri = xmlMeta.XmlType.Namespace;
			}

			if (value != null)
			{
				if (value is IDictionaryAdapter)
				{
					type = ((IDictionaryAdapter)value).Meta.Type;
				}
				else
				{
					type = value.GetType();
				}
			}

			if (xmlMeta == null)
			{
				name = GetDataType(type);
			}

			if (Context.ListItemMeta != null)
			{
				var actualType = type;
				var listItem = Context.ListItemMeta.FirstOrDefault(li => (li.Type ?? baseType) == actualType);

				if (listItem != null)
				{
					type = listItem.Type ?? baseType;

					if (string.IsNullOrEmpty(listItem.ElementName))
					{
						if (listItem.Type != null)
						{
							xmlMeta = getXmlMeta(listItem.Type);

							if (xmlMeta != null)
							{
								name = xmlMeta.XmlType.TypeName;
								typeNamespaceUri = xmlMeta.XmlType.Namespace;
							}
							else
							{
								name = GetDataType(listItem.Type);
							}
						}
					}
					else
					{
						name = listItem.ElementName;
						namespaceUri = listItem.Namespace;
					}
					omitPolymorphism = true;
				}
			}

			XPathNavigator item = null;

			if (IsContainer)
			{
				if (Container == null && create != null)
				{
					Container = create();
				}

				if (Container != null)
				{
					item = Context.AppendElement(name, namespaceUri, Container);
				}
			}
			else if (create != null)
			{
				item = create();
			}

			if (string.IsNullOrEmpty(typeNamespaceUri) == false)
			{
				Context.CreateNamespace(null, typeNamespaceUri, item);
			}

			return new XPathResult(type, item, Context, matchingBehavior)
			{
				XmlMeta = xmlMeta,
				OmitPolymorphism = omitPolymorphism
			};
		}

		public void RemoveAt(int index)
		{
			GetNodeAt(null, index).Remove();
		}

		public void Remove()
		{
			if (Result is XPathNavigator)
			{
				((XPathNavigator)Result).DeleteSelf();
			}
			else if (Result is XPathNodeIterator)
			{
				var nodes = ((XPathNodeIterator)Result).Cast<XPathNavigator>().ToArray();
				for (int i = 0; i < nodes.Length; ++i)
				{
					nodes[i].DeleteSelf();
				}
			}
			Result = null;
		}

		public XPathNavigator RemoveChildren()
		{
			var node = GetNavigator(true);
			if (node != null)
			{
				var children = node.SelectChildren(XPathNodeType.All).Cast<XPathNavigator>();
				foreach (var child in children.ToArray()) child.DeleteSelf();
			}
			return node;
		}

		#region Xml Primitive Data Types

		private static string GetDataType(Type type)
		{
			string dataType;
			if (XmlDataTypes.TryGetValue(type, out dataType) == false)
			{
				dataType = type.Name.ToLower();
			}
			return dataType;
		}

		private static readonly Dictionary<Type, string> XmlDataTypes = new Dictionary<Type, string>()
		{
			{ typeof(int), "int" },
			{ typeof(long), "long" },
			{ typeof(short), "short" },
			{ typeof(float), "float" },
			{ typeof(double), "double" },
			{ typeof(bool), "boolean" },
			{ typeof(DateTime), "dateTime" },
			{ typeof(byte), "byte" },
			{ typeof(uint), "uint" },
			{ typeof(ulong), "ulong" },
			{ typeof(ushort), "ushort" }
		};

		#endregion
	}

	#region Class: XPathPositionComparer

	class XPathPositionComparer : IComparer<XPathNavigator>
	{
		public static readonly XPathPositionComparer Instance = new XPathPositionComparer();

		private XPathPositionComparer()
		{
		}

		public int Compare(XPathNavigator x, XPathNavigator y)
		{
			switch (x.ComparePosition(y))
			{
				case XmlNodeOrder.Before:
					return -1;
				case XmlNodeOrder.After:
					return 1;
				default:
					return 0;
			}
		}
	}

	#endregion
#endif
}