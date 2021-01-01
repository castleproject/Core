﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;

	public static class ProxyUtil
	{
		private static readonly SynchronizedDictionary<Assembly, bool> internalsVisibleToDynamicProxy = new SynchronizedDictionary<Assembly, bool>();

		/// <summary>
		///   Creates a delegate of the specified type to a suitable `Invoke` method
		///   on the given <paramref name="proxy"/> instance.
		/// </summary>
		/// <param name="proxy">The proxy instance to which the delegate should be bound.</param>
		/// <typeparam name="TDelegate">The type of delegate that should be created.</typeparam>
		/// <exception cref="MissingMethodException">
		///   The <paramref name="proxy"/> does not have an `Invoke` method that is compatible with
		///   the requested <typeparamref name="TDelegate"/> type.
		/// </exception>
		public static TDelegate CreateDelegateToMixin<TDelegate>(object proxy)
		{
			return (TDelegate)(object)CreateDelegateToMixin(proxy, typeof(TDelegate));
		}

		/// <summary>
		///   Creates a delegate of the specified type to a suitable `Invoke` method
		///   on the given <paramref name="proxy"/> instance.
		/// </summary>
		/// <param name="proxy">The proxy instance to which the delegate should be bound.</param>
		/// <param name="delegateType">The type of delegate that should be created.</param>
		/// <exception cref="MissingMethodException">
		///   The <paramref name="proxy"/> does not have an `Invoke` method that is compatible with
		///   the requested <paramref name="delegateType"/>.
		/// </exception>
		public static Delegate CreateDelegateToMixin(object proxy, Type delegateType)
		{
			if (proxy == null) throw new ArgumentNullException(nameof(proxy));
			if (delegateType == null) throw new ArgumentNullException(nameof(delegateType));
			if (!delegateType.IsDelegateType()) throw new ArgumentException("Type is not a delegate type.", nameof(delegateType));

			var invokeMethod = delegateType.GetMethod("Invoke");
			var proxiedInvokeMethod =
				proxy
				.GetType()
				.GetMember("Invoke", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Cast<MethodInfo>()
				.FirstOrDefault(m => MethodSignatureComparer.Instance.EqualParameters(m, invokeMethod));

			if (proxiedInvokeMethod == null)
			{
				throw new MissingMethodException("The proxy does not have an Invoke method " +
				                                 "that is compatible with the requested delegate type.");
			}
			else
			{
				return Delegate.CreateDelegate(delegateType, proxy, proxiedInvokeMethod);
			}
		}

		public static object GetUnproxiedInstance(object instance)
		{
			if (instance is IProxyTargetAccessor accessor)
			{
				instance = accessor.DynProxyGetTarget();
			}

			return instance;
		}

		public static Type GetUnproxiedType(object instance)
		{
			if (instance is IProxyTargetAccessor accessor)
			{
				var target = accessor.DynProxyGetTarget();
				if (target != null)
				{
					if (ReferenceEquals(target, instance))
					{
						return instance.GetType().BaseType;
					}

					instance = target;
				}
			}

			return instance.GetType();
		}

		public static bool IsProxy(object instance)
		{
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
			var isPublic = target.IsPublic || target.IsNestedPublic;
			if (isPublic)
			{
				return true;
			}

			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
			var isInternalNotNested = target.IsVisible == false && isTargetNested == false;
			var isInternal = isInternalNotNested || isNestedAndInternal;
			if (isInternal && AreInternalsVisibleToDynamicProxy(target.Assembly))
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
				return AreInternalsVisibleToDynamicProxy(method.DeclaringType.Assembly);
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
			var targetAssembly = containingType.Assembly;

			var messageFormat = "Can not create proxy for method {0} because it or its declaring type is not accessible. ";

			var message = string.Format(messageFormat,
				inaccessibleMethod);

			var instructions = ExceptionMessageBuilder.CreateInstructionsToMakeVisible(targetAssembly);
			return message + instructions;
		}
	}
}