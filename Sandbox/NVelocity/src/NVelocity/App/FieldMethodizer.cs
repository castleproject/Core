using System;
using System.Collections;
using System.Reflection;

namespace NVelocity.App {

    /// <summary>
    /// <p>This is a small utility class allow easy access to static fields in a class,
    /// such as string constants.  Velocity will not introspect for class
    /// fields (and won't in the future :), but writing setter/getter methods to do
    /// this really is a pain,  so use this if you really have
    /// to access fields.
    ///
    /// <p>The idea it so enable access to the fields just like you would in Java.
    /// For example, in Java, you would access a static field like
    /// <blockquote><pre>
    /// MyClass.STRING_CONSTANT
    /// </pre></blockquote>
    /// and that is the same thing we are trying to allow here.
    ///
    /// <p>So to use in your Java code, do something like this :
    /// <blockquote><pre>
    /// context.put("runtime", new FieldMethodizer( "NVelocity.Runtime.Runtime" ));
    /// </pre></blockquote>
    /// and then in your template, you can access any of your static fields in this way :
    /// <blockquote><pre>
    /// $runtime.RUNTIME_LOG_WARN_STACKTRACE
    /// </pre></blockquote>
    ///
    /// <p>Right now, this class only methodizes <code>public static</code> fields.  It seems
    /// that anything else is too dangerous.  This class is for convenience accessing
    /// 'constants'.  If you have fields that aren't <code>static</code> it may be better
    /// to handle them by explicitly placing them into the context.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>$Id: FieldMethodizer.cs,v 1.3 2003/10/27 13:54:07 corts Exp $</version>
    public class FieldMethodizer {

	/// <summary>
	/// Hold the field objects by field name
	/// </summary>
	private Hashtable fieldHash = new Hashtable();

	/// <summary>
	/// Hold the class objects by field name
	/// </summary>
	private Hashtable classHash = new Hashtable();

	/// <summary>
	/// Allow object to be initialized without any data. You would use
	/// addObject() to add data later.
	/// </summary>
	public FieldMethodizer() {}

	/// <summary>
	/// Constructor that takes as it's arg the name of the class
	/// to methodize.
	/// </summary>
	/// <param name="s">Name of class to methodize.</param>
	public FieldMethodizer(String s) {
	    try {
		addObject(s);
	    } catch (System.Exception e) {
		Console.Out.WriteLine(e);
	    }
	}

	/// <summary>
	/// Constructor that takes as it's arg a living
	/// object to methodize.  Note that it will still
	/// only methodized the public static fields of
	/// the class.
	/// </summary>
	/// <param name="o">object to methodize.</param>
	public FieldMethodizer(Object o) {
	    try {
		addObject(o);
	    } catch (System.Exception e) {
		Console.Out.WriteLine(e);
	    }
	}

	/// <summary>
	/// Add the Name of the class to methodize
	/// </summary>
	public virtual void  addObject(String s) {
	    Type type = Type.GetType(s);
	    inspect(type);
	}

	/// <summary> Add an Object to methodize
	/// </summary>
	public virtual void  addObject(Object o) {
	    inspect(o.GetType());
	}

	/// <summary>
	/// Accessor method to get the fields by name.
	/// </summary>
	/// <param name="fieldName">Name of static field to retrieve</param>
	/// <returns>The value of the given field.</returns>
	public virtual Object Get(String fieldName) {
	    try {
		FieldInfo f = (FieldInfo) fieldHash[fieldName];
		if (f != null)
		    return f.GetValue((Type) classHash[fieldName]);
	    } catch (System.Exception e) {}
	    return null;
	}

	/// <summary>  Method that retrieves all public static fields
	/// in the class we are methodizing.
	/// </summary>
	private void  inspect(Type clas) {
	    FieldInfo[] fields = clas.GetFields();
	    for (int i = 0; i < fields.Length; i++) {
		/*
		*  only if public and static
		*/
		if (fields[i].IsPublic && fields[i].IsStatic) {
		    fieldHash[fields[i].Name] = fields[i];
		    classHash[fields[i].Name] = clas;
		}
	    }
	}


    }
}
