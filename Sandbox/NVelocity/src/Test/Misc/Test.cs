namespace org.apache.velocity.test.misc
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
    using TestProvider = org.apache.velocity.test.provider.TestProvider;


    /// <summary> This class the testbed for Velocity. It is used to
    /// test all the directives support by Velocity.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: Test.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class Test : ReferenceInsertionEventHandler, NullSetEventHandler, MethodExceptionEventHandler {
	/// <summary> Cache of writers
	/// </summary>
	private static System.Collections.Stack writerStack = new System.Collections.Stack();

	public Test(System.String templateFile, System.String encoding) {
	    System.IO.StreamWriter writer = null;
	    TestProvider provider = new TestProvider();
	    ArrayList al = provider.Customers;
	    System.Collections.Hashtable h = new System.Collections.Hashtable();

	    /*
	    *  put this in to test introspection $h.Bar or $h.get("Bar") etc
	    */

	    SupportClass.PutElement(h, "Bar", "this is from a hashtable!");
	    SupportClass.PutElement(h, "Foo", "this is from a hashtable too!");

	    /*
	    *  adding simple vector with strings for testing late introspection stuff
	    */

	    System.Collections.ArrayList v = new System.Collections.ArrayList();

	    System.String str = "mystr";

	    v.Add(new System.String("hello".ToCharArray()));
	    v.Add(new System.String("hello2".ToCharArray()));
	    v.Add(str);

	    try {
		/*
		*  this is another way to do properties when initializing Runtime.
		*  make a Properties 
		*/

		//UPGRADE_TODO: Format of property file may need to be changed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1089"'
		System.Configuration.AppSettingsReader p = new System.Configuration.AppSettingsReader();

		/*
		*  now, if you want to, load it from a file (or whatever)
		*/

		try {
		    System.IO.FileStream fis = new System.IO.FileStream(new System.IO.FileInfo("velocity.properties").FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

		    if (fis != null) {
			//UPGRADE_ISSUE: Method 'java.util.Properties.load' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javautilPropertiesload_javaioInputStream"'
			p.load(fis);
		    }
		} catch (System.Exception ex) {
		    /* no worries. no file... */
		}

		/*
		*  iterate out the properties
		*/

		System.Collections.Specialized.NameValueCollection temp_namedvaluecollection;
		temp_namedvaluecollection = System.Configuration.ConfigurationSettings.AppSettings;
		//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
		for (System.Collections.IEnumerator e = temp_namedvaluecollection.GetEnumerator(); e.MoveNext(); ) {
		    //UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
		    System.String el = (System.String) e.Current;

		    //UPGRADE_WARNING: method 'java.util.Properties.getProperty' was converted to ' ' which may throw an exception. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1101"'
		    Velocity.setProperty(el, (System.String) p.GetValue(el, System.Type.GetType("System.String")));
		}

		/*
		*  add some individual properties if you wish
		*/


		Velocity.setProperty(Velocity.RUNTIME_LOG_ERROR_STACKTRACE, "true");
		Velocity.setProperty(Velocity.RUNTIME_LOG_WARN_STACKTRACE, "true");
		Velocity.setProperty(Velocity.RUNTIME_LOG_INFO_STACKTRACE, "true");

		/*
		*  use an alternative logger.  Set it up here and pass it in.
		*/

		//            SimpleLogSystem sls = new SimpleLogSystem("velocity_simple.log");

		// Velocity.setProperty(Velocity.RUNTIME_LOG_LOGSYSTEM, sls );

		/*
		*  and now call init
		*/

		Velocity.init();

		/*
		*  now, do what we want to do.  First, get the Template
		*/

		if (templateFile == null) {
		    templateFile = "examples/example.vm";
		}


		Template template = null;

		try
		{
		    template = RuntimeSingleton.getTemplate(templateFile, encoding)
		    ;
		}
		catch (ResourceNotFoundException rnfe) {
		    System.Console.Out.WriteLine("Test : RNFE : Cannot find template " + templateFile);
		} catch (ParseErrorException pee) {
		    System.Console.Out.WriteLine("Test : Syntax error in template " + templateFile + ":" + pee);
		}

		/*
		* now, make a Context object and populate it.
		*/

		VelocityContext context = new VelocityContext();

		context.put("provider", provider);
		context.put("name", "jason");
		context.put("providers", provider.Customers2);
		context.put("list", al);
		context.put("hashtable", h);
		context.put("search", provider.Search);
		context.put("relatedSearches", provider.RelSearches);
		context.put("searchResults", provider.RelSearches);
		context.put("menu", provider.Menu);
		context.put("stringarray", provider.Array);
		context.put("vector", v);
		context.put("mystring", new System.String("".ToCharArray()));
		context.put("hashmap", new HashMap());
		context.put("runtime", new FieldMethodizer("org.apache.velocity.runtime.RuntimeSingleton"));
		context.put("fmprov", new FieldMethodizer(provider));
		context.put("Floog", "floogie woogie");
		context.put("geirstring", str);
		context.put("mylong", 5);

		/*
		*  we want to make sure we test all types of iterative objects
		*  in #foreach()
		*/

		int[] intarr = new int[]{10, 20, 30, 40, 50};

		System.Object[] oarr = new System.Object[]{"a", "b", "c", "d"};

		context.put("collection", v);
		context.put("iterator", v.iterator());
		context.put("map", h);
		context.put("obarr", oarr);
		context.put("intarr", intarr);

		System.String stest = " My name is $name -> $Floog";
		System.IO.StringWriter w = new System.IO.StringWriter();
		//            Velocity.evaluate( context, w, "evaltest",stest );
		//            System.out.println("Eval = " + w );

		w = new System.IO.StringWriter();
		//Velocity.mergeTemplate( "mergethis.vm",  context, w );
		//System.out.println("Merge = " + w );

		w = new System.IO.StringWriter();
		//Velocity.invokeVelocimacro( "floog", "test", new String[2],  context,  w );
		//System.out.println("Invoke = " + w );


		/*
		*  event cartridge stuff
		*/

		EventCartridge ec = new EventCartridge();
		ec.addEventHandler(this);
		ec.attachToContext(context);

		/*
		*  make a writer, and merge the template 'against' the context
		*/

		VelocityContext vc = new VelocityContext(context);

		if (template != null) {
		    //UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		    writer = new BufferedWriter(new System.IO.StreamWriter(System.Console.Out));
		    template.merge(vc, writer);
		    writer.Flush();
		    writer.Close();
		}

	    } catch (MethodInvocationException mie) {
		System.Console.Out.WriteLine("MIE : " + mie);
	    } catch (System.Exception e) {
		RuntimeSingleton.error("Test- exception : " + e);
		SupportClass.WriteStackTrace(e, Console.Error);

	    }
	}

	public virtual System.Object referenceInsert(System.String reference, System.Object value_Renamed) {
	    if (value_Renamed != null) {}
	    // System.out.println("Woo! referenceInsert : " + reference + " = " + value.toString() );
	    return value_Renamed;
	}

	public virtual bool shouldLogOnNullSet(System.String lhs, System.String rhs) {
	    //        System.out.println("Woo2! nullSetLogMessage : " + lhs + " :  RHS = " + rhs);

	    if (lhs.Equals("$woogie"))
		return false;

	    return true;
	}

	public virtual System.Object methodException(System.Type claz, System.String method, System.Exception e) {
	    if (method.Equals("getThrow"))
		return "I should have thrown";

	    throw e;
	}


	[STAThread]
	public static void  Main(System.String[] args) {
	    Test t;

	    System.String encoding = "ISO-8859-1";

	    if (args.Length > 1)
		encoding = args[1];

	    t = new Test(args[0], encoding);
	}
    }
}
