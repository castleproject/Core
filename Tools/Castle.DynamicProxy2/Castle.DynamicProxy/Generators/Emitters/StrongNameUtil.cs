using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.DynamicProxy.Generators.Emitters
{
	public static class StrongNameUtil
	{
		private static readonly IDictionary signedAssemblyCache = new Hashtable();
		private static readonly object lockObject = new object ();

		public static bool IsAssemblySigned(Assembly assembly)
		{
			lock (lockObject)
			{
				if (signedAssemblyCache.Contains (assembly) == false)
				{
					bool isSigned = ContainsPublicKey (assembly);
					signedAssemblyCache.Add (assembly, isSigned);
				}
				return (bool) signedAssemblyCache[assembly];
			}
		}

		private static bool ContainsPublicKey (Assembly assembly)
		{
			byte[] key = assembly.GetName ().GetPublicKey ();
			return key != null && key.Length != 0;
		}

		public static bool IsAnyTypeFromUnsignedAssembly (IEnumerable<Type> types)
		{
			foreach (Type t in types)
			{
				if (!IsAssemblySigned (t.Assembly))
					return true;
			}
			return false;
		}

		public static bool IsAnyTypeFromUnsignedAssembly (Type baseType, Type[] interfaces)
		{
			return !IsAssemblySigned (baseType.Assembly) || IsAnyTypeFromUnsignedAssembly (interfaces);
		}
	}
}