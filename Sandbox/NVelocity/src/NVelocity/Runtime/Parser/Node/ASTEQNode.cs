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
    using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
    using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;

    /// <summary>  Handles the equivalence operator
    /// *
    /// <arg1>  == <arg2>
    /// *
    /// This operator requires that the LHS and RHS are both of the
    /// same Class.
    /// *
    /// </summary>
    /// <version> $Id: ASTEQNode.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class ASTEQNode:SimpleNode {
	public ASTEQNode(int id):base(id) {}

	public ASTEQNode(Parser p, int id):base(p, id) {}

	/// <summary>Accept the visitor. *
	/// </summary>
	public override System.Object jjtAccept(ParserVisitor visitor, System.Object data) {
	    return visitor.visit(this, data);
	}

	/// <summary>   Calculates the value of the logical expression
	/// *
	/// arg1 == arg2
	/// *
	/// All class types are supported.   Uses equals() to
	/// determine equivalence.  This should work as we represent
	/// with the types we already support, and anything else that
	/// implements equals() to mean more than identical references.
	/// *
	/// *
	/// </summary>
	/// <param name="context"> internal context used to evaluate the LHS and RHS
	/// </param>
	/// <returns>true if equivalent, false if not equivalent,
	/// false if not compatible arguments, or false
	/// if either LHS or RHS is null
	///
	/// </returns>
	public override bool evaluate(InternalContextAdapter context) {
	    System.Object left = jjtGetChild(0).value_Renamed(context);
	    System.Object right = jjtGetChild(1).value_Renamed(context);

	    /*
	    *  they could be null if they are references and not in the context
	    */

	    if (left == null || right == null) {
		rsvc.error((left == null?"Left":"Right") + " side (" + jjtGetChild((left == null?0:1)).literal() + ") of '==' operation " + "has null value. " + "If a reference, it may not be in the context." + " Operation not possible. " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "]");
		return false;
	    }

	    /*
	    *  check to see if they are the same class.  I don't think this is slower
	    *  as I don't think that getClass() results in object creation, and we can
	    *  extend == to handle all classes
	    */

	    if (left.GetType().Equals(right.GetType())) {
		return left.Equals(right);
	    } else {
		rsvc.error("Error in evaluation of == expression." + " Both arguments must be of the same Class." + " Currently left = " + left.GetType() + ", right = " + right.GetType() + ". " + context.CurrentTemplateName + " [line " + Line + ", column " + Column + "] (ASTEQNode)");
	    }

	    return false;
	}
    }
}
