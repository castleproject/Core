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
	using System;
	using System.ComponentModel;
	using System.Reflection;

	/// <summary>
	/// Helper class for retrieving attributes.
	/// </summary>
	public static class AttributesUtil
	{
		/// <summary>
		/// Gets the type attribute.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The type attribute.</returns>
		public static T GetTypeAttribute<T>(Type type) where T : class
		{
			T attribute = GetAttribute<T>(type);

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
		/// Gets the attribute.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns>The member attribute.</returns>
		public static T GetAttribute<T>(MemberInfo member) where T : class
		{
			var attributes = GetAttributes<T>(member);
			if (attributes.Length > 0)
			{
				return (T)attributes[0];
			}
			return null;
		}

		/// <summary>
		/// Gets the type attributes.
		/// </summary>
		/// <param name="type">The type.</param>
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

		/// <summary>
		/// Gets the attributes.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns>The member attributes.</returns>
		public static T[] GetAttributes<T>(MemberInfo member) where T : class
		{
			if (typeof(T) != typeof(object))
			{
				return (T[])member.GetCustomAttributes(typeof(T), false);
			}
			return (T[])member.GetCustomAttributes(false);
		}

		/// <summary>
		/// Gets the type converter.
		/// </summary>
		/// <param name="member">The member.</param>
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
	}
}