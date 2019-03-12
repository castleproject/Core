// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
#if FEATURE_SECURITY_PERMISSIONS
	using System.Security;
	using System.Security.Permissions;
#endif

	public static class StrongNameUtil
	{
		private static readonly IDictionary<Assembly, bool> signedAssemblyCache = new Dictionary<Assembly, bool>();
		private static readonly object lockObject = new object();

#if FEATURE_SECURITY_PERMISSIONS && DOTNET40
		[SecuritySafeCritical]
#endif
		static StrongNameUtil()
		{
#if FEATURE_SECURITY_PERMISSIONS
			//idea after http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx
			try
			{
				new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
				CanStrongNameAssembly = true;
			}
			catch (SecurityException)
			{
				CanStrongNameAssembly = false;
			}
#else
			CanStrongNameAssembly = true;
#endif
		}

		public static bool IsAssemblySigned(this Assembly assembly)
		{
			lock (lockObject)
			{
				if (signedAssemblyCache.TryGetValue(assembly, out var isSigned) == false)
				{
					isSigned = assembly.ContainsPublicKey();
					signedAssemblyCache.Add(assembly, isSigned);
				}
				return isSigned;
			}
		}

		private static bool ContainsPublicKey(this Assembly assembly)
		{
			// Pulled from a comment on http://www.flawlesscode.com/post/2008/08/Mocking-and-IOC-in-Silverlight-2-Castle-Project-and-Moq-ports.aspx
			return assembly.FullName != null && !assembly.FullName.Contains("PublicKeyToken=null");
		}

		public static bool IsAnyTypeFromUnsignedAssembly(IEnumerable<Type> types)
		{
			return types.Any(t => t.GetTypeInfo().Assembly.IsAssemblySigned() == false);
		}

		public static bool IsAnyTypeFromUnsignedAssembly(Type baseType, IEnumerable<Type> interfaces)
		{
			if (baseType != null && baseType.GetTypeInfo().Assembly.IsAssemblySigned() == false)
			{
				return true;
			}

			return IsAnyTypeFromUnsignedAssembly(interfaces);
		}

		public static bool CanStrongNameAssembly { get; set; }
	}
}