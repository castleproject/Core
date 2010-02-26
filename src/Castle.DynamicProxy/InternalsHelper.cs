// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using Castle.Core.Internal;

	public class InternalsHelper
	{
		private static readonly Lock internalsToDynProxyLock = Lock.Create();
		private static readonly IDictionary<Assembly, bool> internalsToDynProxy = new Dictionary<Assembly, bool>();

		/// <summary>
		/// Determines whether this assembly has internals visible to dynamic proxy.
		/// </summary>
		/// <param name="asm">The assembly to inspect.</param>
		public static bool IsInternalToDynamicProxy(Assembly asm)
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

				InternalsVisibleToAttribute[] atts = (InternalsVisibleToAttribute[])
													 asm.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);

				bool found = false;

				foreach (InternalsVisibleToAttribute internals in atts)
				{
					if (internals.AssemblyName.Contains(ModuleScope.DEFAULT_ASSEMBLY_NAME))
					{
						found = true;
						break;
					}
				}

				internalsToDynProxy.Add(asm, found);

				return found;

			}
		}

		/// <summary>
		/// Determines whether the specified method is internal.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		/// 	<c>true</c> if the specified method is internal; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsInternal(MethodInfo method)
		{
			return method.IsAssembly || (method.IsFamilyAndAssembly
										 && !method.IsFamilyOrAssembly);
		}
	}
}
