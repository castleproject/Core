using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Commons.Collections;
using NVelocity.Util.Introspection;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime;
using NVelocity.App;
using NVelocity.Context;
using NVelocity;
using NVelocity.Exception;

namespace NVelocity.Test {

    /// <summary>
    /// Test Velocity Introspector
    /// </summary>
    [TestFixture]
    public class ParserTest {

	[Test]
	public void Test_VelocityCharStream() {
	    String s1 = "this is a test";
	    VelocityCharStream vcs = new VelocityCharStream(new StringReader(s1), 1, 1);

	    String s2 = String.Empty;
	    try {
		Char c = vcs.readChar();
		while (true) {
		    s2 += c;
		    c = vcs.readChar();
		}
	    } catch (System.IO.IOException) {
		// this is expected to happen when the stream has been read
	    }
	    Assertion.Assert("read stream did not match source string", s1.Equals(s2));
	}

	[Test]
	public void Test_Parse() {
	    VelocityCharStream vcs = GetTemplateStream();
	    Parser p = new Parser(vcs);

	    SimpleNode root = p.process();

	    String javaNodes="19,18,9,5,23,56,23,42,23,24,6,18,56,18,9,5,23,56,23,42,23,25,6,18,44,23,5,56,6,18,46,18,43,0";
	    String nodes = String.Empty;

	    if (root != null) {
		Token t = root.FirstToken;
		nodes += t.kind.ToString();
		while (t != root.LastToken) {
		    t = t.next;
		    nodes += "," + t.kind.ToString();
		}
	    }

	    if (!javaNodes.Equals(nodes)) {
		Console.Out.WriteLine("");
		Console.Out.WriteLine(".Net parsed nodes did not match java nodes.");
		Console.Out.WriteLine("java=" + javaNodes);
		Console.Out.WriteLine(".net=" + nodes);
		Assertion.Fail(".Net parsed nodes did not match java nodes.");
	    }
	}


	[Test]
	[Ignore("TODO: running this as well as Example1 will cause the runtime instance to load the properties file twice")]
	public void Test_RuntimeServices() {
	    RuntimeInstance ri = RuntimeSingleton.RuntimeInstance;
	    try {
		ri.init();
	    } catch(System.Exception ex) {
		Console.Out.WriteLine(ex.Message);
		Assertion.Fail();
	    }
	    ExtendedProperties ep = ri.Configuration;
	}

	[Test]
	public void Test_Example1() {
	    String templateFile = "example1.vm";
	    try {
		/*
		 * setup
		 */

		Velocity.SetProperty(NVelocity.Runtime.RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, "../test/templates");
		Velocity.Init();

		/*
		 *  Make a context object and populate with the data.  This 
		 *  is where the Velocity engine gets the data to resolve the
		 *  references (ex. $list) in the template
		 */
		VelocityContext context = new VelocityContext();
		context.Put("list", GetNames());

		ExtendedProperties props = new ExtendedProperties();
		props.Add("runtime.log", "nvelocity.log");
		context.Put("props", props);

		/*
		 *    get the Template object.  This is the parsed version of your 
		 *  template input file.  Note that getTemplate() can throw
		 *   ResourceNotFoundException : if it doesn't find the template
		 *   ParseErrorException : if there is something wrong with the VTL
		 *   Exception : if something else goes wrong (this is generally
		 *        indicative of as serious problem...)
		 */
		Template template =  null;

		try {
		    template = Velocity.GetTemplate(templateFile)
		    ;
		} catch( ResourceNotFoundException rnfe ) {
		    Console.Out.WriteLine("Example : error : cannot find template " + templateFile + " : \n" + rnfe.Message);
		    Assertion.Fail();
		} catch( ParseErrorException pee ) {
		    Console.Out.WriteLine("Example : Syntax error in template " + templateFile + " : \n" + pee );
		    Assertion.Fail();
		}

		/*
		 *  Now have the template engine process your template using the
		 *  data placed into the context.  Think of it as a  'merge' 
		 *  of the template and the data to produce the output stream.
		 */

		// using Console.Out will send it to the screen
		TextWriter writer = new StringWriter();
		if ( template != null)
		    template.Merge(context, writer);

		/*
		 *  flush and cleanup
		 */

		writer.Flush();
		writer.Close();
	    } catch(System.Exception ex) {
		Console.Out.WriteLine(ex);
		Assertion.Fail();
	    }
	}

	private ArrayList GetNames() {
	    ArrayList list = new ArrayList();

	    list.Add("Billy");
	    list.Add("Bob");
	    list.Add("Jane");
	    list.Add("Sarah");

	    return list;
	}

	private VelocityCharStream GetTemplateStream() {
	    StringBuilder sb = new StringBuilder();
	    sb.Append("## This is an example velocity template\n");
	    sb.Append("\n");
	    sb.Append("#set( $this = \"Velocity\")\n");
	    sb.Append("\n");
	    sb.Append("$this is great!\n");
	    sb.Append("\n");
	    //TODO: parse fails for this segment for unknown reason
	    //	    sb.Append("#foreach( $name in $list )\n");
	    //	    sb.Append("row $velocityCount :: $name is great!\n");
	    //	    sb.Append("#end\n");
	    //	    sb.Append("\n");
	    sb.Append("#set( $condition = true)\n");
	    sb.Append("\n");
	    sb.Append("#if ($condition)\n");
	    sb.Append("    The condition is true!\n");
	    sb.Append("#else\n");
	    sb.Append("    The condition is false!\n");
	    sb.Append("#end\n");

	    VelocityCharStream vcs = new VelocityCharStream(new StringReader(sb.ToString()), 1, 1);
	    return vcs;
	}

    }
}
