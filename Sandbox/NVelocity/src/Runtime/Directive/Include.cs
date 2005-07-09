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
    using Resource = NVelocity.Runtime.Resource.Resource;
    using StringUtils = NVelocity.Util.StringUtils;
    using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
    using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;

    using NVelocity.Context;
    using NVelocity.Runtime.Parser.Node;


    /// <summary> Pluggable directive that handles the #include() statement in VTL.
    /// This #include() can take multiple arguments of either
    /// StringLiteral or Reference.
    /// *
    /// Notes:
    /// -----
    /// 1) The included source material can only come from somewhere in
    /// the TemplateRoot tree for security reasons. There is no way
    /// around this.  If you want to include content from elsewhere on
    /// your disk, use a link from somwhere under Template Root to that
    /// content.
    /// *
    /// 2) By default, there is no output to the render stream in the event of
    /// a problem.  You can override this behavior with two property values :
    /// include.output.errormsg.start
    /// include.output.errormsg.end
    /// If both are defined in velocity.properties, they will be used to
    /// in the render output to bracket the arg string that caused the
    /// problem.
    /// Ex. : if you are working in html then
    /// include.output.errormsg.start=<!-- #include error :
    /// include.output.errormsg.end= -->
    /// might be an excellent way to start...
    /// *
    /// 3) As noted above, #include() can take multiple arguments.
    /// Ex : #include( "foo.vm" "bar.vm" $foo )
    /// will simply include all three if valid to output w/o any
    /// special separator.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:kav@kav.dk">Kasper Nielsen</a>
    /// </author>
    /// <version> $Id: Include.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class Include:Directive {
	public override System.String Name {
	    get {
		return "include";
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
	private System.String outputMsgStart = "";
	private System.String outputMsgEnd = "";

	/// <summary> Return name of this directive.
	/// </summary>

	/// <summary> Return type of this directive.
	/// </summary>

	/// <summary>  simple init - init the tree and get the elementKey from
	/// the AST
	/// </summary>
	public override void  init(RuntimeServices rs, InternalContextAdapter context, INode node) {
	    base.init(rs, context, node);

	    /*
	    *  get the msg, and add the space so we don't have to
	    *  do it each time
	    */
	    outputMsgStart = rsvc.getString(NVelocity.Runtime.RuntimeConstants_Fields.ERRORMSG_START);
	    outputMsgStart = outputMsgStart + " ";

	    outputMsgEnd = rsvc.getString(NVelocity.Runtime.RuntimeConstants_Fields.ERRORMSG_END);
	    outputMsgEnd = " " + outputMsgEnd;
	}

	/// <summary>  iterates through the argument list and renders every
	/// argument that is appropriate.  Any non appropriate
	/// arguments are logged, but render() continues.
	/// </summary>
	public override bool render(InternalContextAdapter context, System.IO.TextWriter writer, INode node) {
	    /*
	    *  get our arguments and check them
	    */

	    int argCount = node.jjtGetNumChildren();

	    for (int i = 0; i < argCount; i++) {
		/*
				*  we only handle StringLiterals and References right now
				*/

		INode n = node.jjtGetChild(i);

		if (n.Type == NVelocity.Runtime.Parser.ParserTreeConstants.JJTSTRINGLITERAL || n.Type == NVelocity.Runtime.Parser.ParserTreeConstants.JJTREFERENCE) {
		    if (!renderOutput(n, context, writer))
			outputErrorToStream(writer, "error with arg " + i + " please see log.");
		} else {
		    //UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
		    rsvc.error("#include() error : invalid argument type : " + n.ToString());
		    outputErrorToStream(writer, "error with arg " + i + " please see log.");
		}
	    }

	    return true;
	}

	/// <summary>  does the actual rendering of the included file
	/// *
	/// </summary>
	/// <param name="node">AST argument of type StringLiteral or Reference
	/// </param>
	/// <param name="context">valid context so we can render References
	/// </param>
	/// <param name="writer">output Writer
	/// </param>
	/// <returns>boolean success or failure.  failures are logged
	///
	/// </returns>
	private bool renderOutput(INode node, InternalContextAdapter context, System.IO.TextWriter writer) {
	    System.String arg = "";

	    if (node == null) {
		rsvc.error("#include() error :  null argument");
		return false;
	    }

	    /*
	    *  does it have a value?  If you have a null reference, then no.
	    */
	    System.Object value_Renamed = node.value_Renamed(context);
	    if (value_Renamed == null) {
		rsvc.error("#include() error :  null argument");
		return false;
	    }

	    /*
	    *  get the path
	    */
	    //UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
	    arg = value_Renamed.ToString();

	    Resource resource = null;

	    Resource current = context.CurrentResource;

	    try {
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

		resource = rsvc.getContent(arg, encoding);
	    } catch (ResourceNotFoundException rnfe) {
		/*
				* the arg wasn't found.  Note it and throw
				*/

		rsvc.error("#include(): cannot find resource '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");
		throw rnfe;
	    } catch (System.Exception e) {
		rsvc.error("#include(): arg = '" + arg + "',  called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ") : " + e);
	    }

	    if (resource == null)
		return false;

	    writer.Write((System.String) resource.Data);
	    return true;
	}

	/// <summary>  Puts a message to the render output stream if ERRORMSG_START / END
	/// are valid property strings.  Mainly used for end-user template
	/// debugging.
	/// </summary>
	private void  outputErrorToStream(System.IO.TextWriter writer, System.String msg) {
	    if (outputMsgStart != null && outputMsgEnd != null) {
		writer.Write(outputMsgStart);
		writer.Write(msg);
		writer.Write(outputMsgEnd);
	    }
	    return ;
	}
    }
}
