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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

#if !SILVERLIGHT
	using System.Security.Permissions;
	using Castle.Core.Internal;

#endif

	public static class StrongNameUtil
	{
		private static readonly IDictionary<Assembly, bool> signedAssemblyCache = new Dictionary<Assembly, bool>();
#if !SILVERLIGHT
		private static readonly bool canStrongNameAssembly = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).IsGranted();
#endif
		private static readonly object lockObject = new object();



		public static bool IsAssemblySigned(Assembly assembly)
		{
			lock (lockObject)
			{
				if (signedAssemblyCache.ContainsKey(assembly) == false)
				{
					bool isSigned = ContainsPublicKey(assembly);
					signedAssemblyCache.Add(assembly, isSigned);
				}
				return signedAssemblyCache[assembly];
			}
		}

		private static bool ContainsPublicKey(Assembly assembly)
		{
			// Pulled from a comment on http://www.flawlesscode.com/post/2008/08/Mocking-and-IOC-in-Silverlight-2-Castle-Project-and-Moq-ports.aspx
			if (assembly.FullName != null)
				return !assembly.FullName.Contains("PublicKeyToken=null");
			return false;
		}

		public static bool IsAnyTypeFromUnsignedAssembly(IEnumerable<Type> types)
		{
			foreach (Type t in types)
			{
				if (!IsAssemblySigned(t.Assembly))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAnyTypeFromUnsignedAssembly(Type baseType, IEnumerable<Type> interfaces)
		{
			if (baseType != null && !IsAssemblySigned(baseType.Assembly))
				return true;

			return IsAnyTypeFromUnsignedAssembly(interfaces);
		}

		public static bool CanStrongNameAssembly
		{
			get
			{
				return
#if SILVERLIGHT
					false;
#else
					canStrongNameAssembly;
#endif
			}
		}

	}
}
