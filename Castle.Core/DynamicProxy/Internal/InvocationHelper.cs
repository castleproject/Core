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

namespace Castle.DynamicProxy.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators;

	public static class InvocationHelper
	{
		private static readonly Dictionary<KeyValuePair<MethodInfo, Type>, MethodInfo> cache =
			new Dictionary<KeyValuePair<MethodInfo, Type>, MethodInfo>();

		private static readonly Lock @lock = Lock.Create();

		public static MethodInfo GetMethodOnObject(object target, MethodInfo proxiedMethod)
		{
			if (target == null)
			{
				return null;
			}

			return GetMethodOnType(target.GetType(), proxiedMethod);
		}

		public static MethodInfo GetMethodOnType(Type type, MethodInfo proxiedMethod)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			Debug.Assert(proxiedMethod.DeclaringType.IsAssignableFrom(type),
			             "proxiedMethod.DeclaringType.IsAssignableFrom(type)");
			using (var locker = @lock.ForReadingUpgradeable())
			{
				var methodOnTarget = GetFromCache(proxiedMethod, type);
				if (methodOnTarget != null)
				{
					return methodOnTarget;
				}
				locker.Upgrade();

				methodOnTarget = GetFromCache(proxiedMethod, type);
				if (methodOnTarget != null)
				{
					return methodOnTarget;
				}
				methodOnTarget = ObtainMethod(proxiedMethod, type);
				PutToCache(proxiedMethod, type, methodOnTarget);
				return methodOnTarget;
			}
		}

		private static MethodInfo GetFromCache(MethodInfo methodInfo, Type type)
		{
			var key = new KeyValuePair<MethodInfo, Type>(methodInfo, type);
			MethodInfo method;
			cache.TryGetValue(key, out method);
			return method;
		}

		private static MethodInfo ObtainMethod(MethodInfo proxiedMethod, Type type)
		{
			Type[] genericArguments = null;
			if (proxiedMethod.IsGenericMethod)
			{
				genericArguments = proxiedMethod.GetGenericArguments();
				proxiedMethod = proxiedMethod.GetGenericMethodDefinition();
			}
			var declaringType = proxiedMethod.DeclaringType;
			MethodInfo methodOnTarget = null;
			if (declaringType.IsInterface)
			{
				var mapping = type.GetInterfaceMap(declaringType);
				var index = Array.IndexOf(mapping.InterfaceMethods, proxiedMethod);
				Debug.Assert(index != -1);
				methodOnTarget = mapping.TargetMethods[index];
			}
			else
			{
				// NOTE: this implementation sucks, feel free to improve it.
				var methods = MethodFinder.GetAllInstanceMethods(type, BindingFlags.Public | BindingFlags.NonPublic);
				foreach (var method in methods)
				{
					if (MethodSignatureComparer.Instance.Equals(method.GetBaseDefinition(), proxiedMethod))
					{
						methodOnTarget = method;
						break;
					}
				}
			}
			if (methodOnTarget == null)
			{
				throw new ArgumentException(
					string.Format("Could not find method overriding {0} on type {1}. This is most likely a bug. Please report it.",
					              proxiedMethod, type));
			}

			if (genericArguments == null)
			{
				return methodOnTarget;
			}
			return methodOnTarget.MakeGenericMethod(genericArguments);
		}

		private static void PutToCache(MethodInfo methodInfo, Type type, MethodInfo value)
		{
			var key = new KeyValuePair<MethodInfo, Type>(methodInfo, type);
			cache.Add(key, value);
		}
	}
}