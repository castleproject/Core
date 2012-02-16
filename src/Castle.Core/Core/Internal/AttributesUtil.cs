// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	///   Helper class for retrieving attributes.
	/// </summary>
	public static class AttributesUtil
	{
		/// <summary>
		///   Gets the attribute.
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns>The member attribute.</returns>
		public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : class
		{
			return GetAttributes<T>(member).FirstOrDefault();
		}

		/// <summary>
		///   Gets the attributes. Does not consider inherited attributes!
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns>The member attributes.</returns>
		public static T[] GetAttributes<T>(this ICustomAttributeProvider member) where T : class
		{
			if (typeof(T) != typeof(object))
			{
				return (T[])member.GetCustomAttributes(typeof(T), false);
			}
			return (T[])member.GetCustomAttributes(false);
		}

		/// <summary>
		///   Gets the type attribute.
		/// </summary>
		/// <param name = "type">The type.</param>
		/// <returns>The type attribute.</returns>
		public static T GetTypeAttribute<T>(this Type type) where T : class
		{
			var attribute = GetAttribute<T>(type);

			if (attribute == null)
			{
				foreach (var baseInterface in type.GetInterfaces())
				{
					attribute = GetTypeAttribute<T>(baseInterface);
					if (attribute != null)
					{
						break;
					}
				}
			}

			return attribute;
		}

		/// <summary>
		///   Gets the type attributes.
		/// </summary>
		/// <param name = "type">The type.</param>
		/// <returns>The type attributes.</returns>
		public static T[] GetTypeAttributes<T>(Type type) where T : class
		{
			var attributes = GetAttributes<T>(type);

			if (attributes.Length == 0)
			{
				foreach (var baseInterface in type.GetInterfaces())
				{
					attributes = GetTypeAttributes<T>(baseInterface);
					if (attributes.Length > 0)
					{
						break;
					}
				}
			}

			return attributes;
		}

		public static object[] GetInterfaceAttributes(Type type)
		{
			return InterfaceAttributeUtil.GetAttributes(type, true);
		}

		public static AttributeUsageAttribute GetAttributeUsage(this Type attributeType)
		{
			var attributes = attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
			return attributes.Length != 0
				? (AttributeUsageAttribute) attributes[0]
				: DefaultAttributeUsage;
		}

		private static readonly AttributeUsageAttribute
			DefaultAttributeUsage = new AttributeUsageAttribute(AttributeTargets.All);

		/// <summary>
		///   Gets the type converter.
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns></returns>
		public static Type GetTypeConverter(MemberInfo member)
		{
			var attrib = GetAttribute<TypeConverterAttribute>(member);

			if (attrib != null)
			{
				return Type.GetType(attrib.ConverterTypeName);
			}

			return null;
		}

		/// <summary>
		///   Gets the attribute.
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns>The member attribute.</returns>
		public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : class
		{
			return GetAttributes<T>(member).FirstOrDefault() != null;
		}
	}
}