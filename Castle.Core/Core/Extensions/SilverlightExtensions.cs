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

#if SILVERLIGHT

namespace Castle.Core.Extensions
{
	using System;
	using System.Collections.Generic;

	public static class SilverlightExtensions
	{
		public static T[] FindAll<T>(this T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<T> list = new List<T>();
			for (int i = 0; i < array.Length; i++)
			{
				if (match(array[i]))
				{
					list.Add(array[i]);
				}
			}
			return list.ToArray();
		}
	}
}
namespace Castle.DynamicProxy.SilverlightExtensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class SilverlightAssertException : Exception
	{
		public SilverlightAssertException(string message) : base(message)
		{
		}

		public SilverlightAssertException()
		{
		}
	}

	public static class Extensions
	{
		public static Type[] FindInterfaces(this Type type, TypeFilter filter, object filterCriteria)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			List<Type> ifaces = new List<Type>();
			foreach (Type iface in type.GetInterfaces())
			{
				if (filter(iface, filterCriteria))
					ifaces.Add(iface);
			}

			return ifaces.ToArray();
		}

		/// <summary>
		/// The silverlight System.Type is missing the IsNested property so this exposes similar functionality.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNested(this Type type)
		{
			return type.DeclaringType != null;
		}

		public static T Find<T>(this T[] array, Predicate<T> match)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			if (match == null)
				throw new ArgumentNullException("match");

			for (int i = 0; i < array.Length; i++)
			{
				if (match(array[i]))
					return array[i];
			}

			return default(T);
		}
	}

	/// <summary>
	/// http://www.dolittle.com/blogs/einar/archive/2008/01/13/missing-enum-getvalues-when-doing-silverlight-for-instance.aspx
	/// </summary>
	public static class EnumHelper
	{
		public static T[] GetValues<T>()
		{
			Type enumType = typeof(T);

			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}

			List<T> values = new List<T>();

			var fields = from field in enumType.GetFields()
						 where field.IsLiteral
						 select field;

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(enumType);
				values.Add((T)value);
			}

			return values.ToArray();
		}

		public static object[] GetValues(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}

			List<object> values = new List<object>();

			var fields = from field in enumType.GetFields()
						 where field.IsLiteral
						 select field;

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(enumType);
				values.Add(value);
			}

			return values.ToArray();
		}
	}
}

namespace System.Reflection
{
	public delegate bool TypeFilter(Type m, object filterCriteria);
}

namespace System.Diagnostics
{
	public sealed class Trace
	{
		public static void WriteLine(string message)
		{
			//TODO:???
		}

		public static void Write(Exception e, string message)
		{
			//TODO:???
		}

		public static void Assert(bool condition)
		{
			if (!condition)
			{
				//TODO:???
				throw new Castle.DynamicProxy.SilverlightExtensions.SilverlightAssertException();
			}
		}

		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				//TODO:???
				throw new Castle.DynamicProxy.SilverlightExtensions.SilverlightAssertException(message);
			}
		}
	}
}
namespace System.ComponentModel
{
	public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
	public class PropertyChangingEventArgs : EventArgs
	{

		public PropertyChangingEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}

		public virtual string PropertyName { get; private set; }
	}
}
namespace System.ComponentModel
{
	using System.Collections.Generic;

	using Castle.Core.Extensions;

	public static class TypeDescriptor
	{
		private static readonly IDictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();

		static TypeDescriptor()
		{
			SimpleConverter.Register();
		}

		public static TypeConverter GetConverter(Type type)
		{
			TypeConverter converter;
			converters.TryGetValue(type, out converter);
			return converter;
		}

		public static void RegisterConverter(Type forType, TypeConverter converter)
		{
			converters[forType] = converter;
		}
	}
}
#if SL3
namespace System.ComponentModel
{
	public interface IDataErrorInfo
	{
		string this[string columnName] { get; }
		string Error { get; }
	}
}
#endif
#endif