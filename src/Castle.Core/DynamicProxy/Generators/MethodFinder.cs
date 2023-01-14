// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	///   Returns the methods implemented by a type. Use this instead of Type.GetMethods to filter out duplicate MethodInfos
	///   sometimes reported by the latter. The test suite documents cases where such duplicates may occur.
	/// </summary>
	internal class MethodFinder
	{
		private static readonly Dictionary<Type, MethodInfo[]> cachedMethodInfosByType = new Dictionary<Type, MethodInfo[]>();
		private static readonly object lockObject = new object();

		public static MethodInfo[] GetAllInstanceMethods(Type type)
		{
			MethodInfo[] methodsInCache;

			lock (lockObject)
			{
				if (!cachedMethodInfosByType.TryGetValue(type, out methodsInCache))
				{
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
			return methodsInCache;
		}
	}
}
