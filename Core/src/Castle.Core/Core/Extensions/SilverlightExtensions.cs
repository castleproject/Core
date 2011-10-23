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

namespace Castle.DynamicProxy.SilverlightExtensions
{
	using System;
	using System.Collections.Generic;
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
#endif