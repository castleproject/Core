using System;
using System.IO;
using System.Xml;

using NUnit.Framework;

using NVelocity.Dvsl;

namespace NVelocity.Test.Dvsl {

    /// <summary>
    /// Simple testcase to ensure things are basically working.
    /// This tests both a serialized document as well as a 'live'
    /// dom4j one.
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org>Geir Magnusson Jr.</a></author>
    [TestFixture]
    public class TransformTest {

	/*
	* use simple in-memory style and input strings
	*/
	private String dvslstyle = "#match(\"element\")Hello from element! $node.value()#end";
	private String input = "<?xml version=\"1.0\"?><document><element>Foo</element></document>";

	[Test]
	public virtual void  testSelection() {
	    try {
		doit();
	    } catch (System.Exception e) {
		Assertion.Fail(e.Message);
	    }
	}

	public virtual void  doit() {
	    /*
	    * make a dvsl
	    */
	    NVelocity.Dvsl.Dvsl dvsl = new NVelocity.Dvsl.Dvsl();

	    /*
	    *  register the stylesheet
	    */
	    dvsl.SetStylesheet(new StringReader(dvslstyle));

	    /*
	    *  render the document as a Reader
	    */
	    StringWriter sw = new StringWriter();
	    dvsl.Transform(new StringReader(input), sw);

	    if (!sw.ToString().Equals("Hello from element! Foo"))
		Assertion.Fail("Result of first test is wrong : " + sw.ToString());

	    /*
	    * now test if we can pass it a Document
	    */
	    XmlDocument document = new XmlDocument();
	    document.Load(new StringReader(input));

	    sw = new StringWriter();
	    dvsl.Transform(document, sw);

	    if (!sw.ToString().Equals("Hello from element! Foo"))
		Assertion.Fail("Result of second test is wrong : " + sw.ToString());
	}

    }
}
