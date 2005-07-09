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
    using NVelocity.Runtime.Parser;
    using Context = NVelocity.Context.IContext;

    /// <summary> Utilities for dealing with the AST node structure.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: NodeUtils.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class NodeUtils {
	/// <summary> Collect all the <SPECIAL_TOKEN>s that
	/// are carried along with a token. Special
	/// tokens do not participate in parsing but
	/// can still trigger certain lexical actions.
	/// In some cases you may want to retrieve these
	/// special tokens, this is simply a way to
	/// extract them.
	/// </summary>
	public static System.String specialText(Token t) {
	    System.String specialText = "";

	    if (t.specialToken == null || t.specialToken.image.StartsWith("##"))
		return specialText;

	    Token tmp_t = t.specialToken;

	    while (tmp_t.specialToken != null) {
		tmp_t = tmp_t.specialToken;
	    }

	    while (tmp_t != null) {
		System.String st = tmp_t.image;

		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		for (int i = 0; i < st.Length; i++) {
		    char c = st[i];

		    if (c == '#' || c == '$') {
			sb.Append(c);
		    }

		    /*
		    *  more dreaded MORE hack :)
		    * 
		    *  looking for ("\\")*"$" sequences
		    */

		    if (c == '\\') {
			bool ok = true;
			bool term = false;

			int j = i;
			for (ok = true; ok && j < st.Length; j++) {
			    char cc = st[j];

			    if (cc == '\\') {
				/*
				*  if we see a \, keep going
				*/
				continue;
			    } else if (cc == '$') {
				/*
				*  a $ ends it correctly
				*/
				term = true;
				ok = false;
			    } else {
				/*
				*  nah...
				*/
				ok = false;
			    }
			}

			if (term) {
			    System.String foo = st.Substring(i, (j) - (i));
			    sb.Append(foo);
			    i = j;
			}
		    }
		}

		specialText += sb.ToString();

		tmp_t = tmp_t.next;
	    }

	    return specialText;
	}

	/// <summary>  complete node literal
	/// *
	/// </summary>
	public static System.String tokenLiteral(Token t) {
	    return specialText(t) + t.image;
	}

	/// <summary> Utility method to interpolate context variables
	/// into string literals. So that the following will
	/// work:
	/// *
	/// #set $name = "candy"
	/// $image.getURI("${name}.jpg")
	/// *
	/// And the string literal argument will
	/// be transformed into "candy.jpg" before
	/// the method is executed.
	/// </summary>
	public static System.String interpolate(System.String argStr, Context vars) {
	    System.Text.StringBuilder argBuf = new System.Text.StringBuilder();

	    for (int cIdx = 0; cIdx < argStr.Length; ) {
		char ch = argStr[cIdx];

		switch (ch) {
		    case '$':
			System.Text.StringBuilder nameBuf = new System.Text.StringBuilder();
			for (++cIdx; cIdx < argStr.Length; ++cIdx) {
			    ch = argStr[cIdx];
			    if (ch == '_' || ch == '-' || System.Char.IsLetterOrDigit(ch))
				nameBuf.Append(ch);
			    else if (ch == '{' || ch == '}')
				continue;
			    else
				break;
			}

			if (nameBuf.Length > 0) {
			    System.Object value_Renamed = vars.Get(nameBuf.ToString());

			    if (value_Renamed == null)
				argBuf.Append("$").Append(nameBuf.ToString());
			    else {
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				argBuf.Append(value_Renamed.ToString());
			    }
			}
			break;



		    default:
			argBuf.Append(ch);
			++cIdx;
			break;

		}
	    }

	    return argBuf.ToString();
	}
    }
}
