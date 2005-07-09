using System;
using System.Collections;
using ReferenceException = NVelocity.Runtime.Exception.ReferenceException;
using NVelocity.Runtime.Parser;
using Introspector = NVelocity.Util.Introspection.Introspector;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
using IContext = NVelocity.Context.IContext;
using NVelocity.App.Events;

namespace NVelocity.Runtime.Parser.Node {

    /// <summary>
    /// This class is responsible for handling the references in
    /// VTL ($foo).
    ///
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a></author>
    /// <author> <a href="mailto:kjohnson@transparent.com>Kent Johnson</a></author>
    /// <version> $Id: ASTReference.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
    public class ASTReference : SimpleNode {
	public virtual System.String RootString {
	    get {
		return rootString;
	    }
	}

	private System.String Root {
	    get {
		Token t = FirstToken;

		/*
		*  we have a special case where something like 
		*  $(\\)*!, where the user want's to see something
		*  like $!blargh in the output, but the ! prevents it from showing.
		*  I think that at this point, this isn't a reference.
		*/

		/* so, see if we have "\\!" */

		int slashbang = t.image.IndexOf("\\!");

		if (slashbang != - 1) {
		    /*
		    *  lets do all the work here.  I would argue that if this occurrs, it's 
		    *  not a reference at all, so preceeding \ characters in front of the $
		    *  are just schmoo.  So we just do the escape processing trick (even | odd)
		    *  and move on.  This kind of breaks the rule pattern of $ and # but '!' really
		    *  tosses a wrench into things.
		    */

		    /*
		    *  count the escapes : even # -> not escaped, odd -> escaped
		    */

		    int i = 0;
		    int len = t.image.Length;

		    i = t.image.IndexOf((System.Char) '$');

		    if (i == - 1) {
			/* yikes! */
			rsvc.error("ASTReference.getRoot() : internal error : no $ found for slashbang.");
			computableReference = false;
			nullString = t.image;
			return nullString;
		    }

		    while (i < len && t.image[i] != '\\') {
			i++;
		    }

		    /*  ok, i is the first \ char */

		    int start = i;
		    int count = 0;

		    while (i < len && t.image[i++] == '\\') {
			count++;
		    }

		    /*
		    *  now construct the output string.  We really don't care about leading 
		    *  slashes as this is not a reference.  It's quasi-schmoo
		    */

		    nullString = t.image.Substring(0, (start) - (0)); // prefix up to the first
		    nullString += t.image.Substring(start, (start + count - 1) - (start)); // get the slashes
		    nullString += t.image.Substring(start + count); // and the rest, including the

		    /*
		    *  this isn't a valid reference, so lets short circuit the value and set calcs
		    */

		    computableReference = false;

		    return nullString;
		}

		/*
		*  we need to see if this reference is escaped.  if so
		*  we will clean off the leading \'s and let the 
		*  regular behavior determine if we should output this
		*  as \$foo or $foo later on in render(). Lazyness..
		*/

		escaped = false;

		if (t.image.StartsWith("\\")) {
		    /*
		    *  count the escapes : even # -> not escaped, odd -> escaped
		    */

		    int i = 0;
		    int len = t.image.Length;

		    while (i < len && t.image[i] == '\\') {
			i++;
		    }


		    if ((i % 2) != 0)
			escaped = true;

		    if (i > 0)
			escPrefix = t.image.Substring(0, (i / 2) - (0));

		    t.image = t.image.Substring(i);
		}

		/*
		*  Look for preceeding stuff like '#' and '$'
		*  and snip it off, except for the
		*  last $
		*/

		int loc1 = t.image.LastIndexOf((System.Char) '$');

		/*
		*  if we have extra stuff, loc > 0
		*  ex. '#$foo' so attach that to 
		*  the prefix.
		*/
		if (loc1 > 0) {
		    morePrefix = morePrefix + t.image.Substring(0, (loc1) - (0));
		    t.image = t.image.Substring(loc1);
		}

		/*
		*  Now it should be clean. Get the literal in case this reference 
		*  isn't backed by the context at runtime, and then figure out what
		*  we are working with.
		*/

		nullString = literal();

		if (t.image.StartsWith("$!")) {
		    referenceType = QUIET_REFERENCE;

		    /*
		    *  only if we aren't escaped do we want to null the output
		    */

		    if (!escaped)
			nullString = "";

		    if (t.image.StartsWith("$!{")) {
			/*
			*  ex : $!{provider.Title} 
			*/

			return t.next.image;
		    } else {
			/*
			*  ex : $!provider.Title
			*/

			return t.image.Substring(2);
		    }
		} else if (t.image.Equals("${")) {
		    /*
		    *  ex : ${provider.Title}
		    */

		    referenceType = FORMAL_REFERENCE;
		    return t.next.image;
		} else if (t.image.StartsWith("$")) {
		    /*
		    *  just nip off the '$' so we have 
		    *  the root
		    */

		    referenceType = NORMAL_REFERENCE;
		    return t.image.Substring(1);
		} else {
		    /*
		    * this is a 'RUNT', which can happen in certain circumstances where
		    *  the parser is fooled into believeing that an IDENTIFIER is a real 
		    *  reference.  Another 'dreaded' MORE hack :). 
		    */
		    referenceType = RUNT;
		    return t.image;
		}

	    }

	}
	public virtual System.String Literal {
	    set {
		/*
		* do only once
		*/

		if (this.literal_Renamed_Field == null)
		    this.literal_Renamed_Field = value;
	    }

	}
	/* Reference types */
	private const int NORMAL_REFERENCE = 1;
	private const int FORMAL_REFERENCE = 2;
	private const int QUIET_REFERENCE = 3;
	private const int RUNT = 4;

