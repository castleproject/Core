namespace NVelocity.Util.Introspection
{
	using System;
	using System.Reflection;
	using NVelocity.Runtime;

	/// <summary> This basic function of this class is to return a Method
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
	public class Introspector : IntrospectorBase
	{
		/// <summary>  our engine runtime services
		/// </summary>
		private IRuntimeLogger rlog = null;

		/// <summary>  Recieves our RuntimeServices object
		/// </summary>
		public Introspector(IRuntimeLogger r)
		{
			rlog = r;
		}

		/// <summary>
		/// Gets the method defined by <code>name</code> and
		/// <code>params</code> for the Class <code>c</code>.
		/// </summary>
		/// <param name="c">Class in which the method search is taking place</param>
		/// <param name="name">Name of the method being searched for</param>
		/// <param name="parameters">An array of Objects (not Classes) that describe the the parameters</param>
		/// <returns>The desired Method object.</returns>
		public override MethodInfo GetMethod(Type c, String name, Object[] parameters)
		{
			/*
	    *  just delegate to the base class
	    */

			try
			{
				return base.GetMethod(c, name, parameters);
			}
			catch(AmbiguousException)
			{
				// whoops.  Ambiguous.  Make a nice log message and return null...
				String msg = "Introspection Error : Ambiguous method invocation " + name + "( ";

				for(int i = 0; i < parameters.Length; i++)
				{
					if (i > 0)
						msg = msg + ", ";

					msg = msg + parameters[i].GetType().FullName;
				}

				msg = msg + ") for class " + c;

				rlog.Error(msg);
			}

			return null;
		}

		/// <summary>
		/// Gets the method defined by <code>name</code>
		/// for the Class <code>c</code>.
		/// </summary>
		/// <param name="c">Class in which the method search is taking place</param>
		/// <param name="name">Name of the method being searched for</param>
		/// <returns>The desired <see cref="PropertyInfo"/> object.</returns>
		public override PropertyInfo GetProperty(Type c, String name)
		{
			//  just delegate to the base class
			try
			{
				return base.GetProperty(c, name);
			}
			catch(AmbiguousException)
			{
				// whoops.  Ambiguous.  Make a nice log message and return null...
				String msg = "Introspection Error : Ambiguous property invocation " + name + " for class " + c;
				rlog.Error(msg);
			}
			return null;
		}
	}
}