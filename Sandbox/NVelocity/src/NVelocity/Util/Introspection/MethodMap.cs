using System;
using System.Collections;
using System.Reflection;

namespace NVelocity.Util.Introspection {

    /// <summary>*
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:bob@werken.com">Bob McWhirter</a>
    /// </author>
    /// <author>  <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <version>  $Id: MethodMap.cs,v 1.4 2004/12/27 05:55:08 corts Exp $
    /// 
    /// </version>
    public class MethodMap {
	public MethodMap() {
	    InitBlock();
	}
	private void  InitBlock() {
	    methodByNameMap = new Hashtable();
	}
	private const int MORE_SPECIFIC = 0;
	private const int LESS_SPECIFIC = 1;
	private const int INCOMPARABLE = 2;
		
	/// <summary> Keep track of all methods with the same name.
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'methodByNameMap' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
	internal IDictionary methodByNameMap;
		
	/// <summary> Add a method to a list of methods by name.
	/// For a particular class we are keeping track
	/// of all the methods with the same name.
	/// </summary>
	public virtual void  add(MethodInfo method) {
	    String methodName = method.Name;
			
	    IList l = get(methodName);
			
	    if (l == null) {
		l = new ArrayList();
		methodByNameMap.Add(methodName, l);
	    }
			
	    l.Add(method);
			
	    return ;
	}
		
	/// <summary> Return a list of methods with the same name.
	/// *
	/// </summary>
	/// <param name="String">key
	/// </param>
	/// <returns> List list of methods
	/// 
	/// </returns>
	public virtual IList get(String key) {
	    return (IList) methodByNameMap[key];
	}
		
	/// <summary>  <p>
	/// Find a method.  Attempts to find the
	/// most specific applicable method using the
	/// algorithm described in the JLS section
	/// 15.12.2 (with the exception that it can't
	/// distinguish a primitive type argument from
	/// an object type argument, since in reflection
	/// primitive type arguments are represented by
	/// their object counterparts, so for an argument of
	/// type (say) java.lang.Integer, it will not be able
	/// to decide between a method that takes int and a
	/// method that takes java.lang.Integer as a parameter.
	/// </p>
	/// *
	/// <p>
	/// This turns out to be a relatively rare case
	/// where this is needed - however, functionality
	/// like this is needed.
	/// </p>
	/// *
	/// </summary>
	/// <param name="methodName">name of method
	/// </param>
	/// <param name="args">the actual arguments with which the method is called
	/// </param>
	/// <returns> the most specific applicable method, or null if no
	/// method is applicable.
	/// @throws AmbiguousException if there is more than one maximally
	/// specific applicable method
	/// 
	/// </returns>
	public virtual MethodInfo find(String methodName, Object[] args) {
	    IList methodList = get(methodName);
			
	    if (methodList == null) {
		return null;
	    }
			
	    int l = args.Length;
	    Type[] classes = new Type[l];
			
	    for (int i = 0; i < l; ++i) {
		Object arg = args[i];
				
		/*
		* if we are careful down below, a null argument goes in there
		* so we can know that the null was passed to the method
		*/
		classes[i] = arg == null ? null : arg.GetType();
	    }
			
	    return getMostSpecific(methodList, classes);
	}
		
	/// <summary>  simple distinguishable exception, used when
	/// we run across ambiguous overloading
	/// </summary>
	public class AmbiguousException:System.Exception {
	}
		
		
	private static MethodInfo getMostSpecific(IList methods, Type[] classes) {
	    ArrayList applicables = getApplicables(methods, classes);
			
	    if (applicables.Count == 0) {
		return null;
	    }
			
	    if (applicables.Count == 1) {
		return (MethodInfo) applicables[0];
	    }
			
	    /*
	    * This list will contain the maximally specific methods. Hopefully at
	    * the end of the below loop, the list will contain exactly one method,
	    * (the most specific method) otherwise we have ambiguity.
	    */
	    ArrayList maximals = new ArrayList();
			
	    //for (Iterator applicable = applicables.iterator(); applicable.hasNext(); ) {
	    foreach (MethodInfo app in applicables) {
		//MethodInfo app = (MethodInfo) applicable.next();
		//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.reflect.Method.getParameterTypes' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
		ParameterInfo[] appArgs = app.GetParameters();
		bool lessSpecific = false;
				
		//for (Iterator maximal = maximals.iterator(); !lessSpecific && maximal.hasNext(); ) {
		foreach (MethodInfo max in maximals) {
		    //MethodInfo max = (MethodInfo) maximal.next();
					
		    //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.reflect.Method.getParameterTypes' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
		    switch (moreSpecific(appArgs, max.GetParameters())) {
						
			case MORE_SPECIFIC: {
			    /*
			    * This method is more specific than the previously
			    * known maximally specific, so remove the old maximum.
			    */
			    maximals.Remove(max);
			    break;
			}
						
						
			case LESS_SPECIFIC: {
			    /*
			    * This method is less specific than some of the
			    * currently known maximally specific methods, so we
			    * won't add it into the set of maximally specific
			    * methods
			    */
							
			    lessSpecific = true;
			    break;
			}
		    }
		}
				
		if (!lessSpecific) {
		    maximals.Add(app);
		}
	    }
			
	    if (maximals.Count > 1) {
		// We have more than one maximally specific method
		throw new AmbiguousException();
	    }
			
	    return (MethodInfo) maximals[0];
	}
		
