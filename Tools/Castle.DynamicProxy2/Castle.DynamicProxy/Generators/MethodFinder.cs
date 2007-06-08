namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text;
	using System.Collections;

	// Returns the methods implemented by a type. Use this instead of Type.GetMethods() to work around a CLR issue
	// where duplicate MethodInfos are returned by Type.GetMethods() after a token of a generic type's method was loaded.
	public class MethodFinder
	{
		private static Hashtable _cachedMethodInfosByType = new Hashtable();
		private static object _lockObject = new object();

		public static MethodInfo[] GetAllInstanceMethods (Type type, BindingFlags flags)
		{
			if ((flags & ~(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) != 0)
				throw new ArgumentException ("MethodFinder only supports the Public, NonPublic, and Instance binding flags.", "flags");

			MethodInfo[] methodsInCache;
			lock (_lockObject)
			{
				if (!_cachedMethodInfosByType.ContainsKey (type))
				{
					// We always load all instance methods into the cache, we will filter them later
					_cachedMethodInfosByType.Add (
							type,
							type.GetMethods (
									BindingFlags.Public | BindingFlags.NonPublic
									| BindingFlags.Instance));
				}
				methodsInCache = (MethodInfo[]) _cachedMethodInfosByType[type];
			}
			return MakeFilteredCopy (methodsInCache, flags & (BindingFlags.Public | BindingFlags.NonPublic));
		}

		private static MethodInfo[] MakeFilteredCopy (MethodInfo[] methodsInCache, BindingFlags visibilityFlags)
		{
			if ((visibilityFlags & ~(BindingFlags.Public | BindingFlags.NonPublic)) != 0)
				throw new ArgumentException ("Only supports BindingFlags.Public and NonPublic.", "visibilityFlags");

			bool includePublic = (visibilityFlags & BindingFlags.Public) == BindingFlags.Public;
			bool includeNonPublic = (visibilityFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic;

			// Return a copy of the cached array, only returning the public methods unless requested otherwise
			ArrayList result = new ArrayList (methodsInCache.Length);
			foreach (MethodInfo method in methodsInCache)
			{
				if ((method.IsPublic && includePublic) || (!method.IsPublic && includeNonPublic))
					result.Add (method);
			}
			return (MethodInfo[]) result.ToArray (typeof (MethodInfo));
		}
	}
}
