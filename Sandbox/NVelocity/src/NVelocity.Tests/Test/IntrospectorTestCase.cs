namespace org.apache.velocity.test
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
    using System.Collections;

    /// <summary> Test case for the Velocity Introspector which uses
    /// the Java Reflection API to determine the correct
    /// signature of the methods used in VTL templates.
    /// *
    /// This should be split into separate tests for each
    /// of the methods searched for but this is a start
    /// for now.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version> $Id: IntrospectorTestCase.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class IntrospectorTestCase:BaseTestCase {
	private void  InitBlock() {
	    failures = new ArrayList();
	}
	private System.Reflection.MethodInfo method;
	private System.String result;
	private System.String type;
	//UPGRADE_NOTE: The initialization of  'failures' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private ArrayList failures;

	internal IntrospectorTestCase():base("IntrospectorTestCase") {
	    InitBlock();
	}

	/// <summary> Creates a new instance.
	/// </summary>
	public IntrospectorTestCase(System.String name):base(name) {
	    InitBlock();
	}


	public virtual void  runTest() {
	    MethodProvider mp = new MethodProvider();

	    try {
		// Test boolean primitive.
		System.Object[] booleanParams = new System.Object[]{true};
		type = "boolean";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", booleanParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) booleanParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test byte primitive.
		System.Object[] byteParams = new System.Object[]{System.Byte.Parse("1")};
		type = "byte";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", byteParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) byteParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test char primitive.
		System.Object[] characterParams = new System.Object[]{'a'};
		type = "character";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", characterParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) characterParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test double primitive.
		System.Object[] doubleParams = new System.Object[]{(double) 1};
		type = "double";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", doubleParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) doubleParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test float primitive.
		System.Object[] floatParams = new System.Object[]{(float) 1};
		type = "float";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", floatParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) floatParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test integer primitive.
		System.Object[] integerParams = new System.Object[]{(int) 1};
		type = "integer";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", integerParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) integerParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test long primitive.
		System.Object[] longParams = new System.Object[]{(long) 1};
		type = "long";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", longParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) longParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test short primitive.
		System.Object[] shortParams = new System.Object[]{(short) 1};
		type = "short";
		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), type + "Method", shortParams);
		result = (System.String) method.Invoke(mp, (System.Object[]) shortParams);

		if (!result.Equals(type))
		    failures.add(type + "Method could not be found!");

		// Test untouchable

		System.Object[] params_Renamed = new System.Object[]{};

		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), "untouchable", params_Renamed);

		if (method != null)
		    failures.add(type + "able to access a private-access method.");

		// Test really untouchable

		method = RuntimeSingleton.Introspector.getMethod(typeof(MethodProvider), "reallyuntouchable", params_Renamed);

		if (method != null)
		    failures.add(type + "able to access a default-access method.");

		// There were any failures then show all the
		// errors that occured.

		int totalFailures = failures.size();
		if (totalFailures > 0) {
		    System.Text.StringBuilder sb = new System.Text.StringBuilder("\nIntrospection Errors:\n");
		    for (int i = 0; i < totalFailures; i++)
			sb.Append((System.String) failures.get(i)).Append("\n");

		    fail(sb.ToString());
		}
	    } catch (System.Exception e) {
		fail(e.ToString());
	    }
	}

	public class MethodProvider {
	    /*
	    * Methods with native parameter types.
	    */
	    public virtual System.String booleanMethod(bool p) {
		return "boolean";
	    }
	    public virtual System.String byteMethod(sbyte p) {
		return "byte";
	    }
	    public virtual System.String characterMethod(char p) {
		return "character";
	    }
	    public virtual System.String doubleMethod(double p) {
		return "double";
	    }
	    public virtual System.String floatMethod(float p) {
		return "float";
	    }
	    public virtual System.String integerMethod(int p) {
		return "integer";
	    }
	    public virtual System.String longMethod(long p) {
		return "long";
	    }
	    public virtual System.String shortMethod(short p) {
		return "short";
	    }

	    internal virtual System.String untouchable() {
		return "yech";
	    }
	    private System.String reallyuntouchable() {
		return "yech!";
	    }
	}
    }
}
