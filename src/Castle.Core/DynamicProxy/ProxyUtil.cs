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
	using System.Reflection;

	using Castle.DynamicProxy.Internal;
#if FEATURE_REMOTING
	using System.Runtime.Remoting;
#endif

	public class ProxyUtil
	{
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
		/// Checks to see if a method is accessible to DynamicProxyGenAssembly2.</summary>
		/// <param name="method">The method to check</param>
		/// <returns><c>true</c> if the method is accessible, <c>false</c> otherwise</returns>
		public static bool IsAccessible(MethodBase method)
		{
			return method.IsAccessible();
		}

		/// <summary>
		/// Checks to see if a method is accessible to DynamicProxyGenAssembly2.</summary>
		/// <param name="method">The method to check</param>
		/// <param name="message">If the method is accessible, <c>null</c>; otherwise, an explanation of why the method is not accessible</param>
		/// <returns><c>true</c> if the method is accessible, <c>false</c> otherwise</returns>
		public static bool IsAccessible(MethodBase method, out string message)
		{
			if (method.IsAccessible())
			{
				message = null;
				return true;
			}

			message = CreateMessageForInaccessibleMethod(method);
			return false;
		}

		private static string CreateMessageForInaccessibleMethod(MethodBase inaccessibleMethod)
		{
			var containingType = inaccessibleMethod.DeclaringType;
			var targetAssembly = containingType.GetTypeInfo().Assembly;

			var messageFormat = "Can not create proxy for method {0} because it is not accessible. ";

			var message = string.Format(messageFormat,
				inaccessibleMethod);

			var instructions = InternalsUtil.CreateInstructionsToMakeVisible(targetAssembly);
			return message + instructions;
		}
	}
}