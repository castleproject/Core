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

    /// <summary>  Tests event handling
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: EventHandlingTestCase.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class EventHandlingTestCase:TestCase, ReferenceInsertionEventHandler, NullSetEventHandler, MethodExceptionEventHandler, LogSystem {

	private System.String logString = null;
	private bool exceptionSwitch = true;
	private static System.String NO_REFERENCE_VALUE = "<no reference value>";
	private static System.String REFERENCE_VALUE = "<reference value>";

	/// <summary> Default constructor.
	/// </summary>
	public EventHandlingTestCase():base("EventHandlingTestCase") {

	    try {
		/*
		*  use an alternative logger.  Set it up here and pass it in.
		*/

		Velocity.setProperty(Velocity.RUNTIME_LOG_LOGSYSTEM, this);
		Velocity.init();
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup event handling test : " + e);
		System.Environment.Exit(1);
	    }
	}

	public virtual void  init(RuntimeServices rs) {
	    /* don't need it...*/
	}

	/// <summary> Runs the test.
	/// </summary>
	public virtual void  runTest() {

	    /*
	    *  lets make a Context and add the event cartridge
	    */

	    VelocityContext inner = new VelocityContext();

	    /*
	    *  Now make an event cartridge, register all the 
	    *  event handlers (at once) and attach it to the
	    *  Context
	    */

	    EventCartridge ec = new EventCartridge();
	    ec.addEventHandler(this);
	    ec.attachToContext(inner);

	    /*
	    *  now wrap the event cartridge - we want to make sure that
	    *  we can do this w/o harm
	    */

	    VelocityContext context = new VelocityContext(inner);

	    context.put("name", "Velocity");

	    try {
		/*
		*  First, the reference insertion handler
		*/

		System.String s = "$name";

		System.IO.StringWriter w = new System.IO.StringWriter();
		Velocity.evaluate(context, w, "mystring", s);

		if (!w.ToString().Equals(REFERENCE_VALUE)) {
		    fail("Reference insertion test 1");
		}

		/*
		*  using the same handler, we can deal with 
		*  null references as well
		*/

		s = "$floobie";

		w = new System.IO.StringWriter();
		Velocity.evaluate(context, w, "mystring", s);

		if (!w.ToString().Equals(NO_REFERENCE_VALUE)) {
		    fail("Reference insertion test 2");
		}

		/*
		*  now lets test setting a null value - this test
		*  should result in *no* log output.
		*/

		s = "#set($settest = $NotAReference)";
		w = new System.IO.StringWriter();
		logString = null;
		Velocity.evaluate(context, w, "mystring", s);

		if (logString != null) {
		    fail("NullSetEventHandler test 1");
		}

		/*
		*  now lets test setting a null value - this test
		*  should result in log output.
		*/

		s = "#set($logthis = $NotAReference)";
		w = new System.IO.StringWriter();
		logString = null;
		Velocity.evaluate(context, w, "mystring", s);

		if (logString == null) {
		    fail("NullSetEventHandler test 1");
		}

		/*
		*  finally, we test a method exception event - we do this 
		*  by putting this class in the context, and calling 
		*  a method that does nothing but throw an exception.
		*  we use a little switch to turn the event handling
		*  on and off
		*
		*  Note also how the reference insertion process
		*  happens as well
		*/

		exceptionSwitch = true;

		context.put("this", this);

		s = " $this.throwException()";
		w = new System.IO.StringWriter();

		try {
		    Velocity.evaluate(context, w, "mystring", s);
		} catch (MethodInvocationException mee) {
		    fail("MethodExceptionEvent test 1");
		} catch (System.Exception e) {
		    fail("MethodExceptionEvent test 1");
		}

		/*
		*  now, we turn the switch off, and we can see that the 
		*  exception will propgate all the way up here, and 
		*  wil be caught by the catch() block below
		*/

		exceptionSwitch = false;

		s = " $this.throwException()";
		w = new System.IO.StringWriter();

		try {
		    Velocity.evaluate(context, w, "mystring", s);
		    fail("MethodExceptionEvent test 2");
		} catch (MethodInvocationException mee) {
		    /*
		    * correct - should land here...
		    */
		} catch (System.Exception e) {
		    fail("MethodExceptionEvent test 2");
		}
	    } catch (ParseErrorException pee) {
		fail("ParseErrorException" + pee);
	    } catch (MethodInvocationException mee) {
		fail("MethodInvocationException" + mee);
	    } catch (System.Exception e) {
		fail("Exception" + e);
	    }
	}

	/// <summary>  silly method to throw an exception to test
	/// the method invocation exception event handling
	/// </summary>
	public virtual void  throwException() {
	    throw new System.Exception("Hello from throwException()");
	}

	/// <summary>  Event handler for when a reference is inserted into the output stream.
	/// </summary>
	public virtual System.Object referenceInsert(System.String reference, System.Object value_Renamed) {
	    /*
	    *  if we have a value
	    *  return a known value
	    */

	    System.String s = null;

	    if (value_Renamed != null) {
		s = REFERENCE_VALUE;
	    } else {
		/*
		* we only want to deal with $floobie - anything
		*  else we let go
		*/
		if (reference.Equals("$floobie")) {
		    s = NO_REFERENCE_VALUE;
		}
	    }
	    return s;
	}

	/// <summary>  Event handler for when the right hand side of
	/// a #set() directive is null, which results in
	/// a log message.  This method gives the application
	/// a chance to 'vote' on msg generation
	/// </summary>
	public virtual bool shouldLogOnNullSet(System.String lhs, System.String rhs) {
	    if (lhs.Equals("$settest"))
		return false;

	    return true;
	}

	/// <summary>  Handles exceptions thrown during in-template method access
	/// </summary>
	public virtual System.Object methodException(System.Type claz, System.String method, System.Exception e) {
	    /*
	    *  only do processing if the switch is on
	    */

	    if (exceptionSwitch && method.Equals("throwException")) {
		return "handler";
	    }

	    throw e;
	}

	/// <summary>  handler for LogSystem interface
	/// </summary>
	public virtual void  logVelocityMessage(int level, System.String message) {
	    logString = message;
	}
    }
}
