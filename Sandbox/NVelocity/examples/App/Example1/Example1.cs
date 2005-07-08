using System;
using System.Collections;

using Velocity = NVelocity.App.Velocity;
using VelocityContext = NVelocity.VelocityContext;
using Template = NVelocity.Template;
using ParseErrorException = NVelocity.Exception.ParseErrorException;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;

/// <summary>
/// This class is a simple demonstration of how the NVelocity Template Engine
/// can be used in a standalone application.
/// </summary>
/// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
/// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
public class Example1 {

    public virtual ArrayList Names {
	get {
	    ArrayList list = new ArrayList();

	    list.Add("ArrayList element 1");
	    list.Add("ArrayList element 2");
	    list.Add("ArrayList element 3");
	    list.Add("ArrayList element 4");

	    return list;
	}
    }

    public Example1(System.String templateFile) {
	try {
	    /*
	    * setup
	    */
	    Velocity.Init("nvelocity.properties");

	    /*
	    *  Make a context object and populate with the data.  This 
	    *  is where the Velocity engine gets the data to resolve the
	    *  references (ex. $list) in the template
	    */
	    VelocityContext context = new VelocityContext();
	    context.Put("list", Names);

	    /*
	    *  get the Template object.  This is the parsed version of your 
	    *  template input file.  Note that getTemplate() can throw
	    *   ResourceNotFoundException : if it doesn't find the template
	    *   ParseErrorException : if there is something wrong with the VTL
	    *   Exception : if something else goes wrong (this is generally
	    *        indicative of as serious problem...)
	    */
	    Template template = null;

	    try {
		template = Velocity.GetTemplate(templateFile)
		;
	    } catch (ResourceNotFoundException) {
		System.Console.Out.WriteLine("Example1 : error : cannot find template " + templateFile);
	    } catch (ParseErrorException pee) {
		System.Console.Out.WriteLine("Example1 : Syntax error in template " + templateFile + ":" + pee);
	    }

	    /*
	    *  Now have the template engine process your template using the
	    *  data placed into the context.  Think of it as a  'merge' 
	    *  of the template and the data to produce the output stream.
	    */
	    if (template != null) {
		template.Merge(context, System.Console.Out);
	    }
	} catch (System.Exception e) {
	    System.Console.Out.WriteLine(e);
	}
    }


    [STAThread]
    public static void  Main(System.String[] args) {
	if (args.Length > 0) {
	    Example1 t = new Example1(args[0]);
	} else {
	    Console.Out.WriteLine("usage: example1.exe <template>");
	}
    }
}
