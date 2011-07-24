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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	using Castle.Core.Internal;

	public static class InternalsUtil
	{
		private static readonly IDictionary<Assembly, bool> internalsToDynProxy = new Dictionary<Assembly, bool>();
		private static readonly Lock internalsToDynProxyLock = Lock.Create();

		/// <summary>
		///   Determines whether the specified method is internal.
		/// </summary>
		/// <param name = "method">The method.</param>
		/// <returns>
		///   <c>true</c> if the specified method is internal; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsInternal(this MethodBase method)
		{
			return method.IsAssembly || (method.IsFamilyAndAssembly
			                             && !method.IsFamilyOrAssembly);
		}

		/// <summary>
		///   Determines whether this assembly has internals visible to dynamic proxy.
		/// </summary>
		/// <param name = "asm">The assembly to inspect.</param>
		public static bool IsInternalToDynamicProxy(this Assembly asm)
		{
			using (var locker = internalsToDynProxyLock.ForReadingUpgradeable())
			{
				if (internalsToDynProxy.ContainsKey(asm))
				{
					return internalsToDynProxy[asm];
				}

				locker.Upgrade();

				if (internalsToDynProxy.ContainsKey(asm))
				{
					return internalsToDynProxy[asm];
				}

				var internalsVisibleTo = asm.GetAttributes<InternalsVisibleToAttribute>();
				var found = internalsVisibleTo.Any(VisibleToDynamicProxy);

				internalsToDynProxy.Add(asm, found);
				return found;
			}
		}

		private static bool VisibleToDynamicProxy(InternalsVisibleToAttribute attribute)
		{
			return attribute.AssemblyName.Contains(ModuleScope.DEFAULT_ASSEMBLY_NAME);
		}

		/// <summary>
		///   Checks if the method is public or protected.
		/// </summary>
		/// <param name = "method"></param>
		/// <returns></returns>
		public static bool IsAccessible(this MethodBase method)
		{
			// Accessibility supported by the full framework and CoreCLR
			if (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
			{
				return true;
			}

			if (method.IsFamilyAndAssembly)
			{
				return true;
			}

			if (method.DeclaringType.Assembly.IsInternalToDynamicProxy() && method.IsAssembly)
			{
				return true;
			}
			return false;
		}
	}
}