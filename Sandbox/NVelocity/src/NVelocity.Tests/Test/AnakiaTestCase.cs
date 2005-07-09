using System;

namespace org.apache.velocity.test {

    /// <summary>
    /// This is a test case for Anakia. Right now, it simply will compare
    /// two index.html files together. These are produced as a result of
    /// first running Anakia and then running this test.
    /// </summary>
    /// <author><a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
    public class AnakiaTestCase:BaseTestCase {
	private const System.String COMPARE_DIR = "../test/anakia/compare";
	private const System.String RESULTS_DIR = "../test/anakia/results";
	private const System.String FILE_EXT = ".html";

	/// <summary> Creates a new instance.
	/// *
	/// </summary>
	public AnakiaTestCase():base("AnakiaTestCase") {}

	/// <summary> Runs the test. This is empty on purpose because the
	/// code to do the Anakia output is in the .xml file that runs
	/// this test.
	/// </summary>
	public virtual void  runTest() {
	    try {
		assureResultsDirectoryExists(RESULTS_DIR);

		if (!isMatch(RESULTS_DIR, COMPARE_DIR, "index", FILE_EXT, FILE_EXT)) {
		    fail("Output is incorrect!");
		} else {
		    System.Console.Out.WriteLine("Passed!");
		}
	    } catch (System.Exception e) {
		/*
		* do nothing.
		*/
	    }
	}
    }
}