	private int referenceType;
	private System.String nullString;
	private System.String rootString;
	private bool escaped = false;
	private bool computableReference = true;
	private System.String escPrefix = "";
	private System.String morePrefix = "";
	private System.String identifier = "";

	//UPGRADE_NOTE: Field literal was renamed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1029"'
	private System.String literal_Renamed_Field = null;

	private int numChildren = 0;

    public ASTReference(int id):base(id) {}

	public ASTReference(Parser p, int id):base(p, id) {}

	/// <summary>Accept the visitor.
	/// </summary>
	public override System.Object jjtAccept(ParserVisitor visitor, System.Object data) {
	    return visitor.visit(this, data);
	}

	public override System.Object init(InternalContextAdapter context, System.Object data) {
	    /*
	    *  init our children
	    */

	    base.init(context, data);

	    /*
	    *  the only thing we can do in init() is getRoot()
	    *  as that is template based, not context based,
	    *  so it's thread- and context-safe
	    */

	    rootString = Root;

	    numChildren = jjtGetNumChildren();

	    /*
	    * and if appropriate...
	    */

	    if (numChildren > 0) {
		identifier = jjtGetChild(numChildren - 1).FirstToken.image;
	    }

	    return data;
	}

	/// <summary>  Returns the 'root string', the reference key
	/// </summary>

	/// <summary>   gets an Object that 'is' the value of the reference
	/// *
	/// </summary>
	/// <param name="o">  unused Object parameter
	/// </param>
	/// <param name="context">context used to generate value
	///
	/// </param>
	public override System.Object execute(System.Object o, InternalContextAdapter context) {

	    if (referenceType == RUNT)
		return null;

	    /*
	    *  get the root object from the context
	    */

	    System.Object result = getVariableValue(context, rootString);

	    if (result == null) {
		return null;
	    }

	    /*
	    * Iteratively work 'down' (it's flat...) the reference
	    * to get the value, but check to make sure that
	    * every result along the path is valid. For example:
	    *
	    * $hashtable.Customer.Name
	    *
	    * The $hashtable may be valid, but there is no key
	    * 'Customer' in the hashtable so we want to stop
	    * when we find a null value and return the null
	    * so the error gets logged.
	    */

	    try {
		for (int i = 0; i < numChildren; i++) {
		    result = jjtGetChild(i).execute(result, context);

		    if (result == null) {
			return null;
		    }
		}

		return result;
	    } catch (MethodInvocationException mie) {
		/*
		*  someone tossed their cookies
		*/

		rsvc.error("Method " + mie.MethodName + " threw exception for reference $" + rootString + " in template " + context.CurrentTemplateName + " at " + " [" + this.Line + "," + this.Column + "]");

		mie.ReferenceName = rootString;
		throw mie;
	    }
	}

