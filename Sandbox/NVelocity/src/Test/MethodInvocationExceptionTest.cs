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

    /// <summary> Tests if we can hand Velocity an arbitrary class for logging.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: MethodInvocationExceptionTest.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class MethodInvocationExceptionTest:NUnit.Framework.TestCase {
	public virtual System.String Foo
	{
	    set {
		throw new System.Exception("Hello from setFoo()");
	    }

	}
	/// <summary> Default constructor.
	/// </summary>
	public MethodInvocationExceptionTest():base("MethodInvocationExceptionTest") {

	    try {
		/*
		*  init() Runtime with defaults
		*/
		Velocity.init();

	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup MethodInvocationExceptionTest : " + e);
		System.Environment.Exit(1);
	    }
	}

	/// <summary> Runs the test :
	/// *
	/// uses the Velocity class to eval a string
	/// which accesses a method that throws an
	/// exception.
	/// </summary>
	public virtual void  runTest() {
	    System.String template = "$woogie.doException() boing!";

	    VelocityContext vc = new VelocityContext()
	    ;

	    vc.put("woogie", this);

	    System.IO.StringWriter w = new System.IO.StringWriter();

	    try {
		Velocity.evaluate(vc, w, "test", template)
		;
		fail("No exception thrown");
	    } catch (MethodInvocationException mie) {
		System.Console.Out.WriteLine("Caught MIE (good!) :");
		System.Console.Out.WriteLine("  reference = " + mie.ReferenceName);
		System.Console.Out.WriteLine("  method    = " + mie.MethodName);

		//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
		System.Exception t = mie.WrappedThrowable;
		System.Console.Out.WriteLine("  throwable = " + t);

		if (t is System.Exception) {
		    System.Console.Out.WriteLine("  exception = " + ((System.Exception) t).Message);
		}
	    } catch (System.Exception e) {
		fail("Wrong exception thrown, first test." + e);
		SupportClass.WriteStackTrace(e, Console.Error);
	    }

	    /*
	    *  second test - to ensure that methods accessed via get+ construction
	    *  also work
	    */

	    template = "$woogie.foo boing!";

	    try
	    {
		Velocity.evaluate(vc, w, "test", template)
		;
		fail("No exception thrown, second test.");
	    }
	    catch (MethodInvocationException mie) {
		System.Console.Out.WriteLine("Caught MIE (good!) :");
		System.Console.Out.WriteLine("  reference = " + mie.ReferenceName);
		System.Console.Out.WriteLine("  method    = " + mie.MethodName);

		//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
		System.Exception t = mie.WrappedThrowable;
		System.Console.Out.WriteLine("  throwable = " + t);

		if (t is System.Exception) {
		    System.Console.Out.WriteLine("  exception = " + ((System.Exception) t).Message);
		}
	    } catch (System.Exception e) {
		fail("Wrong exception thrown, second test");
	    }

	    template = "$woogie.Foo boing!";

	    try
	    {
		Velocity.evaluate(vc, w, "test", template)
		;
		fail("No exception thrown, third test.");
	    }
	    catch (MethodInvocationException mie) {
		System.Console.Out.WriteLine("Caught MIE (good!) :");
		System.Console.Out.WriteLine("  reference = " + mie.ReferenceName);
		System.Console.Out.WriteLine("  method    = " + mie.MethodName);

		//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
		System.Exception t = mie.WrappedThrowable;
		System.Console.Out.WriteLine("  throwable = " + t);

		if (t is System.Exception) {
		    System.Console.Out.WriteLine("  exception = " + ((System.Exception) t).Message);
		}
	    } catch (System.Exception e) {
		fail("Wrong exception thrown, third test");
	    }

	    template = "#set($woogie.foo = 'lala') boing!";

	    try
	    {
		Velocity.evaluate(vc, w, "test", template)
		;
		fail("No exception thrown, set test.");
	    }
	    catch (MethodInvocationException mie) {
		System.Console.Out.WriteLine("Caught MIE (good!) :");
		System.Console.Out.WriteLine("  reference = " + mie.ReferenceName);
		System.Console.Out.WriteLine("  method    = " + mie.MethodName);

		//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
		System.Exception t = mie.WrappedThrowable;
		System.Console.Out.WriteLine("  throwable = " + t);

		if (t is System.Exception) {
		    System.Console.Out.WriteLine("  exception = " + ((System.Exception) t).Message);
		}
	    } catch (System.Exception e) {
		fail("Wrong exception thrown, set test");
	    }
	}

	public virtual void  doException() {
	    throw new System.NullReferenceException();
	}

	public virtual void  getFoo() {
	    throw new System.Exception("Hello from getFoo()");
	}

    }
}
