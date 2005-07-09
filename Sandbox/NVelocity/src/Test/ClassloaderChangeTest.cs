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
    /// <version> $Id: ClassloaderChangeTest.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class ClassloaderChangeTest:TestCase, LogSystem {
	private VelocityEngine ve = null;
	private bool sawCacheDump = false;

	private static System.String OUTPUT = "Hello From Foo";


	/// <summary> Default constructor.
	/// </summary>
	public ClassloaderChangeTest():base("ClassloaderChangeTest") {

	    try {
		/*
		*  use an alternative logger.  Set it up here and pass it in.
		*/

		ve = new VelocityEngine();
		ve.setProperty(VelocityEngine.RUNTIME_LOG_LOGSYSTEM, this);
		ve.init();
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup ClassloaderChnageTest : " + e);
		System.Environment.Exit(1);
	    }
	}

	public virtual void  init(RuntimeServices rs) {
	    // do nothing with it
	}

	/// <summary> Runs the test.
	/// </summary>
	public virtual void  runTest() {
	    sawCacheDump = false;

	    try {
		VelocityContext vc = new VelocityContext();
		System.Object foo = null;

		/*
		*  first, we need a classloader to make our foo object
		*/

		TestClassloader cl = new TestClassloader();
		//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.loadClass' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader"'
		System.Type fooclass = cl.loadClass("Foo");
		System.Type temp_Class;
		temp_Class = fooclass;
		foo = temp_Class;

		/*
		*  put it into the context
		*/
		vc.put("foo", foo);

		/*
		*  and render something that would use it
		*  that will get it into the introspector cache
		*/
		System.IO.StringWriter writer = new System.IO.StringWriter();
		ve.evaluate(vc, writer, "test", "$foo.doIt()");

		/*
		*  Check to make sure ok.  note the obvious
		*  dependency on the Foo class...
		*/

		if (!writer.ToString().Equals(OUTPUT)) {
		    fail("Output from doIt() incorrect");
		}

		/*
		* and do it again :)
		*/
		cl = new TestClassloader();
		//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.loadClass' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader"'
		fooclass = cl.loadClass("Foo");
		System.Type temp_Class2;
		temp_Class2 = fooclass;
		foo = temp_Class2;

		vc.put("foo", foo);

		writer = new System.IO.StringWriter();
		ve.evaluate(vc, writer, "test", "$foo.doIt()");

		if (!writer.ToString().Equals(OUTPUT)) {
		    fail("Output from doIt() incorrect");
		}
	    } catch (System.Exception ee) {
		System.Console.Out.WriteLine("ClassloaderChangeTest : " + ee);
	    }

	    if (!sawCacheDump) {
		fail("Didn't see introspector cache dump.");
	    }
	}

	/// <summary>  method to catch Velocity log messages.  When we
	/// see the introspector dump message, then set the flag
	/// </summary>
	public virtual void  logVelocityMessage(int level, System.String message) {
	    if (message.equals(Introspector.CACHEDUMP_MSG)) {
		sawCacheDump = true;
	    }
	}
    }

    /// <summary>  Simple (real simple...) classloader that depends
    /// on a Foo.class being located in the classloader
    /// directory under test
    /// </summary>
    //UPGRADE_ISSUE: Class 'java.lang.ClassLoader' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader"'
    class TestClassloader:ClassLoader {
	private const System.String testclass = "../test/classloader/Foo.class";

	private System.Type fooClass = null;

	public TestClassloader() {
	    try {
		System.IO.FileInfo f = new System.IO.FileInfo(testclass);

		sbyte[] barr = new sbyte[(int) SupportClass.FileLength(f)];

		System.IO.FileStream fis = new System.IO.FileStream(f.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
		//UPGRADE_TODO: Equivalent of method 'java.io.FileInputStream.read' may not have an optimal performance in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1117"'
		SupportClass.ReadInput(fis, ref barr, 0, barr.Length);
		fis.Close();

		//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.defineClass' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader"'
		fooClass = defineClass("Foo", barr, 0, barr.Length);
	    } catch (System.Exception e) {
		System.Console.Out.WriteLine("TestClassloader : exception : " + e);
	    }
	}


	public virtual System.Type findClass(System.String name) {
	    return fooClass;
	}
    }
}
