namespace org.apache.velocity.test.view
{
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
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
    using org.apache.velocity.runtime.parser.node;

    /// <summary> Simple class for dumping the AST for a template.
    /// Good for debugging and writing new directives.
    /// </summary>
    public class TemplateNodeView {
	///
	/// <summary> Root of the AST node structure that results from
	/// parsing a template.
	/// </summary>
	private SimpleNode document;

	/// <summary> Visitor used to traverse the AST node structure
	/// and produce a visual representation of the
	/// node structure. Very good for debugging and
	/// writing new directives.
	/// </summary>
	private NodeViewMode visitor;

	/// <summary> Default constructor: sets up the Velocity
	/// Runtime, creates the visitor for traversing
	/// the node structure and then produces the
	/// visual representation by the visitation.
	/// </summary>
	public TemplateNodeView(System.String template) {
	    try {
		RuntimeSingleton.init("velocity.properties");

		System.IO.StreamReader isr = new InputStreamReader(new System.IO.FileStream(template, System.IO.FileMode.Open, System.IO.FileAccess.Read), RuntimeSingleton.getString(RuntimeSingleton.INPUT_ENCODING))
		;

		//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
		System.IO.StreamReader br = new System.IO.StreamReader(isr.BaseStream);

		document = RuntimeSingleton.parse(br, template)
		;

		visitor = new NodeViewMode();
		visitor.Context = null;
		//UPGRADE_ISSUE: The equivalent of parameter java.lang.System.out is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
		visitor.Writer = new System.IO.StreamWriter(System.Console.Out);
		document.jjtAccept(visitor, null);
	    } catch (System.Exception e) {
		System.Console.Out.WriteLine(e);
		SupportClass.WriteStackTrace(e, Console.Error);
	    }
	}

	/// <summary>For testing
	/// </summary>
	[STAThread]
	public static void  Main(System.String[] args) {
	    TemplateNodeView v = new TemplateNodeView(args[0]);
	}
    }
}
