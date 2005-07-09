namespace NVelocity.Test.Dvsl
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.Dvsl;

	/// <summary>
	/// Tests some Xpath peculiarities
	/// </summary>
	/// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
	[TestFixture]
	public class XPathTest
	{
		/// <summary>
		/// ensure we can match CDATA sections
		/// </summary>
		[Test]
		public virtual void testCDATA()
		{
			String dvslstyle = "#match(\"text()\")$node.value()#end";
			String input = "<?xml version=\"1.0\"?><document><![CDATA[Hello from CDATA]]></document>";
			Dvsl dvsl = new Dvsl();

			/*
	    *  register the stylesheet
	    */

			dvsl.SetStylesheet(new StringReader(dvslstyle));

			/*
	    *  render the document as a Reader
	    */

			StringWriter sw = new StringWriter();

			dvsl.Transform(new StringReader(input), sw);

			Assertion.Assert("First Test : " + sw.ToString(), sw.ToString().Equals("Hello from CDATA"));
		}

		/// <summary>
		/// ensure that the Union is working
		/// </summary>
		[Test]
		public virtual void testUNION()
		{
			String dvslstyle = "#match(\"p | document \")Matched#end";
			String input = "<?xml version=\"1.0\"?><document>document</document>";
			Dvsl dvsl = new Dvsl();

			/*
	    *  register the stylesheet
	    */

			dvsl.SetStylesheet(new StringReader(dvslstyle));

			/*
	    *  render the document as a Reader
	    */

			StringWriter sw = new StringWriter();

			dvsl.Transform(new StringReader(input), sw);

			Assertion.Assert("First Test : " + sw.ToString(), sw.ToString().Equals("Matched"));
		}

	}
}