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

namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// A cache of introspection information for a specific class instance.
	/// Keys <see cref="MethodInfo"/> objects by a concatenation of the
	/// method name and the names of classes that make up the parameters.
	/// </summary>
	public class ClassMap
	{
		private static readonly MethodInfo CACHE_MISS =
			typeof(ClassMap).GetMethod("MethodMiss", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly Object OBJECT = new Object();

		private readonly Type type;

		/// <summary> Cache of Methods, or CACHE_MISS, keyed by method
		/// name and actual arguments used to find it.
		/// </summary>
		private readonly Dictionary<string, MethodInfo> methodCache =
			new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, MemberInfo> propertyCache =
			new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);

		private readonly MethodMap methodMap = new MethodMap();

		/// <summary> Standard constructor
		/// </summary>
		public ClassMap(Type type)
		{
			this.type = type;
			PopulateMethodCache();
			PopulatePropertyCache();
		}

		public ClassMap()
		{
		}

		/// <summary>
		/// Class passed into the constructor used to as
		/// the basis for the Method map.
		/// </summary>
		internal Type CachedClass
		{
			get { return type; }
		}


		/// <summary>
		/// Find a Method using the methodKey provided.
		///
		/// Look in the methodMap for an entry.  If found,
		/// it'll either be a CACHE_MISS, in which case we
		/// simply give up, or it'll be a Method, in which
		/// case, we return it.
		///
		/// If nothing is found, then we must actually go
		/// and introspect the method from the MethodMap.
		/// </summary>
		/// <returns>
		/// the class object whose methods are cached by this map.
		/// </returns>
		public MethodInfo FindMethod(String name, Object[] parameters)
		{
			String methodKey = MakeMethodKey(name, parameters);
			MethodInfo cacheEntry;

			if (methodCache.TryGetValue(methodKey, out cacheEntry))
			{
				if (cacheEntry == CACHE_MISS)
				{
					return null;
				}
			}
			else
			{
				try
				{
					cacheEntry = methodMap.Find(name, parameters);
				}
				catch(AmbiguousException)
				{
					// that's a miss :)
					methodCache[methodKey] = CACHE_MISS;
					throw;
				}

				methodCache[methodKey] = cacheEntry ?? CACHE_MISS;
			}

			// Yes, this might just be null.

			return cacheEntry;
		}

		/// <summary>
		/// Find a Method using the methodKey
		/// provided.
		///
		/// Look in the methodMap for an entry.  If found,
		/// it'll either be a CACHE_MISS, in which case we
		/// simply give up, or it'll be a Method, in which
		/// case, we return it.
		///
		/// If nothing is found, then we must actually go
		/// and introspect the method from the MethodMap.
		/// </summary>
		public PropertyInfo FindProperty(String name)
		{
			MemberInfo cacheEntry;

			if (propertyCache.TryGetValue(name, out cacheEntry))
			{
				if (cacheEntry == CACHE_MISS)
				{
					return null;
				}
			}

			// Yes, this might just be null.
			return (PropertyInfo) cacheEntry;
		}

		/// <summary>
		/// Populate the Map of direct hits. These
		/// are taken from all the public methods
		/// that our class provides.
		/// </summary>
		private void PopulateMethodCache()
		{
			// get all publicly accessible methods
			MethodInfo[] methods = GetAccessibleMethods(type);

			// map and cache them
			foreach(MethodInfo method in methods)
			{
				methodMap.Add(method);
				methodCache[MakeMethodKey(method)] = method;
			}
		}

		private void PopulatePropertyCache()
		{
			// get all publicly accessible methods
			PropertyInfo[] properties = GetAccessibleProperties(type);

			// map and cache them
			foreach(PropertyInfo property in properties)
			{
				//propertyMap.add(publicProperty);
				propertyCache[property.Name] = property;
			}
		}

		/// <summary>
		/// Make a methodKey for the given method using
		/// the concatenation of the name and the
		/// types of the method parameters.
		/// </summary>
		private static String MakeMethodKey(MethodInfo method)
		{
			StringBuilder methodKey = new StringBuilder(method.Name);

			foreach(ParameterInfo p in method.GetParameters())
			{
				methodKey.Append(p.ParameterType.FullName);
			}

			return methodKey.ToString();
		}

		private static String MakeMethodKey(String method, Object[] parameters)
		{
			StringBuilder methodKey = new StringBuilder(method);

			if (parameters != null)
			{
				for(int j = 0; j < parameters.Length; j++)
				{
					Object arg = parameters[j];

					if (arg == null) arg = OBJECT;

					methodKey.Append(arg.GetType().FullName);
				}
			}

			return methodKey.ToString();
		}

		/// <summary>
		/// Retrieves public methods for a class.
		/// </summary>
		private static MethodInfo[] GetAccessibleMethods(Type type)
		{
			ArrayList methods = new ArrayList();

			foreach(Type interfaceType in type.GetInterfaces())
			{
				methods.AddRange(interfaceType.GetMethods());
			}

			methods.AddRange(type.GetMethods());

			return (MethodInfo[]) methods.ToArray(typeof(MethodInfo));
		}

		private static PropertyInfo[] GetAccessibleProperties(Type type)
		{
			ArrayList props = new ArrayList();

			foreach(Type interfaceType in type.GetInterfaces())
			{
				props.AddRange(interfaceType.GetProperties());
			}

			props.AddRange(type.GetProperties());

			return (PropertyInfo[]) props.ToArray(typeof(PropertyInfo));
		}
	}
}