// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Threading;

	public class InternalsHelper
	{
		private static ReaderWriterLock internalsToDynProxyLock = new ReaderWriterLock();
		private static IDictionary<Assembly, bool> internalsToDynProxy = new Dictionary<Assembly, bool>();

		/// <summary>
		/// Determines whether this assembly has internals visisble to dynamic proxy.
		/// </summary>
		/// <param name="asm">The asm.</param>
		public static bool IsInternalToDynamicProxy(Assembly asm)
		{
			internalsToDynProxyLock.AcquireReaderLock(-1);

			if (internalsToDynProxy.ContainsKey(asm))
			{
				internalsToDynProxyLock.ReleaseReaderLock();

				return internalsToDynProxy[asm];
			}

			internalsToDynProxyLock.UpgradeToWriterLock(-1);

			try
			{
				if (internalsToDynProxy.ContainsKey(asm))
				{
					return internalsToDynProxy[asm];
				}

				InternalsVisibleToAttribute[] atts = (InternalsVisibleToAttribute[])
				                                     asm.GetCustomAttributes(typeof (InternalsVisibleToAttribute), false);

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
			finally
			{
				internalsToDynProxyLock.ReleaseWriterLock();
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