	/// <summary> Determines which method signature (represented by a class array) is more
	/// specific. This defines a partial ordering on the method signatures.
	/// </summary>
	/// <param name="c1">first signature to compare
	/// </param>
	/// <param name="c2">second signature to compare
	/// </param>
	/// <returns> MORE_SPECIFIC if c1 is more specific than c2, LESS_SPECIFIC if
	/// c1 is less specific than c2, INCOMPARABLE if they are incomparable.
	/// 
	/// </returns>
	private static int moreSpecific(ParameterInfo[] c1, ParameterInfo[] c2) {
	    bool c1MoreSpecific = false;
	    bool c2MoreSpecific = false;
			
	    for (int i = 0; i < c1.Length; ++i) {
		if (c1[i] != c2[i]) {
		    c1MoreSpecific = c1MoreSpecific || isStrictMethodInvocationConvertible(c2[i], c1[i]);
		    c2MoreSpecific = c2MoreSpecific || isStrictMethodInvocationConvertible(c1[i], c2[i]);
		}
	    }
			
	    if (c1MoreSpecific) {
		if (c2MoreSpecific) {
		    /*
					*  Incomparable due to cross-assignable arguments (i.e.
					* foo(String, Object) vs. foo(Object, String))
					*/
					
		    return INCOMPARABLE;
		}
				
		return MORE_SPECIFIC;
	    }
			
	    if (c2MoreSpecific) {
		return LESS_SPECIFIC;
	    }
			
	    /*
			* Incomparable due to non-related arguments (i.e.
			* foo(Runnable) vs. foo(Serializable))
			*/
			
	    return INCOMPARABLE;
	}
		
	/// <summary> Returns all methods that are applicable to actual argument types.
	/// </summary>
	/// <param name="methods">list of all candidate methods
	/// </param>
	/// <param name="classes">the actual types of the arguments
	/// </param>
	/// <returns> a list that contains only applicable methods (number of
	/// formal and actual arguments matches, and argument types are assignable
	/// to formal types through a method invocation conversion).
	/// 
	/// </returns>
	/// TODO: this used to return a LinkedList -- changed to an ArrayList for now until I can figure out what is really needed
	private static ArrayList getApplicables(IList methods, Type[] classes) {
	    ArrayList list = new ArrayList();
			
	    //for (Iterator imethod = methods.iterator(); imethod.hasNext(); ) {
	    foreach (MethodInfo method in methods) {
		//MethodInfo method = (MethodInfo) imethod.next();
				
		if (isApplicable(method, classes)) {
		    list.Add(method);
		}
	    }
	    return list;
	}
		
	/// <summary>
	/// Returns true if the supplied method is applicable to actual
	/// argument types.
	/// </summary>
	private static bool isApplicable(MethodInfo method, Type[] classes) {
	    //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.reflect.Method.getParameterTypes' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
	    ParameterInfo[] methodArgs = method.GetParameters();
			
	    if (methodArgs.Length != classes.Length) {
		return false;
	    }
			
	    for (int i = 0; i < classes.Length; ++i) {
		if (!isMethodInvocationConvertible(methodArgs[i], classes[i])) {
		    return false;
		}
	    }
			
	    return true;
	}
		
