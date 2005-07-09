using System;
using System.Collections;
//using ArrayIterator = NVelocity.Util.ArrayIterator;
//using EnumerationIterator = NVelocity.Util.EnumerationIterator;
using Token = NVelocity.Runtime.Parser.Token;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
using ParseErrorException = NVelocity.Exception.ParseErrorException;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
using Introspector = NVelocity.Util.Introspection.Introspector;
using IntrospectionCacheData = NVelocity.Util.Introspection.IntrospectionCacheData;
using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;

namespace NVelocity.Runtime.Directive {

    /// <summary>
    /// Foreach directive used for moving through arrays,
    /// or objects that provide an Iterator.
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: Foreach.cs,v 1.6 2003/10/27 13:54:10 corts Exp $</version>
    public class Foreach:Directive {

	public override System.String Name {
	    get {
		return "foreach";
	    }
	    set {
		throw new NotSupportedException();
	    }
	}

	public override int Type {
	    get {
		return NVelocity.Runtime.Directive.DirectiveConstants_Fields.BLOCK;
	    }
	}

	/// <summary> Return name of this directive.
	/// </summary>

	/// <summary> Return type of this directive.
	/// </summary>

	private static int UNKNOWN = - 1;

	/// <summary> Flag to indicate that the list object being used
	/// in an array.
	/// </summary>
	private const int INFO_ARRAY = 1;

	/// <summary> Flag to indicate that the list object being used
	/// provides an Iterator.
	/// </summary>
	private const int INFO_ITERATOR = 2;

	/// <summary> Flag to indicate that the list object being used
	/// is a Map.
	/// </summary>
	private const int INFO_MAP = 3;

	/// <summary> Flag to indicate that the list object being used
	/// is a Collection.
	/// </summary>
	private const int INFO_COLLECTION = 4;

	/// <summary>  Flag to indicate that the list object being used
	/// is an Enumeration
	/// </summary>
	private const int INFO_ENUMERATION = 5;

	/// <summary>  Flag to indicate that the list object being used
	/// is an IEnumerable
	/// </summary>
	private const int INFO_ENUMERABLE = 6;

	/// <summary> The name of the variable to use when placing
	/// the counter value into the context. Right
	/// now the default is $velocityCount.
	/// </summary>
	private System.String counterName;

	/// <summary> What value to start the loop counter at.
	/// </summary>
	private int counterInitialValue;

	/// <summary> The reference name used to access each
	/// of the elements in the list object. It
	/// is the $item in the following:
	///
	/// #foreach ($item in $list)
	///
	/// This can be used class wide because
	/// it is immutable.
	/// </summary>
	private System.String elementKey;


	/// <summary>  simple init - init the tree and get the elementKey from
	/// the AST
	/// </summary>
	public override void  init(RuntimeServices rs, InternalContextAdapter context, INode node) {
	    base.init(rs, context, node);

	    counterName = rsvc.getString(NVelocity.Runtime.RuntimeConstants_Fields.COUNTER_NAME);
	    counterInitialValue = rsvc.getInt(NVelocity.Runtime.RuntimeConstants_Fields.COUNTER_INITIAL_VALUE);

	    /*
	    *  this is really the only thing we can do here as everything
	    *  else is context sensitive
	    */

	    elementKey = node.jjtGetChild(0).FirstToken.image.Substring(1);
	}

	/// <summary>  returns an Iterator to the collection in the #foreach()
	///
	/// </summary>
	/// <param name="context"> current context
	/// </param>
	/// <param name="node">  AST node
	/// </param>
	/// <returns>Iterator to do the dataset
	///
	/// </returns>
	private IEnumerator getIterator(InternalContextAdapter context, INode node) {
	    /*
	    *  get our list object, and punt if it's null.
	    */
	    Object listObject = node.jjtGetChild(2).value_Renamed(context);
	    if (listObject == null)
		return null;

	    /*
	    *  See if we already know what type this is. 
	    *  Use the introspection cache
	    */
	    int type = UNKNOWN;

	    IntrospectionCacheData icd = context.ICacheGet(this);
	    System.Type c = listObject.GetType();

	    /*
	    *  if we have an entry in the cache, and the Class we have
	    *  cached is the same as the Class of the data object
	    *  then we are ok
	    */

	    if (icd != null && icd.contextData == c) {
		/* dig the type out of the cata object */
		type = ((System.Int32) icd.thingy);
	    }

	    /*
	    * If we still don't know what this is, 
	    * figure out what type of object the list
	    * element is, and get the iterator for it
	    */

	    if (type == UNKNOWN) {
		if (listObject.GetType().IsArray)
		    type = INFO_ARRAY;

		// NOTE: IDictionary needs to come before ICollection as it support ICollection
		else if (listObject is IDictionary)
		    type = INFO_MAP;
		else if (listObject is ICollection)
		    type = INFO_COLLECTION;
		else if (listObject is IEnumerable)
		    type = INFO_ENUMERABLE;
		else if (listObject is IEnumerator)
		    type = INFO_ENUMERATION;

		/*
		*  if we did figure it out, cache it
		*/

		if (type != UNKNOWN) {
		    icd = new IntrospectionCacheData();
		    icd.thingy = type;
		    icd.contextData = c;
		    context.ICachePut(this, icd);
		}
	    }

	    /*
	    *  now based on the type from either cache or examination...
	    */

	    switch (type) {

		case INFO_COLLECTION:
		    return ((ICollection) listObject).GetEnumerator();

		case INFO_ENUMERABLE:
		    return ((IEnumerable) listObject).GetEnumerator();

		case INFO_ENUMERATION:
		    rsvc.warn("Warning! The reference " + node.jjtGetChild(2).FirstToken.image + " is an Enumeration in the #foreach() loop at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName + ". Because it's not resetable," + " if used in more than once, this may lead to" + " unexpected results.");
		    return (IEnumerator)listObject;

		case INFO_ARRAY:
		    return ((Array)listObject).GetEnumerator();

		case INFO_MAP:
		    return ((IDictionary)listObject).GetEnumerator();

		default:

		    /*  we have no clue what this is  */
		    rsvc.warn("Could not determine type of enumerator (" + listObject.GetType().Name + ") in " + "#foreach loop for " + node.jjtGetChild(2).FirstToken.image + " at [" + Line + "," + Column + "]" + " in template " + context.CurrentTemplateName);

		    return null;

	    }
	}

	/// <summary>  renders the #foreach() block
	/// </summary>
	public override bool render(InternalContextAdapter context, System.IO.TextWriter writer, INode node) {
	    /*
	    *  do our introspection to see what our collection is
	    */

	    IEnumerator i = getIterator(context, node);

	    if (i == null)
		return false;

	    int counter = counterInitialValue;

	    /*
	    *  save the element key if there is one,
	    *  and the loop counter
	    */

	    System.Object o = context.Get(elementKey);
	    System.Object ctr = context.Get(counterName);

	    while (i.MoveNext()) {
		context.Put(counterName, counter);
		context.Put(elementKey, i.Current);
		node.jjtGetChild(3).render(context, writer);
		counter++;
	    }

	    /*
	    * restores the loop counter (if we were nested)
	    * if we have one, else just removes
	    */

	    if (ctr != null) {
		context.Put(counterName, ctr);
	    } else {
		context.Remove(counterName);
	    }


	    /*
	    *  restores element key if exists
	    *  otherwise just removes
	    */

	    if (o != null) {
		context.Put(elementKey, o);
	    } else {
		context.Remove(elementKey);
	    }

	    return true;
	}
    }
}
