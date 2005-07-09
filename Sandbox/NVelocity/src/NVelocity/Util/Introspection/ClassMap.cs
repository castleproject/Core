namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;

	/// <summary> A cache of introspection information for a specific class instance.
	/// Keys {@link java.lang.Method} objects by a concatenation of the
	/// method name and the names of classes that make up the parameters.
	/// </summary>
	public class ClassMap
	{
		internal Type CachedClass
		{
			get { return clazz; }
		}

		private sealed class CacheMiss
		{
		}

		//UPGRADE_NOTE: Final was removed from the declaration of 'CACHE_MISS '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		private static CacheMiss CACHE_MISS = new CacheMiss();
		//UPGRADE_NOTE: Final was removed from the declaration of 'OBJECT '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		private static Object OBJECT = new Object();

		///
		/// <summary> Class passed into the constructor used to as
		/// the basis for the Method map.
		/// </summary>
		private Type clazz;

		/// <summary> Cache of Methods, or CACHE_MISS, keyed by method
		/// name and actual arguments used to find it.
		/// </summary>
		private Hashtable methodCache = new Hashtable();

		private Hashtable propertyCache = new Hashtable();
		private MethodMap methodMap = new MethodMap();

		/// <summary> Standard constructor
		/// </summary>
		public ClassMap(Type clazz)
		{
			this.clazz = clazz;
			populateMethodCache();
			populatePropertyCache();
		}

		public ClassMap()
		{
		}

		/// <returns>the class object whose methods are cached by this map.
		/// </returns>
		/// <summary> Find a Method using the methodKey
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
		public System.Reflection.MethodInfo findMethod(String name, Object[] params_Renamed)
		{
			String methodKey = makeMethodKey(name, params_Renamed);
			Object cacheEntry = methodCache[methodKey];

			if (cacheEntry == CACHE_MISS)
			{
				return null;
			}

			if (cacheEntry == null)
			{
				try
				{
					cacheEntry = methodMap.find(name, params_Renamed);
				}
				catch (AmbiguousException ae)
				{
					/*
		    *  that's a miss :)
		    */

					methodCache[methodKey] = CACHE_MISS;

					throw ae;
				}

				if (cacheEntry == null)
				{
					methodCache[methodKey] = CACHE_MISS;
				}
				else
				{
					methodCache[methodKey] = cacheEntry;
				}
			}

			// Yes, this might just be null.

			return (System.Reflection.MethodInfo) cacheEntry;
		}

		/// <summary> Find a Method using the methodKey
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
		public PropertyInfo findProperty(String name)
		{
			Object cacheEntry = propertyCache[name];

			if (cacheEntry == CACHE_MISS)
			{
				return null;
			}

			// Yes, this might just be null.
			return (PropertyInfo) cacheEntry;
		}


		/// <summary> Populate the Map of direct hits. These
		/// are taken from all the public methods
		/// that our class provides.
		/// </summary>
		private void populateMethodCache()
		{
			StringBuilder methodKey;

			/*
	    *  get all publicly accessible methods
	    */
			System.Reflection.MethodInfo[] methods = getAccessibleMethods(clazz);

			/*
	    * map and cache them
	    */
			for (int i = 0; i < methods.Length; i++)
			{
				System.Reflection.MethodInfo method = methods[i];

				/*
		*  now get the 'public method', the method declared by a 
		*  public interface or class. (because the actual implementing
		*  class may be a facade...
		*/
				System.Reflection.MethodInfo publicMethod = getPublicMethod(method);

				/*
		*  it is entirely possible that there is no public method for
		*  the methods of this class (i.e. in the facade, a method
		*  that isn't on any of the interfaces or superclass
		*  in which case, ignore it.  Otherwise, map and cache
		*/
				if (publicMethod != null)
				{
					methodMap.add(publicMethod);
					methodCache[makeMethodKey(publicMethod)] = publicMethod;
				}
			}
		}

		private void populatePropertyCache()
		{
			StringBuilder methodKey;

			/*
	    *  get all publicly accessible methods
	    */
			PropertyInfo[] properties = getAccessibleProperties(clazz);

			/*
	    * map and cache them
	    */
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo property = properties[i];

				/*
		*  now get the 'public method', the method declared by a 
		*  public interface or class. (because the actual implementing
		*  class may be a facade...
		*/
				PropertyInfo publicProperty = getPublicProperty(property);

				/*
		*  it is entirely possible that there is no public method for
		*  the methods of this class (i.e. in the facade, a method
		*  that isn't on any of the interfaces or superclass
		*  in which case, ignore it.  Otherwise, map and cache
		*/
				if (publicProperty != null)
				{
					//propertyMap.add(publicProperty);
					propertyCache[publicProperty.Name] = publicProperty;
				}
			}
		}


		/// <summary> Make a methodKey for the given method using
		/// the concatenation of the name and the
		/// types of the method parameters.
		/// </summary>
		private String makeMethodKey(System.Reflection.MethodInfo method)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.reflect.Method.getParameterTypes' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			ParameterInfo[] parameterTypes = method.GetParameters();

			StringBuilder methodKey = new StringBuilder(method.Name);

			for (int j = 0; j < parameterTypes.Length; j++)
			{
				/*
		* If the argument type is primitive then we want
		* to convert our primitive type signature to the 
		* corresponding Object type so introspection for
		* methods with primitive types will work correctly.
		*/

				// TODO:  I don't think that this is needed in .Net - boxing will happen and the types will still be available.

				//		if (parameterTypes[j].GetType().IsPrimitive) {
				//		    if (parameterTypes[j].Equals(System.Type.GetType("System.Boolean")))
				//			methodKey.Append("java.lang.Boolean");
				//		    else if (parameterTypes[j].Equals(System.Type.GetType("System.Byte")))
				//			methodKey.Append("java.lang.Byte");
				//		    else if (parameterTypes[j].Equals(System.Type.GetType("System.Char")))
				//			methodKey.Append("java.lang.Character");
				//		    else if (parameterTypes[j].Equals(System.Type.GetType("System.Double")))
				//			methodKey.Append("java.lang.Double");
				//		    else if (parameterTypes[j].Equals(System.Type.GetType("System.Single")))
				//			methodKey.Append("java.lang.Float");
				//		    else {
				//			//UPGRADE_TODO: Field java.lang.Integer.TYPE was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
				//			if (parameterTypes[j].Equals(typeof(Int32)))
				//			    methodKey.Append("System.Int32");
				//			else {
				//			    //UPGRADE_TODO: Field java.lang.Long.TYPE was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
				//			    if (parameterTypes[j].Equals(typeof(Int64)))
				//				methodKey.Append("System.Int64");
				//			    else if (parameterTypes[j].Equals(System.Type.GetType("System.Int16")))
				//				methodKey.Append("System.Int16");
				//			}
				//		    }
				//		}
				//		else {
				//		    methodKey.Append(parameterTypes[j].FullName);
				//		}
				methodKey.Append(parameterTypes[j].ParameterType.FullName);
			}

			return methodKey.ToString();
		}

		private static String makeMethodKey(String method, Object[] params_Renamed)
		{
			StringBuilder methodKey = new StringBuilder().Append(method);

			if (params_Renamed != null)
			{
				for (int j = 0; j < params_Renamed.Length; j++)
				{
					Object arg = params_Renamed[j];

					if (arg == null)
					{
						arg = OBJECT;
					}

					methodKey.Append(arg.GetType().FullName);
				}
			}

			return methodKey.ToString();
		}

		/// <summary> Retrieves public methods for a class. In case the class is not
		/// public, retrieves methods with same signature as its public methods
		/// from public superclasses and interfaces (if they exist). Basically
		/// upcasts every method to the nearest acccessible method.
		/// </summary>
		private static System.Reflection.MethodInfo[] getAccessibleMethods(Type clazz)
		{
			System.Reflection.MethodInfo[] methods = clazz.GetMethods();


			// TODO:  the rest of this method is trying to determine what is supposed to be callable - I think .Net just returns what is callable
			return methods;

			/*
	    *  Short circuit for the (hopefully) majority of cases where the
	    *  clazz is public
	    */

			//UPGRADE_TODO: Method java.lang.reflect.Modifier.isPublic was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
			//UPGRADE_ISSUE: Method 'java.lang.Class.getModifiers' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassgetModifiers"'
			if (clazz.IsPublic)
			{
				return methods;
			}

			/*
	    *  No luck - the class is not public, so we're going the longer way.
	    */

			MethodInfo[] methodInfos = new MethodInfo[methods.Length];

			for (int i = methods.Length; i-- > 0; )
			{
				methodInfos[i] = new MethodInfo(methods[i]);
			}

			int upcastCount = getAccessibleMethods(clazz, methodInfos, 0);

			/*
	    *  Reallocate array in case some method had no accessible counterpart.
	    */

			if (upcastCount < methods.Length)
			{
				methods = new System.Reflection.MethodInfo[upcastCount];
			}

			int j = 0;
			for (int i = 0; i < methodInfos.Length; ++i)
			{
				MethodInfo methodInfo = methodInfos[i];
				if (methodInfo.upcast)
				{
					methods[j++] = methodInfo.method;
				}
			}
			return methods;
		}

		private static PropertyInfo[] getAccessibleProperties(Type clazz)
		{
			PropertyInfo[] properties = clazz.GetProperties();

			//TODO
			return properties;

			/*
	    *  Short circuit for the (hopefully) majority of cases where the
	    *  clazz is public
	    */
			if (clazz.IsPublic)
			{
				return properties;
			}

			/*
	    *  No luck - the class is not public, so we're going the longer way.
	    */

			properties = new PropertyInfo[0];
			return properties;

			// TODO
			//	    MethodInfo[] methodInfos = new MethodInfo[methods.Length];
			//
			//	    for (int i = methods.Length; i-- > 0; ) {
			//		methodInfos[i] = new MethodInfo(methods[i]);
			//	    }
			//
			//	    int upcastCount = getAccessibleMethods(clazz, methodInfos, 0);
			//
			//	    /*
			//	    *  Reallocate array in case some method had no accessible counterpart.
			//	    */
			//
			//	    if (upcastCount < methods.Length) {
			//		methods = new System.Reflection.MethodInfo[upcastCount];
			//	    }
			//
			//	    int j = 0;
			//	    for (int i = 0; i < methodInfos.Length; ++i) {
			//		MethodInfo methodInfo = methodInfos[i];
			//		if (methodInfo.upcast) {
			//		    methods[j++] = methodInfo.method;
			//		}
			//	    }
			//	    return methods;
		}


		/// <summary>
		/// Recursively finds a match for each method, starting with the class, and then
		/// searching the superclass and interfaces.
		/// </summary>
		/// <param name="clazz">Class to check</param>
		/// <param name="methodInfos">array of methods we are searching to match</param>
		/// <param name="upcastCount">current number of methods we have matched</param>
		/// <returns>count of matched methods</returns>
		private static int getAccessibleMethods(Type clazz, MethodInfo[] methodInfos, int upcastCount)
		{
			int l = methodInfos.Length;

			/*
	    *  if this class is public, then check each of the currently
	    *  'non-upcasted' methods to see if we have a match
	    */

			//UPGRADE_TODO: Method java.lang.reflect.Modifier.isPublic was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
			//UPGRADE_ISSUE: Method 'java.lang.Class.getModifiers' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassgetModifiers"'
			if (clazz.IsPublic)
			{
				for (int i = 0; i < l && upcastCount < l; ++i)
				{
					try
					{
						MethodInfo methodInfo = methodInfos[i];

						if (!methodInfo.upcast)
						{
							methodInfo.tryUpcasting(clazz);
							upcastCount++;
						}
					}
					catch (MethodAccessException e)
					{
						/*
			*  Intentionally ignored - it means
			*  it wasn't found in the current class
			*/
					}
				}

				/*
		*  Short circuit if all methods were upcast
		*/

				if (upcastCount == l)
				{
					return upcastCount;
				}
			}

			/*
	    *   Examine superclass
	    */

			Type superclazz = clazz.BaseType;

			if (superclazz != null)
			{
				upcastCount = getAccessibleMethods(superclazz, methodInfos, upcastCount);

				/*
		*  Short circuit if all methods were upcast
		*/

				if (upcastCount == l)
				{
					return upcastCount;
				}
			}

			/*
	    *  Examine interfaces. Note we do it even if superclazz == null.
	    *  This is redundant as currently java.lang.Object does not implement
	    *  any interfaces, however nothing guarantees it will not in future.
	    */

			Type[] interfaces = clazz.GetInterfaces();

			for (int i = interfaces.Length; i-- > 0; )
			{
				upcastCount = getAccessibleMethods(interfaces[i], methodInfos, upcastCount);

				/*
		*  Short circuit if all methods were upcast
		*/

				if (upcastCount == l)
				{
					return upcastCount;
				}
			}

			return upcastCount;
		}

		/// <summary>  For a given method, retrieves its publicly accessible counterpart.
		/// This method will look for a method with same name
		/// and signature declared in a public superclass or implemented interface of this
		/// method's declaring class. This counterpart method is publicly callable.
		/// </summary>
		/// <param name="method">a method whose publicly callable counterpart is requested.
		/// </param>
		/// <returns>the publicly callable counterpart method. Note that if the parameter
		/// method is itself declared by a public class, this method is an identity
		/// function.
		/// </returns>
		public static System.Reflection.MethodInfo getPublicMethod(System.Reflection.MethodInfo method)
		{
			Type clazz = method.DeclaringType;

			//TODO see other todo messages in this class
			return method;

			/*
	    *   Short circuit for (hopefully the majority of) cases where the declaring
	    *   class is public.
	    */

			if (clazz.IsPublic)
			{
				return method;
			}

			return getPublicMethod(clazz, method.Name, GetMethodParameterTypes(method));
		}

		public static PropertyInfo getPublicProperty(PropertyInfo property)
		{
			Type clazz = property.DeclaringType;

			// TODO:
			return property;

			/*
	    *   Short circuit for (hopefully the majority of) cases where the declaring
	    *   class is public.
	    */
			if (clazz.IsPublic)
			{
				return property;
			}


			//TODO
			return null;
			//	    return getPublicMethod(clazz, method.Name, GetMethodParameterTypes(method));
		}


		/// <summary>  Looks up the method with specified name and signature in the first public
		/// superclass or implemented interface of the class.
		/// </summary>
		/// <param name="class">the class whose method is sought
		/// </param>
		/// <param name="name">the name of the method
		/// </param>
		/// <param name="paramTypes">the classes of method parameters
		/// </param>
		private static System.Reflection.MethodInfo getPublicMethod(Type clazz, String name, Type[] paramTypes)
		{
			/*
	    *  if this class is public, then try to get it
	    */

			if (clazz.IsPublic)
			{
				try
				{
					return clazz.GetMethod(name, (Type[]) paramTypes);
				}
				catch (MethodAccessException e)
				{
					/*
		    *  If the class does not have the method, then neither its
		    *  superclass nor any of its interfaces has it so quickly return
		    *  null.
		    */
					return null;
				}
			}

			/*
	    *  try the superclass
	    */


			Type superclazz = clazz.BaseType;

			if (superclazz != null)
			{
				System.Reflection.MethodInfo superclazzMethod = getPublicMethod(superclazz, name, paramTypes);

				if (superclazzMethod != null)
				{
					return superclazzMethod;
				}
			}

			/*
	    *  and interfaces
	    */

			Type[] interfaces = clazz.GetInterfaces();

			for (int i = 0; i < interfaces.Length; ++i)
			{
				System.Reflection.MethodInfo interfaceMethod = getPublicMethod(interfaces[i], name, paramTypes);

				if (interfaceMethod != null)
				{
					return interfaceMethod;
				}
			}

			return null;
		}

		/// <summary>  Used for the iterative discovery process for public methods.
		/// </summary>
		private sealed class MethodInfo
		{
			internal System.Reflection.MethodInfo method;
			internal String name;
			internal Type[] parameterTypes;
			internal bool upcast;

			internal MethodInfo(System.Reflection.MethodInfo method)
			{
				this.method = null;
				name = method.Name;
				parameterTypes = GetMethodParameterTypes(method);
				upcast = false;
			}

			internal void tryUpcasting(Type clazz)
			{
				method = clazz.GetMethod(name, (Type[]) parameterTypes);
				name = null;
				parameterTypes = null;
				upcast = true;
			}
		}

		private static Type[] GetMethodParameterTypes(System.Reflection.MethodInfo method)
		{
			ParameterInfo[] parms = method.GetParameters();
			Type[] types = new Type[parms.Length];

			for (Int32 i = 0; i < parms.Length; i++)
			{
				types[i] = parms[i].ParameterType;
			}

			return types;
		}


	}
}