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
	using System.Reflection;
	using System.Linq;
	using System.Xml;
	using System.Xml.XPath;
	using System.Text;

	public abstract class XmlPropertyAccessor
	{
		private readonly Type type;

		protected XmlPropertyAccessor(Type type)
		{
			this.type = UnwrapNullable(type);
		}

		public Type PropertyType
		{
			get { return type; }
		}

		protected abstract Iterator<XPathNavigator> SelectPropertyNode   (XPathNavigator node, bool create);
		protected abstract Iterator<XPathNavigator> SelectCollectionNode (XPathNavigator node, bool create);
		protected abstract Iterator<XPathNavigator> SelectCollectionItems(XPathNavigator node, bool create);

		public virtual object GetPropertyValue(XPathNavigator node, IDictionaryAdapter da, bool ifExists)
		{
			if (type == typeof(string) || type == typeof(byte[]))
				return GetSimpleProperty(node);
			if (type.IsArray)
				return GetArrayProperty(node, da, ifExists);
			if (type.IsInterface)
				return GetComponentProperty(node, da, ifExists);
			if (typeof(IEnumerable).IsAssignableFrom(type))
				return GetCollectionProperty(node, da, ifExists);

			return GetSimpleProperty(node);
		}

		private object GetSimpleProperty(XPathNavigator node)
		{
			var iterator = SelectPropertyNode(node, false);
			return iterator.MoveNext()
				? GetTypedValue(iterator.Current, type)
				: null;
		}

		private object GetTypedValue(XPathNavigator node, Type type)
		{
			if (type == typeof(string))
				return node.Value;
			if (type.IsEnum)
				return Enum.Parse(type, node.Value, true);
			if (type == typeof(Guid))
				return Guid.Parse(node.Value);

			try
			{
				return node.ValueAs(type);
			}
			catch (InvalidCastException)
			{
				// TODO: Use this code
				//object value;
				//if (DefaultXmlSerializer.Instance.ReadObject(result, node, out value))
				//    return value;
				throw;
			}
		}

		private object GetComponentProperty(XPathNavigator node, IDictionaryAdapter da, bool ifExists)
		{
			var iterator = SelectPropertyNode(node, !ifExists);
			if (!iterator.MoveNext() && ifExists)
				return null;

			var adapter = new XmlAdapter(iterator);
			return CreateDictionaryAdapter(da, type, null, adapter);
		}

		private object GetArrayProperty(XPathNavigator node, IDictionaryAdapter da, bool ifExists)
		{
			var iterator = SelectCollectionNode(node, false);
			if (!iterator.MoveNext())
				return null;

			iterator = SelectCollectionItems(iterator.Current, false);
			var items = (IList) new List<object>();
			var itemType = type.GetElementType();
			var accessor = new XmlSelfAccessor(itemType);

			while (iterator.MoveNext())
				items.Add(accessor.GetPropertyValue(iterator.Current, da, ifExists));

			var array = Array.CreateInstance(itemType, items.Count);
			items.CopyTo(array, 0);
			return array;
		}

		private object GetCollectionProperty(XPathNavigator node, IDictionaryAdapter da, bool ifExists)
		{
			var iterator = SelectCollectionNode(node, !ifExists);
			if (!iterator.MoveNext())
				return null;

			iterator = SelectCollectionItems(iterator.Current, true);

			var args = type.GetGenericArguments();
			var def  = type.GetGenericTypeDefinition();
			var itemType = args[0];

			if (def == typeof(IList<>))
				return GetListProperty(itemType);

			throw Error.NotSupported();
		}

		private object GetListProperty(Type itemType)
		{
			var listType = typeof(EditableList<>).MakeGenericType(itemType);
			var list = (IList) Activator.CreateInstance(listType);

			throw Error.NotSupported();
		}

		private static object CreateDictionaryAdapter(IDictionaryAdapter source, Type type, IDictionary dictionary, XmlAdapter xmlAdapter)
		{
			dictionary = dictionary ?? new Hashtable();

			var descriptor = new DictionaryDescriptor(source.Meta.Behaviors);
			source.This.Descriptor.CopyBehaviors(descriptor);
			descriptor.AddBehavior(xmlAdapter);

			return source.This.Factory.GetAdapter(type, dictionary, descriptor);
		}

		public virtual void SetPropertyValue(XPathNavigator node, object value)
		{
			if (type == typeof(string) || type.IsEnum || type == typeof(Guid))
				SetStringProperty(node, value);
			else if (type == typeof(byte[]))
				SetSimpleProperty(node, value);
			else if (type.IsArray)
				SetArrayProperty(node, value);
			else
				SetSimpleProperty(node, value);
		}

		private void SetStringProperty(XPathNavigator node, object value)
		{
			EnsurePropertyNode(node).SetValue(value.ToString());
		}

		private void SetSimpleProperty(XPathNavigator node, object value)
		{
			EnsurePropertyNode(node).SetTypedValue(value);
		}

		private void SetArrayProperty(XPathNavigator node, object value)
		{
			var iterator = SelectCollectionNode(node, false);
			if (!iterator.MoveNext())
				iterator.Create();

			iterator = SelectCollectionItems(iterator.Current, true);
			var array = (Array) value;
			var itemType = type.GetElementType();
			var accessor = new XmlSelfAccessor(itemType);

			foreach (var item in array)
			{
				if (!iterator.MoveNext())
					iterator.Create();

				accessor.SetPropertyValue(iterator.Current, item);
			}
		}

		internal XPathNavigator EnsurePropertyNode(XPathNavigator node)
		{
			var iterator = SelectPropertyNode(node, true);

			if (!iterator.MoveNext())
				iterator.Create();

			return iterator.Current;
		}

		private static Type UnwrapNullable(Type type)
		{
			return type.IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(Nullable<>)
				? type.GetGenericArguments()[0]
				: type;
		}

		protected string GetLocalName(Type itemType)
		{
			string name;
			if (KnownSimpleTypes.TryGetValue(itemType, out name))
				return name;

			name = itemType.Name;

			var first  = name[0];
			if (!char.IsUpper(first))
				return name;

			var length = name.Length;
			return new StringBuilder(length)
				.Append(char.ToLower(first))
				.Append(name, 1, length - 1)
				.ToString();
		}

		private static readonly Dictionary<Type, string>
			KnownSimpleTypes = new Dictionary<Type,string>
		{
			{ typeof(bool),           "bool"           },
			{ typeof(string),         "string"         },
			{ typeof(sbyte),          "sbyte"          },
			{ typeof(byte),           "byte"           },
			{ typeof(short),          "short"          },
			{ typeof(ushort),         "ushort"         },
			{ typeof(int),            "int"            },
			{ typeof(uint),           "uint"           },
			{ typeof(long),           "long"           },
			{ typeof(ulong),          "ulong"          },
			{ typeof(float),          "float"          },
			{ typeof(double),         "double"         },
			{ typeof(decimal),        "decimal"        },
			{ typeof(Guid),           "guid"           },
			{ typeof(DateTime),       "dateTime"       },
			{ typeof(DateTimeOffset), "dateTimeOffset" },
			{ typeof(TimeSpan),       "timeSpan"       },
			{ typeof(byte[]),         "byteArray"      },
		};
	}
}
#endif
