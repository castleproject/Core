namespace NVelocity.Runtime.Parser.Node
{
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
    using Parser = NVelocity.Runtime.Parser.Parser;
    using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;

    /// <summary> ASTStringLiteral support.  Will interpolate!
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version> $Id: ASTStringLiteral.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class ASTStringLiteral:SimpleNode {
	/* cache the value of the interpolation switch */
	private bool interpolate = true;
	private SimpleNode nodeTree = null;
	private System.String image = "";
	private System.String interpolateimage = "";

	public ASTStringLiteral(int id):base(id) {}

	public ASTStringLiteral(Parser p, int id):base(p, id) {}

	/// <summary>  init : we don't have to do much.  Init the tree (there
	/// shouldn't be one) and then see if interpolation is turned on.
	/// </summary>
	public override System.Object init(InternalContextAdapter context, System.Object data) {
	    /*
	    *  simple habit...  we prollie don't have an AST beneath us
	    */

	    base.init(context, data);

	    /*
	    *  the stringlit is set at template parse time, so we can 
	    *  do this here for now.  if things change and we can somehow 
	    * create stringlits at runtime, this must
	    *  move to the runtime execution path
	    *
	    *  so, only if interpolation is turned on AND it starts 
	    *  with a " AND it has a  directive or reference, then we 
	    *  can  interpolate.  Otherwise, don't bother.
	    */

	    interpolate = rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.INTERPOLATE_STRINGLITERALS, true) && FirstToken.image.StartsWith("\"") && ((FirstToken.image.IndexOf((System.Char) '$') != - 1) || (FirstToken.image.IndexOf((System.Char) '#') != - 1));

	    /*
	    *  get the contents of the string, minus the '/" at each end
	    */

	    image = FirstToken.image.Substring(1, (FirstToken.image.Length - 1) - (1));

	    /*
	    * tack a space on the end (dreaded <MORE> kludge)
	    */

	    interpolateimage = image + " ";

	    if (interpolate) {
		/*
		*  now parse and init the nodeTree
		*/
		//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
		System.IO.TextReader br = new System.IO.StringReader(interpolateimage);

		/*
		* it's possible to not have an initialization context - or we don't
		* want to trust the caller - so have a fallback value if so
		*
		*  Also, do *not* dump the VM namespace for this template
		*/

		nodeTree = rsvc.parse(br, (context != null)?context.CurrentTemplateName:"StringLiteral", false);

		/*
		*  init with context. It won't modify anything
		*/

		nodeTree.init(context, rsvc);
	    }

	    return data;
	}

	/// <summary>Accept the visitor. *
	/// </summary>
	public override System.Object jjtAccept(ParserVisitor visitor, System.Object data) {
	    return visitor.visit(this, data);
	}

	/// <summary>  renders the value of the string literal
	/// If the properties allow, and the string literal contains a $ or a #
	/// the literal is rendered against the context
	/// Otherwise, the stringlit is returned.
	/// </summary>
	public override System.Object value_Renamed(InternalContextAdapter context) {
	    if (interpolate) {
		try {
		    /*
		    *  now render against the real context
		    */

		    System.IO.TextWriter writer = new System.IO.StringWriter();
		    nodeTree.render(context, writer);

		    /*
		    * and return the result as a String
		    */

		    System.String ret = writer.ToString();

		    /*
		    *  remove the space from the end (dreaded <MORE> kludge)
		    */

		    return ret.Substring(0, (ret.Length - 1) - (0));
		} catch (System.Exception e) {
		    /*
		    *  eh.  If anything wrong, just punt 
		    *  and output the literal 
		    */
		    rsvc.error("Error in interpolating string literal : " + e);
		}
	    }

	    /*
	    *  ok, either not allowed to interpolate, there wasn't 
	    *  a ref or directive, or we failed, so
	    *  just output the literal
	    */

	    return image;
	}
    }
}
