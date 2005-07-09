namespace NVelocity.Runtime.Directive {
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2000-2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
    using System;
    using Template = NVelocity.Template;
    using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
    using Resource = NVelocity.Runtime.Resource.Resource;
    using StringUtils = NVelocity.Util.StringUtils;
    using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
    using ParseErrorException = NVelocity.Exception.ParseErrorException;
    using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
    using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
    using Node = NVelocity.Runtime.Parser.Node.INode;
    using NVelocity.Runtime;

    /// <summary> Pluggable directive that handles the #parse() statement in VTL.
    /// *
    /// Notes:
    /// -----
    /// 1) The parsed source material can only come from somewhere in
    /// the TemplateRoot tree for security reasons. There is no way
    /// around this.  If you want to include content from elsewhere on
    /// your disk, use a link from somwhere under Template Root to that
    /// content.
    /// *
    /// 2) There is a limited parse depth.  It is set as a property
    /// "parse_directive.maxdepth = 10"  for example.  There is a 20 iteration
    /// safety in the event that the parameter isn't set.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <version> $Id: Parse.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class Parse:Directive {
	public override System.String Name {
	    get {
		return "parse";
	    }
	    set {
		throw new NotSupportedException();
	    }
	}
	public override int Type {
	    get {
		return NVelocity.Runtime.Directive.DirectiveConstants_Fields.LINE;
	    }

	}
	private bool ready = false;

	/// <summary> Return name of this directive.
	/// </summary>

	/// <summary> Return type of this directive.
	/// </summary>

	/// <summary>  iterates through the argument list and renders every
	/// argument that is appropriate.  Any non appropriate
	/// arguments are logged, but render() continues.
	/// </summary>
	public override bool render(InternalContextAdapter context, System.IO.TextWriter writer, Node node) {
	    /*
	    *  did we get an argument?
	    */
	    if (node.jjtGetChild(0) == null) {
		rsvc.error("#parse() error :  null argument");
		return false;
	    }

	    /*
	    *  does it have a value?  If you have a null reference, then no.
	    */
	    System.Object value_Renamed = node.jjtGetChild(0).value_Renamed(context);

	    if (value_Renamed == null) {
		rsvc.error("#parse() error :  null argument");
		return false;
	    }

	    /*
	    *  get the path
	    */
	    //UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
	    System.String arg = value_Renamed.ToString();

	    /*
	    *   see if we have exceeded the configured depth.
	    *   If it isn't configured, put a stop at 20 just in case.
	    */

	    System.Object[] templateStack = context.TemplateNameStack;

	    if (templateStack.Length >= rsvc.getInt(NVelocity.Runtime.RuntimeConstants_Fields.PARSE_DIRECTIVE_MAXDEPTH, 20)) {
		System.Text.StringBuilder path = new System.Text.StringBuilder();

		for (int i = 0; i < templateStack.Length; ++i) {
		    path.Append(" > " + templateStack[i]);
		}

		rsvc.error("Max recursion depth reached (" + templateStack.Length + ")" + " File stack:" + path);
		return false;
	    }

	    Resource current = context.CurrentResource;

	    /*
	    *  get the resource, and assume that we use the encoding of the current template
	    *  the 'current resource' can be null if we are processing a stream....
	    */

	    System.String encoding = null;

	    if (current != null) {
		encoding = current.Encoding;
	    } else {
		encoding = (System.String) rsvc.getProperty(NVelocity.Runtime.RuntimeConstants_Fields.INPUT_ENCODING);
	    }

	    /*
	    *  now use the Runtime resource loader to get the template
	    */

	    Template t = null;

	    try {
		t = rsvc.getTemplate(arg, encoding);
	    } catch (ResourceNotFoundException rnfe) {
		/*
				* the arg wasn't found.  Note it and throw
				*/

		rsvc.error("#parse(): cannot find template '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");
		throw rnfe;
	    } catch (ParseErrorException pee) {
		/*
				* the arg was found, but didn't parse - syntax error
				*  note it and throw
				*/

		rsvc.error("#parse(): syntax error in #parse()-ed template '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");

		throw pee;
	    } catch (System.Exception e) {
		rsvc.error("#parse() : arg = " + arg + ".  Exception : " + e);
		return false;
	    }

	    /*
	    *  and render it
	    */
	    try {
		context.PushCurrentTemplateName(arg);
		((SimpleNode) t.Data).render(context, writer);
	    } catch (System.Exception e) {
		/*
				*  if it's a MIE, it came from the render.... throw it...
				*/

		if (e is MethodInvocationException) {
		    throw (MethodInvocationException) e;
		}

		rsvc.error("Exception rendering #parse( " + arg + " )  : " + e);
		return false;
	    } finally {
		context.PopCurrentTemplateName();
	    }

	    return true;
	}
    }
}
