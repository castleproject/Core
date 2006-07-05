namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// A cache of introspection information for a specific class instance.
	/// Keys <see cref="MethodInfo"/> objects by a concatenation of the
	/// method name and the names of classes that make up the parameters.
	/// </summary>
	public class ClassMap
	{
		private readonly static CacheMiss CACHE_MISS = new CacheMiss();
		private readonly static Object OBJECT = new Object();

		private readonly Type clazz;

		/// <summary> Cache of Methods, or CACHE_MISS, keyed by method
		/// name and actual arguments used to find it.
		/// </summary>
		private readonly Hashtable methodCache = new Hashtable();

		private readonly Hashtable propertyCache = new Hashtable();
		private readonly MethodMap methodMap = new MethodMap();

		/// <summary> Standard constructor
		/// </summary>
		public ClassMap(Type clazz)
		{
			this.clazz = clazz;
			PopulateMethodCache();
			PopulatePropertyCache();
		}

		public ClassMap()
		{
		}

		/// <summary>
		/// Class passed into the constructor used to as
		/// the basis for the Method map.
		/// </summary>
		internal Type CachedClass
		{
			get { return clazz; }
		}

		private sealed class CacheMiss
		{
		}

		/// <summary>
		/// Find a Method using the methodKey provided.
		///
		/// Look in the methodMap for an entry.  If found,
		/// it'll either be a CACHE_MISS, in which case we
		/// simply give up, or it'll be a Method, in which
		/// case, we return it.
		///
		/// If nothing is found, then we must actually go
		/// and introspect the method from the MethodMap.
		/// </summary>
		/// <returns>
		/// the class object whose methods are cached by this map.
		/// </returns>
		public MethodInfo FindMethod(String name, Object[] params_Renamed)
		{
			String methodKey = MakeMethodKey(name, params_Renamed);
			Object cacheEntry = methodCache[methodKey];

			if (cacheEntry == CACHE_MISS)
			{
				return null;
			}

			if (cacheEntry == null)
			{
				try
				{
					cacheEntry = methodMap.Find(name, params_Renamed);
				}
				catch (AmbiguousException)
				{
					// that's a miss :)
					methodCache[methodKey] = CACHE_MISS;
					throw;
				}

				methodCache[methodKey] = cacheEntry == null ? CACHE_MISS : cacheEntry;
			}

			// Yes, this might just be null.

			return (MethodInfo) cacheEntry;
		}

		/// <summary>
		/// Find a Method using the methodKey
		/// provided.
		///
		/// Look in the methodMap for an entry.  If found,
		/// it'll either be a CACHE_MISS, in which case we
		/// simply give up, or it'll be a Method, in which
		/// case, we return it.
		///
		/// If nothing is found, then we must actually go
		/// and introspect the method from the MethodMap.
		/// </summary>
		public PropertyInfo FindProperty(String name)
		{
			Object cacheEntry = propertyCache[name];

			if (cacheEntry == CACHE_MISS)
				return null;

			// Yes, this might just be null.
			return (PropertyInfo) cacheEntry;
		}

		/// <summary>
		/// Populate the Map of direct hits. These
		/// are taken from all the public methods
		/// that our class provides.
		/// </summary>
		private void PopulateMethodCache()
		{
			// get all publicly accessible methods
			MethodInfo[] methods = GetAccessibleMethods(clazz);

			// map and cache them
			foreach (MethodInfo method in methods)
			{
				methodMap.Add(method);
				methodCache[MakeMethodKey(method)] = method;
			}
		}

		private void PopulatePropertyCache()
		{
			// get all publicly accessible methods
			PropertyInfo[] properties = GetAccessibleProperties(clazz);

			// map and cache them
			foreach (PropertyInfo property in properties)
			{
				//propertyMap.add(publicProperty);
				propertyCache[property.Name] = property;
			}
		}

		/// <summary>
		/// Make a methodKey for the given method using
		/// the concatenation of the name and the
		/// types of the method parameters.
		/// </summary>
		private String MakeMethodKey(MethodInfo method)
		{
			StringBuilder methodKey = new StringBuilder(method.Name);

			foreach (ParameterInfo p in method.GetParameters())
				methodKey.Append(p.ParameterType.FullName);

			return methodKey.ToString();
		}

		private static String MakeMethodKey(String method, Object[] parameters)
		{
			StringBuilder methodKey = new StringBuilder(method);

			if (parameters != null)
			{
				for (int j = 0; j < parameters.Length; j++)
				{
					Object arg = parameters[j];

					if (arg == null)
						arg = OBJECT;

					methodKey.Append(arg.GetType().FullName);
				}
			}

			return methodKey.ToString();
		}

		/// <summary>
		/// Retrieves public methods for a class.
		/// </summary>
		private static MethodInfo[] GetAccessibleMethods(Type clazz)
		{
			ArrayList methods = new ArrayList();
			foreach (Type iface in clazz.GetInterfaces())
				methods.AddRange(iface.GetMethods());
			methods.AddRange(clazz.GetMethods());

			return (MethodInfo[]) methods.ToArray(typeof(MethodInfo));
		}

		private static PropertyInfo[] GetAccessibleProperties(Type clazz)
		{
			ArrayList props = new ArrayList();
			foreach (Type iface in clazz.GetInterfaces())
				props.AddRange(iface.GetProperties());
			props.AddRange(clazz.GetProperties());

			return (PropertyInfo[]) props.ToArray(typeof(PropertyInfo));
		}
	}
}