	/// <summary>  gets the value of the reference and outputs it to the
	/// writer.
	/// *
	/// </summary>
	/// <param name="context"> context of data to use in getting value
	/// </param>
	/// <param name="writer">  writer to render to
	///
	/// </param>
	public override bool render(InternalContextAdapter context, System.IO.TextWriter writer) {

	    if (referenceType == RUNT) {
		writer.Write(rootString);
		return true;
	    }

	    System.Object value_Renamed = execute(null, context);

	    /*
	    *  if this reference is escaped (\$foo) then we want to do one of two things :
	    *  1) if this is a reference in the context, then we want to print $foo
	    *  2) if not, then \$foo  (its considered shmoo, not VTL)
	    */

	    if (escaped) {
		if (value_Renamed == null) {
		    writer.Write(escPrefix);
		    writer.Write("\\");
		    writer.Write(nullString);
		} else {
		    writer.Write(escPrefix);
		    writer.Write(nullString);
		}

		return true;
	    }

	    /*
	    *  the normal processing
	    *
	    *  if we have an event cartridge, get a new value object
	    */
	    EventCartridge ec = context.EventCartridge;

	    if (ec != null) {
		value_Renamed = ec.referenceInsert(nullString, value_Renamed);
	    }

	    /*
	    *  if value is null...
	    */

	    if (value_Renamed == null) {
		/*
		*  write prefix twice, because it's shmoo, so the \ don't escape each other...
		*/

		writer.Write(escPrefix);
		writer.Write(escPrefix);
		writer.Write(morePrefix);
		writer.Write(nullString);

		if (referenceType != QUIET_REFERENCE && rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG_REFERENCE_LOG_INVALID, true)) {
		    rsvc.warn(new ReferenceException("reference : template = " + context.CurrentTemplateName, this));
		}

		return true;
	    } else {
		/*
		*  non-null processing
		*/

		writer.Write(escPrefix);
		writer.Write(morePrefix);
		//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
		writer.Write(value_Renamed.ToString());

		return true;
	    }
	}

	/// <summary>   Computes boolean value of this reference
	/// Returns the actual value of reference return type
	/// boolean, and 'true' if value is not null
	/// *
	/// </summary>
	/// <param name="context">context to compute value with
	///
	/// </param>
	public override bool evaluate(InternalContextAdapter context) {
	    System.Object value_Renamed = execute(null, context);

	    if (value_Renamed == null) {
		return false;
	    } else if (value_Renamed is System.Boolean) {
		if (((System.Boolean) value_Renamed))
		    return true;
		else
		    return false;
	    } else
		return true;
	}

	public override System.Object value_Renamed(InternalContextAdapter context) {
	    return (computableReference?execute(null, context):null);
	}

