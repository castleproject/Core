using System;
using NVelocity.Runtime;

namespace NVelocity.Util.Introspection {

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
    public class Introspector:IntrospectorBase {
	/// <summary>  define a public string so that it can be looked for
	/// if interested
	/// </summary>

	public const System.String CACHEDUMP_MSG = "Introspector : detected classloader change. Dumping cache.";

	/// <summary>  our engine runtime services
	/// </summary>
	private RuntimeLogger rlog = null;

	/// <summary>  Recieves our RuntimeServices object
	/// </summary>
	public Introspector(RuntimeLogger r) {
	    this.rlog = r;
	}

	/// <summary> Gets the method defined by <code>name</code> and
	/// <code>params</code> for the Class <code>c</code>.
	/// </summary>
	/// <param name="c">Class in which the method search is taking place
	/// </param>
	/// <param name="name">Name of the method being searched for
	/// </param>
	/// <param name="params">An array of Objects (not Classes) that describe the
	/// the parameters
	/// </param>
	/// <returns>The desired Method object.
	/// </returns>
	public override System.Reflection.MethodInfo getMethod(System.Type c, System.String name, System.Object[] params_Renamed) {
	    /*
	    *  just delegate to the base class
	    */

	    try {
		return base.getMethod(c, name, params_Renamed);
	    } catch (AmbiguousException ae) {
		/*
		*  whoops.  Ambiguous.  Make a nice log message and return null...
		*/

		System.String msg = "Introspection Error : Ambiguous method invocation " + name + "( ";

		for (int i = 0; i < params_Renamed.Length; i++) {
		    if (i > 0)
			msg = msg + ", ";

		    msg = msg + params_Renamed[i].GetType().FullName;
		}

		msg = msg + ") for class " + c;

		rlog.error(msg);
	    }

	    return null;
	}

	/// <summary> Gets the method defined by <code>name</code> and
	/// <code>params</code> for the Class <code>c</code>.
	/// </summary>
	/// <param name="c">Class in which the method search is taking place
	/// </param>
	/// <param name="name">Name of the method being searched for
	/// </param>
	/// <param name="params">An array of Objects (not Classes) that describe the
	/// the parameters
	/// </param>
	/// <returns>The desired Method object.
	/// </returns>
	public override System.Reflection.PropertyInfo getProperty(System.Type c, System.String name) {
	    /*
	    *  just delegate to the base class
	    */

	    try {
		return base.getProperty(c, name);
	    } catch (AmbiguousException ae) {
		/*
		*  whoops.  Ambiguous.  Make a nice log message and return null...
		*/

		System.String msg = "Introspection Error : Ambiguous property invocation " + name + " ";
		msg = msg + " for class " + c;
		rlog.error(msg);
	    }
	    return null;
	}


	/// <summary> Clears the classmap and classname
	/// caches, and logs that we did so
	/// </summary>
	protected internal override void  clearCache() {
	    base.clearCache();
	    rlog.info(CACHEDUMP_MSG);
	}
    }
}
