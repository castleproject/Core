namespace NVelocity.Util.Introspection
{
	using System;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// This basic function of this class is to return a Method
	/// object for a particular class given the name of a method
	/// and the parameters to the method in the form of an Object[]
	///
	/// The first time the Introspector sees a
	/// class it creates a class method map for the
	/// class in question. Basically the class method map
	/// is a Hastable where Method objects are keyed by a
	/// concatenation of the method name and the names of
	/// classes that make up the parameters.
	///
	/// For example, a method with the following signature:
	///
	/// public void method(String a, StringBuffer b)
	///
	/// would be mapped by the key:
	///
	/// "method" + "java.lang.String" + "java.lang.StringBuffer"
	///
	/// This mapping is performed for all the methods in a class
	/// and stored for
	/// </summary>
	/// <version> $Id: IntrospectorBase.cs,v 1.3 2003/10/27 13:54:12 corts Exp $ </version>
	public abstract class IntrospectorBase
	{
		/// <summary>
		/// Holds the method maps for the classes we know about, keyed by
		/// Class object.
		/// </summary>
		protected internal Hashtable classMethodMaps = new Hashtable();

		/// <summary>
		/// Holds the qualified class names for the classes
		/// we hold in the classMethodMaps hash
		/// </summary>
		protected internal IList cachedClassNames = new ArrayList();

		/// <summary>
		/// Gets the method defined by <code>name</code> and
		/// <code>params</code> for the Class <code>c</code>.
		/// </summary>
		/// <param name="c">Class in which the method search is taking place</param>
		/// <param name="name">Name of the method being searched for</param>
		/// <param name="parameters">An array of Objects (not Classes) that describe the the parameters</param>
		/// <returns>The desired <see cref="MethodInfo"/> object.</returns>
		public virtual MethodInfo GetMethod(Type c, String name, Object[] parameters)
		{
			if (c == null)
				throw new Exception("Introspector.getMethod(): Class method key was null: " + name);

			ClassMap classMap = null;

			lock(classMethodMaps)
			{
				classMap = (ClassMap) classMethodMaps[c];

				// if we don't have this, check to see if we have it
				// by name.  if so, then we have a classloader change
				// so dump our caches.
				if (classMap == null)
				{
					if (cachedClassNames.Contains(c.FullName))
					{
						// we have a map for a class with same name, but not
						// this class we are looking at.  This implies a 
						// classloader change, so dump
						ClearCache();
					}

					classMap = CreateClassMap(c);
				}
			}

			return classMap.FindMethod(name, parameters);
		}

		/// <summary>
		/// Gets the method defined by <code>name</code>
		/// for the Class <code>c</code>.
		/// </summary>
		/// <param name="c">Class in which the method search is taking place</param>
		/// <param name="name">Name of the method being searched for</param>
		/// <returns>The desired <see cref="PropertyInfo"/> object.</returns>
		public virtual PropertyInfo GetProperty(Type c, String name)
		{
			if (c == null)
				throw new Exception("Introspector.getMethod(): Class method key was null: " + name);

			ClassMap classMap = null;

			lock(classMethodMaps)
			{
				classMap = (ClassMap) classMethodMaps[c];

				// if we don't have this, check to see if we have it
				// by name.  if so, then we have a classloader change
				// so dump our caches.

				if (classMap == null)
				{
					classMap = CreateClassMap(c);
				}
			}

			return classMap.FindProperty(name);
		}

		/// <summary>
		/// Creates a class map for specific class and registers it in the
		/// cache.  Also adds the qualified name to the name->class map
		/// for later Classloader change detection.
		/// </summary>
		protected internal ClassMap CreateClassMap(Type c)
		{
			ClassMap classMap = new ClassMap(c);
			classMethodMaps[c] = classMap;
			cachedClassNames.Add(c.FullName);

			return classMap;
		}

		/// <summary>
		/// Clears the classmap and classname caches
		/// </summary>
		protected virtual internal void ClearCache()
		{
			// since we are synchronizing on this
			// object, we have to clear it rather than
			// just dump it.
			classMethodMaps.Clear();

			// for speed, we can just make a new one
			// and let the old one be GC'd
			cachedClassNames = new ArrayList();
		}
	}
}