	/// <summary>  Sets the value of a complex reference (something like $foo.bar)
	/// Currently used by ASTSetReference()
	/// *
	/// </summary>
	/// <seealso cref=" ASTSetDirective
	/// *
	/// "/>
	/// <param name="context">context object containing this reference
	/// </param>
	/// <param name="value">Object to set as value
	/// </param>
	/// <returns>true if successful, false otherwise
	///
	/// </returns>
	public virtual bool setValue(InternalContextAdapter context, System.Object value_Renamed) {
	    /*
	    *  The rootOfIntrospection is the object we will
	    *  retrieve from the Context. This is the base
	    *  object we will apply reflection to.
	    */

	    System.Object result = getVariableValue(context, rootString);

	    if (result == null) {
		rsvc.error(new ReferenceException("reference set : template = " + context.CurrentTemplateName, this));
		return false;
	    }

	    /*
	    * How many child nodes do we have?
	    */

	    for (int i = 0; i < numChildren - 1; i++) {
		result = jjtGetChild(i).execute(result, context);

		if (result == null) {
		    rsvc.error(new ReferenceException("reference set : template = " + context.CurrentTemplateName, this));
		    return false;
		}
	    }

	    /*
	    *  We support two ways of setting the value in a #set($ref.foo = $value ) :
	    *  1) ref.setFoo( value )
	    *  2) ref,put("foo", value ) to parallel the get() map introspection
	    */

	    try {
		/*
				*  first, we introspect for the set<identifier> setter method
				*/

		System.Object[] params_Renamed = new System.Object[]{value_Renamed};

		System.Type c = result.GetType();
		System.Reflection.PropertyInfo p = null;



		try {
		    p = rsvc.Introspector.getProperty(c, identifier);

		    if (p == null) {
			throw new System.MethodAccessException();
		    }
		} catch (System.MethodAccessException nsme2) {
		    System.Text.StringBuilder sb = new System.Text.StringBuilder();
		    sb.Append(identifier);

		    if (System.Char.IsLower(sb[0])) {
			sb[0] = System.Char.ToUpper(sb[0]);
		    } else {
			sb[0] = System.Char.ToLower(sb[0]);
		    }

		    p = rsvc.Introspector.getProperty(c, sb.ToString());

		    if (p == null) {
			throw new System.MethodAccessException();
		    }
		}

		/*
				*  and if we get here, getMethod() didn't chuck an exception...
				*/

		System.Object[] args = new System.Object[]{};
		p.SetValue(result, value_Renamed, args);
	    } catch (System.MethodAccessException nsme) {
		/*
				*  right now, we only support the Map interface
				*/

		if (result is IDictionary) {
		    try {
			IDictionary d = (IDictionary)result;
			d[identifier] = value_Renamed;
		    } catch (System.Exception ex) {
			rsvc.error("ASTReference Map.put : exception : " + ex + " template = " + context.CurrentTemplateName + " [" + this.Line + "," + this.Column + "]");
			return false;
		    }
		} else {
		    rsvc.error("ASTReference : cannot find " + identifier + " as settable property or key to Map in" + " template = " + context.CurrentTemplateName + " [" + this.Line + "," + this.Column + "]");
		    return false;

		}
	    } catch (System.Reflection.TargetInvocationException ite) {
		/*
				*  this is possible 
				*/

		throw new MethodInvocationException("ASTReference : Invocation of method '" + identifier + "' in  " + result.GetType() + " threw exception " + ite.GetBaseException().GetType(), ite.GetBaseException(), identifier);
	    } catch (System.Exception e) {
		/*
				*  maybe a security exception?
				*/
		rsvc.error("ASTReference setValue() : exception : " + e + " template = " + context.CurrentTemplateName + " [" + this.Line + "," + this.Column + "]");
		return false;
	    }

	    return true;
	}


	public virtual System.Object getVariableValue(IContext context, System.String variable) {
	    return context.Get(variable);
	}


	/// <summary>  Routine to allow the literal representation to be
	/// externally overridden.  Used now in the VM system
	/// to override a reference in a VM tree with the
	/// literal of the calling arg to make it work nicely
	/// when calling arg is null.  It seems a bit much, but
	/// does keep things consistant.
	/// *
	/// Note, you can only set the literal once...
	/// *
	/// </summary>
	/// <param name="literal">String to render to when null
	///
	/// </param>

	/// <summary>  Override of the SimpleNode method literal()
	/// Returns the literal representation of the
	/// node.  Should be something like
	/// $<token>.
	/// </summary>
	public override System.String literal() {
	    if (literal_Renamed_Field != null)
		return literal_Renamed_Field;

	    return base.literal();
	}
    }
}
