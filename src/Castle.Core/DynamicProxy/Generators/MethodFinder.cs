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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	///   Returns the methods implemented by a type. Use this instead of Type.GetMethods() to work around a CLR issue
	///   where duplicate MethodInfos are returned by Type.GetMethods() after a token of a generic type's method was loaded.
	/// </summary>
	public class MethodFinder
	{
		private static readonly Dictionary<Type, MethodInfo[]> cachedMethodInfosByType = new Dictionary<Type, MethodInfo[]>();
		private static readonly object lockObject = new object();

		public static MethodInfo[] GetAllInstanceMethods(Type type, BindingFlags flags)
		{
			if ((flags & ~(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) != 0)
			{
				throw new ArgumentException("MethodFinder only supports the Public, NonPublic, and Instance binding flags.", "flags");
			}

			MethodInfo[] methodsInCache;

			lock (lockObject)
			{
				if (!cachedMethodInfosByType.TryGetValue(type, out methodsInCache))
				{
					// We always load all instance methods into the cache, we will filter them later
					methodsInCache = type.GetMethods(
							BindingFlags.Public | BindingFlags.NonPublic
						    | BindingFlags.Instance)
						.Distinct(MethodSignatureComparer.Instance)
						.ToArray();
					cachedMethodInfosByType.Add(
						type,
						methodsInCache);
				}
			}
			return MakeFilteredCopy(methodsInCache, flags & (BindingFlags.Public | BindingFlags.NonPublic));
		}

		private static MethodInfo[] MakeFilteredCopy(MethodInfo[] methodsInCache, BindingFlags visibilityFlags)
		{
			if ((visibilityFlags & ~(BindingFlags.Public | BindingFlags.NonPublic)) != 0)
			{
				throw new ArgumentException("Only supports BindingFlags.Public and NonPublic.", "visibilityFlags");
			}

			var includePublic = (visibilityFlags & BindingFlags.Public) == BindingFlags.Public;
			var includeNonPublic = (visibilityFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic;

			// Return a copy of the cached array, only returning the public methods unless requested otherwise
			var result = new List<MethodInfo>(methodsInCache.Length);

			foreach (var method in methodsInCache)
			{
				if ((method.IsPublic && includePublic) || (!method.IsPublic && includeNonPublic))
				{
					result.Add(method);
				}
			}

			return result.ToArray();
		}

	}
}
