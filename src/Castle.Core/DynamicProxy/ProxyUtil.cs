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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Threading;

#if FEATURE_REMOTING
	using System.Runtime.Remoting;
#endif

	using Castle.Core.Internal;

	public static class ProxyUtil
	{
		private static readonly SynchronizedDictionary<Assembly, bool> internalsVisibleToDynamicProxy = new SynchronizedDictionary<Assembly, bool>();

		public static object GetUnproxiedInstance(object instance)
		{
#if FEATURE_REMOTING
			if (!RemotingServices.IsTransparentProxy(instance))
#endif
			{
				var accessor = instance as IProxyTargetAccessor;
				if (accessor != null)
				{
					instance = accessor.DynProxyGetTarget();
				}
			}

			return instance;
		}

		public static Type GetUnproxiedType(object instance)
		{
#if FEATURE_REMOTING
			if (!RemotingServices.IsTransparentProxy(instance))
#endif
			{
				var accessor = instance as IProxyTargetAccessor;

				if (accessor != null)
				{
					var target = accessor.DynProxyGetTarget();

					if (target != null)
					{
						if (ReferenceEquals(target, instance))
						{
							return instance.GetType().GetTypeInfo().BaseType;
						}

						instance = target;
					}
				}
			}

			return instance.GetType();
		}

		public static bool IsProxy(object instance)
		{
#if FEATURE_REMOTING
			if (RemotingServices.IsTransparentProxy(instance))
			{
				return true;
			}
#endif
			return instance is IProxyTargetAccessor;
		}

		public static bool IsProxyType(Type type)
		{
			return typeof(IProxyTargetAccessor).IsAssignableFrom(type);
		}

		/// <summary>
		/// Checks whether the specified method is accessible to DynamicProxy.</summary>
		/// <param name="method">The method to check.</param>
		/// <returns><c>true</c> if the method is accessible to DynamicProxy, <c>false</c> otherwise.</returns>
		public static bool IsAccessible(MethodBase method)
		{
			return IsAccessibleMethod(method) && IsAccessibleType(method.DeclaringType);
		}

		/// <summary>
		/// Checks whether the specified method is accessible to DynamicProxy.</summary>
		/// <param name="method">The method to check.</param>
		/// <param name="message">If the method is accessible to DynamicProxy, <c>null</c>; otherwise, an explanation of why the method is not accessible.</param>
		/// <returns><c>true</c> if the method is accessible to DynamicProxy, <c>false</c> otherwise.</returns>
		public static bool IsAccessible(MethodBase method, out string message)
		{
			if (IsAccessible(method))
			{
				message = null;
				return true;
			}

			message = CreateMessageForInaccessibleMethod(method);
			return false;
		}

		/// <summary>
		/// Checks whether the specified type is accessible to DynamicProxy.</summary>
		/// <param name="type">The type to check.</param>
		/// <returns><c>true</c> if the type is accessible to DynamicProxy, <c>false</c> otherwise.</returns>
		public static bool IsAccessible(Type type)
		{
			return IsAccessibleType(type);
		}

		/// <summary>
		///   Determines whether this assembly has internals visible to DynamicProxy.
		/// </summary>
		/// <param name="asm">The assembly to inspect.</param>
		internal static bool AreInternalsVisibleToDynamicProxy(Assembly asm)
		{
			return internalsVisibleToDynamicProxy.GetOrAdd(asm, a =>
			{
				var internalsVisibleTo = asm.GetCustomAttributes<InternalsVisibleToAttribute>();
				return internalsVisibleTo.Any(attr => attr.AssemblyName.Contains(ModuleScope.DEFAULT_ASSEMBLY_NAME));
			});
		}

		internal static bool IsAccessibleType(Type target)
		{
			var typeInfo = target.GetTypeInfo();

			var isPublic = typeInfo.IsPublic || typeInfo.IsNestedPublic;
			if (isPublic)
			{
				return true;
			}

			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (typeInfo.IsNestedAssembly || typeInfo.IsNestedFamORAssem);
			var isInternalNotNested = typeInfo.IsVisible == false && isTargetNested == false;
			var isInternal = isInternalNotNested || isNestedAndInternal;
			if (isInternal && AreInternalsVisibleToDynamicProxy(typeInfo.Assembly))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		///   Checks whether the specified method is accessible to DynamicProxy.
		///   Unlike with <see cref="IsAccessible(MethodBase)"/>, the declaring type's accessibility is ignored.
		/// </summary>
		/// <param name = "method">The method to check.</param>
		/// <returns><c>true</c> if the method is accessible to DynamicProxy, <c>false</c> otherwise.</returns>
		internal static bool IsAccessibleMethod(MethodBase method)
		{
			if (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
			{
				return true;
			}

			if (method.IsAssembly || method.IsFamilyAndAssembly)
			{
				return AreInternalsVisibleToDynamicProxy(method.DeclaringType.GetTypeInfo().Assembly);
			}

			return false;
		}

		/// <summary>
		///   Determines whether the specified method is internal.
		/// </summary>
		/// <param name = "method">The method.</param>
		/// <returns>
		///   <c>true</c> if the specified method is internal; otherwise, <c>false</c>.
		/// </returns>
		internal static bool IsInternal(MethodBase method)
		{
			return method.IsAssembly || (method.IsFamilyAndAssembly && !method.IsFamilyOrAssembly);
		}

		private static string CreateMessageForInaccessibleMethod(MethodBase inaccessibleMethod)
		{
			var containingType = inaccessibleMethod.DeclaringType;
			var targetAssembly = containingType.GetTypeInfo().Assembly;

			var messageFormat = "Can not create proxy for method {0} because it or its declaring type is not accessible. ";

			var message = string.Format(messageFormat,
				inaccessibleMethod);

			var instructions = ExceptionMessageBuilder.CreateInstructionsToMakeVisible(targetAssembly);
			return message + instructions;
		}
	}
}