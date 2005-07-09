using System;
using NVelocity.Test.Provider;
using NVelocity;
using NVelocity.App;
using NUnit.Framework;
using System.IO;
using NVelocity.Runtime;


namespace NVelocity.Test {

    /// <summary>
    /// Tests input encoding handling.  The input target is UTF-8, having
    /// chinese and and a spanish enyay (n-twiddle)
    /// 
    /// Thanks to Kent Johnson for the example input file.
    /// </summary>
    /// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: EncodingTestCase.cs,v 1.4 2005/01/01 17:57:56 corts Exp $</version>
    public class EncodingTestCase:BaseTestCase { //, TemplateTestBase {

	public EncodingTestCase() {
	    try {
		Velocity.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, TemplateTestBase_Fields.FILE_RESOURCE_LOADER_PATH);
		Velocity.SetProperty(RuntimeConstants_Fields.INPUT_ENCODING, "UTF-8");
		Velocity.Init();
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup EncodingTestCase!");
		SupportClass.WriteStackTrace(e, Console.Error);
		System.Environment.Exit(1);
	    }
	}

	/// <summary>
	/// Runs the test.
	/// </summary>
	public virtual void RunTest() {
	    VelocityContext context = new VelocityContext();

	    assureResultsDirectoryExists(TemplateTestBase_Fields.RESULT_DIR);

	    /*
	    *  get the template and the output
	    */

	    /*
	    *  Chinese and spanish
	    */

	    Template template = Velocity.GetTemplate(getFileName(null, "encodingtest", TemplateTestBase_Fields.TMPL_FILE_EXT), "UTF-8");

	    FileStream fos = new FileStream(getFileName(TemplateTestBase_Fields.RESULT_DIR, "encodingtest", TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);

	    StreamWriter writer = new StreamWriter(fos);

	    template.Merge(context, writer);
	    writer.Flush();
	    writer.Close();

	    if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, "encodingtest", TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		Assertion.Fail("Output 1 incorrect.");
	    }

	    /*
	    *  a 'high-byte' chinese example from Michael Zhou
	    */

	    template = Velocity.GetTemplate(getFileName(null, "encodingtest2", TemplateTestBase_Fields.TMPL_FILE_EXT), "UTF-8");

	    fos = new FileStream(getFileName(TemplateTestBase_Fields.RESULT_DIR, "encodingtest2", TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);

	    writer = new StreamWriter(fos);

	    template.Merge(context, writer);
	    writer.Flush();
	    writer.Close();

	    if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, "encodingtest2", TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		Assertion.Fail("Output 2 incorrect.");
	    }

	    /*
	    *  a 'high-byte' chinese from Ilkka
	    */

	    template = Velocity.GetTemplate(getFileName(null, "encodingtest3", TemplateTestBase_Fields.TMPL_FILE_EXT), "GB18030"); //GBK=936?

	    fos = new FileStream(getFileName(TemplateTestBase_Fields.RESULT_DIR, "encodingtest3", TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);

	    writer = new StreamWriter(fos, System.Text.Encoding.GetEncoding("GB18030"));

	    template.Merge(context, writer);
	    writer.Flush();
	    writer.Close();

	    if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, "encodingtest3", TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		Assertion.Fail("Output 3 incorrect.");
	    }

	    /*
	    *  Russian example from Vitaly Repetenko
	    */

	    template = Velocity.GetTemplate(getFileName(null, "encodingtest_KOI8-R", TemplateTestBase_Fields.TMPL_FILE_EXT), "KOI8-R");
	    fos = new FileStream(getFileName(TemplateTestBase_Fields.RESULT_DIR, "encodingtest_KOI8-R", TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);
	    writer = new StreamWriter(fos, System.Text.Encoding.GetEncoding("KOI8-R"));

	    template.Merge(context, writer);
	    writer.Flush();
	    writer.Close();

	    if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, "encodingtest_KOI8-R", TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		Assertion.Fail("Output 4 incorrect.");
	    }

	
	    /*
	    *  ISO-8859-1 example from Mike Bridge
	    */

	    template = Velocity.GetTemplate(getFileName(null, "encodingtest_ISO-8859-1", TemplateTestBase_Fields.TMPL_FILE_EXT), "ISO-8859-1");
	    fos = new FileStream(getFileName(TemplateTestBase_Fields.RESULT_DIR, "encodingtest_ISO-8859-1", TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);
	    writer = new StreamWriter(fos, System.Text.Encoding.GetEncoding("ISO-8859-1"));

	    template.Merge(context, writer);
	    writer.Flush();
	    writer.Close();

	    if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, "encodingtest_ISO-8859-1", TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		Assertion.Fail("Output 5 incorrect.");
	    }
	}
    }
}
