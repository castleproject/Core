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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
#if !SL3
	using System.Xml.XPath;
#endif

	public static class XmlExtensions
	{
		public static string GetAttribute(this XmlNode node, string localName, string namespaceUri)
		{
			var attribute = node.Attributes[localName, namespaceUri];
			if (attribute == null)
				return null;

			var value = attribute.Value;
			if (string.IsNullOrEmpty(value))
				return null;

			return value;
		}

		public static void SetAttribute(this XmlNode node, string localName, string namespaceUri, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				node.Attributes.RemoveNamedItem(localName, namespaceUri);
			}
			else
			{
				var attribute = node.Attributes[localName, namespaceUri];
				if (attribute == null)
				{
					attribute = node.OwnerDocument.CreateAttribute(null, localName, namespaceUri);
					node.Attributes.Append(attribute);
				}
				attribute.Value = value;
			}
		}

		public static bool HasAttribute(this XmlNode node, string localName, string namespaceUri, string value)
		{
			var attribute = node.Attributes[localName, namespaceUri];
			return attribute != null
				&& attribute.Value == value;
		}

		public static bool IsNamespace(this XmlAttribute attribute)
		{
			return attribute.Prefix == XmlnsPrefix ||
			(
				string.IsNullOrEmpty(attribute.Prefix) &&
				attribute.LocalName == XmlnsPrefix
			);
		}

#if !SL3
#endif

		public static XmlMetadata GetXmlMeta(this DictionaryAdapterMeta meta)
		{
			return (XmlMetadata) meta.ExtendedProperties[XmlMetaKey];
		}

		public static void SetXmlMeta(this DictionaryAdapterMeta meta, XmlMetadata xmlMeta)
		{
			meta.ExtendedProperties[XmlMetaKey] = xmlMeta;
		}

		public static bool HasXmlMeta(this DictionaryAdapterMeta meta)
		{
			return meta.ExtendedProperties.Contains(XmlMetaKey);
		}

		public static XmlAccessor GetAccessor(this PropertyDescriptor property)
		{
		    return (XmlAccessor) property.ExtendedProperties[XmlAccessorKey];
		}

		public static void SetAccessor(this PropertyDescriptor property, XmlAccessor accessor)
		{
		    property.ExtendedProperties[XmlAccessorKey] = accessor;
		}

		public static bool HasAccessor(this PropertyDescriptor property)
		{
			return property.ExtendedProperties.Contains(XmlAccessorKey);
		}

		public static string GetLocalName(this Type type)
		{
			string name;
			if (XsdTypes.TryGetValue(type, out name))
				return name;

			name = type.Name;
			return type.IsInterface && name.IsInterfaceName()
				? name.Substring(1)
				: name;
		}

		internal static bool IsSimpleType(this Type type)
		{
			return XsdTypes.ContainsKey(type);
		}

		internal static bool IsCustomSerializable(this Type type)
		{
			return typeof(IXmlSerializable).IsAssignableFrom(type);
		}

		private static bool IsInterfaceName(this string name)
		{
			return name.Length > 1
				&& name[0] == 'I'
				&& char.IsUpper(name, 1);
		}

		private const string
			XmlAccessorKey = "XmlAccessor",
			XmlMetaKey     = "XmlMeta";

		private static readonly StringComparer
			Comparer = StringComparer.OrdinalIgnoreCase;

		public const string
			XmlnsPrefix       = "xmlns",
			XmlnsNamespaceUri = "http://www.w3.org/2000/xmlns/",
			WsdlPrefix        = "wsdl", // For Guid
			WsdlNamespaceUri  = "http://microsoft.com/wsdl/types/"; // For Guid

		internal static readonly Dictionary<Type, string>
			XsdTypes = new Dictionary<Type,string>
		{
			{ typeof(object),           "anyType"       },
			{ typeof(string),           "string"        },
			{ typeof(bool),             "boolean"       },
			{ typeof(sbyte),            "byte"          },
			{ typeof(byte),             "unsignedByte"  },
			{ typeof(short),            "short"         },
			{ typeof(ushort),           "unsignedShort" },
			{ typeof(int),              "int"           },
			{ typeof(uint),             "unsignedInt"   },
			{ typeof(long),             "long"          },
			{ typeof(ulong),            "unsignedLong"  },
			{ typeof(float),            "float"         },
			{ typeof(double),           "double"        },
			{ typeof(decimal),          "decimal"       },
			{ typeof(Guid),             "guid"          },
			{ typeof(DateTime),         "dateTime"      },
			{ typeof(TimeSpan),         "duration"      },
			{ typeof(byte[]),           "base64Binary"  },
			{ typeof(Uri),              "anyURI"        },
			{ typeof(XmlQualifiedName), "QName"         }
		};
	}
}
#endif
