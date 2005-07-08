using System;
using System.IO;
using Velocity = NVelocity.App.Velocity;
using VelocityContext = NVelocity.VelocityContext;
using ParseErrorException = NVelocity.Exception.ParseErrorException;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;

/// <summary>
/// <p>This class is a simple demonstration of how the NVelocity Template Engine
/// can be used in a standalone application using the Velocity utility class.</p>
/// <p>It demonstrates two of the 'helper' methods found in the Velocity
/// class, MergeTemplate() and Evaluate().</p>
/// </summary>
/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
public class Example2 {

    [STAThread]
    public static void  Main(System.String[] args) {
	// first, we init the runtime engine.  Defaults are fine.
	try {
	    Velocity.Init();
	} catch (System.Exception e) {
	    System.Console.Out.WriteLine("Problem initializing Velocity : " + e);
	    return;
	}

	// lets make a Context and put data into it
	VelocityContext context = new VelocityContext();
	context.Put("name", "Velocity");
	context.Put("project", "Jakarta");

	// lets render a template
	StringWriter writer = new StringWriter();
	try {
	    Velocity.MergeTemplate("example2.vm", context, writer);
	} catch (System.Exception e) {
	    System.Console.Out.WriteLine("Problem merging template : " + e);
	}

	System.Console.Out.WriteLine(" template : " + writer.GetStringBuilder().ToString());

	// lets dynamically 'create' our template
	// and use the evaluate() method to render it
	System.String s = "We are using $project $name to render this.";
	writer = new StringWriter();
	try {
	    Velocity.Evaluate(context, writer, "mystring", s);
	} catch (ParseErrorException pee) {
	    // thrown if something is wrong with the
	    // syntax of our template string
	    System.Console.Out.WriteLine("ParseErrorException : " + pee);
	} catch (MethodInvocationException mee) {
	    // thrown if a method of a reference
	    // called by the template
	    // throws an exception. That won't happen here
	    // as we aren't calling any methods in this
	    // example, but we have to catch them anyway
	    System.Console.Out.WriteLine("MethodInvocationException : " + mee);
	} catch (System.Exception e) {
	    System.Console.Out.WriteLine("Exception : " + e);
	}

	System.Console.Out.WriteLine(" string : " + writer.GetStringBuilder().ToString());
    }
}
