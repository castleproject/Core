using System;

namespace NVelocity.Tool {

    /// <summary>
    /// <p>A view tool that allows template designers to load
    /// an arbitrary object into the context. Any object
    /// with a public constructor without parameters can be used
    /// as a view tool.</p>
    /// <p>THIS CLASS IS HERE AS A PROOF OF CONCEPT ONLY. IT IS NOT
    /// INTENDED FOR USE IN PRODUCTION ENVIRONMENTS. USE AT YOUR OWN RISK.</p>
    /// </summary>
    /// <author><a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
    /// <author><a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class ToolLoader {

	public ToolLoader() {}

	/// <summary>
	/// Creates and returns an object of the specified classname.
	/// The object must have a valid default constructor.
	/// </summary>
	/// <param name="clazz">the fully qualified class name of the object</param>
	/// <returns>an instance of the specified class or null if the class
	/// could not be instantiated.</returns>
	public virtual System.Object Load(System.String clazz) {
	    try {
		Type type = System.Type.GetType(clazz);
		Object o = System.Activator.CreateInstance(type);
		return o;
	    } catch (System.Exception) {
		return null;
	    }
	}


    }
}