	/// <summary> Determines whether a type represented by a class object is
	/// convertible to another type represented by a class object using a
	/// method invocation conversion, treating object types of primitive
	/// types as if they were primitive types (that is, a Boolean actual
	/// parameter type matches boolean primitive formal type). This behavior
	/// is because this method is used to determine applicable methods for
	/// an actual parameter list, and primitive types are represented by
	/// their object duals in reflective method calls.
	/// *
	/// </summary>
	/// <param name="formal">the formal parameter type to which the actual
	/// parameter type should be convertible
	/// </param>
	/// <param name="actual">the actual parameter type.
	/// </param>
	/// <returns> true if either formal type is assignable from actual type,
	/// or formal is a primitive type and actual is its corresponding object
	/// type or an object type of a primitive type that can be converted to
	/// the formal type.
	/// 
	/// </returns>
	private static bool isMethodInvocationConvertible(ParameterInfo formal, Type actual) {
	    /*
	    * if it's a null, it means the arg was null
	    */
	    if (actual == null && !formal.ParameterType.IsPrimitive) {
		return true;
	    }
			
	    /*
	    *  Check for identity or widening reference conversion
	    */
	    
	    if (actual != null && formal.ParameterType.IsAssignableFrom(actual)) {
		return true;
	    }
			
	    /*
	    * Check for boxing with widening primitive conversion. Note that
	    * actual parameters are never primitives.
	    */
			
	    if (formal.ParameterType.IsPrimitive) {
		if (formal.ParameterType == Type.GetType("Boolean") && actual == (Object) typeof(Boolean))
		    return true;
		if (formal.ParameterType == Type.GetType("Char") && actual == typeof(Char))
		    return true;
		if (formal.ParameterType == Type.GetType("Byte") && actual == (Object) typeof(Byte))
		    return true;
		if (formal.ParameterType == Type.GetType("Int16") && (actual == (Object) typeof(Int16) || actual == (Object) typeof(Byte)))
		    return true;
		if (formal.ParameterType == Type.GetType("Int32") && (actual == (Object) typeof(Int32) || actual == (Object) typeof(Int16) || actual == (Object) typeof(Byte)))
		    return true;
		if (formal.ParameterType == Type.GetType("Int64") && (actual == (Object) typeof(Int64) || actual == (Object) typeof(Int32) || actual == (Object) typeof(Int16) || actual == (Object) typeof(Byte)))
		    return true;
		if (formal.ParameterType == Type.GetType("Single") && (actual == (Object) typeof(Single) || actual == (Object) typeof(Int64) || actual == (Object) typeof(Int32) || actual == (Object) typeof(Int16) || actual == (Object) typeof(Byte)))
		    return true;
		if (formal.ParameterType == Type.GetType("Double") && (actual == (Object) typeof(Double) || actual == (Object) typeof(Single) || actual == (Object) typeof(Int64) || actual == (Object) typeof(Int32) || actual == (Object) typeof(Int16) || actual == (Object) typeof(Byte)))
		    return true;
	    }
			
	    return false;
	}
		
	/// <summary> Determines whether a type represented by a class object is
	/// convertible to another type represented by a class object using a
	/// method invocation conversion, without matching object and primitive
	/// types. This method is used to determine the more specific type when
	/// comparing signatures of methods.
	/// *
	/// </summary>
	/// <param name="formal">the formal parameter type to which the actual
	/// parameter type should be convertible
	/// </param>
	/// <param name="actual">the actual parameter type.
	/// </param>
	/// <returns> true if either formal type is assignable from actual type,
	/// or formal and actual are both primitive types and actual can be
	/// subject to widening conversion to formal.
	/// 
	/// </returns>
	private static bool isStrictMethodInvocationConvertible(ParameterInfo formal, ParameterInfo actual) {
	    /*
	    * we shouldn't get a null into, but if so
	    */
	    if (actual == null && !formal.ParameterType.IsPrimitive) {
		return true;
	    }
			
	    /*
	    *  Check for identity or widening reference conversion
	    */
			
	    if (formal.ParameterType.IsAssignableFrom(actual.ParameterType)) {
		return true;
	    }
			
	    /*
	    *  Check for widening primitive conversion.
	    */
	    if (formal.ParameterType.IsPrimitive) {
		if (formal.ParameterType == Type.GetType("Int16") && (actual.ParameterType == Type.GetType("Byte")))
		    return true;
		if (formal.ParameterType == Type.GetType("Int32") && (actual.ParameterType == Type.GetType("Int16") || actual.ParameterType == Type.GetType("Byte")))
		    return true;
		if (formal.ParameterType == Type.GetType("Int64") && (actual.ParameterType == Type.GetType("Int32") || actual.ParameterType == Type.GetType("Int16") || actual.ParameterType == Type.GetType("Byte")))
		    return true;
		if (formal.ParameterType == Type.GetType("Single") && (actual.ParameterType == Type.GetType("Int64") || actual.ParameterType == Type.GetType("Int32") || actual.ParameterType == Type.GetType("Int16") || actual.ParameterType == Type.GetType("Byte")))
		    return true;
		if (formal.ParameterType == Type.GetType("Double") && (actual.ParameterType == Type.GetType("Single") || actual.ParameterType == Type.GetType("Int64") || actual.ParameterType == Type.GetType("Int32") || actual.ParameterType == Type.GetType("Int16") || actual.ParameterType == Type.GetType("Byte")))
		    return true;
	    }
	    return false;
	}
    }
}