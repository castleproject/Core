using System;
using System.Collections;
using Commons.Collections;

using NUnit.Framework;

namespace NVelocity.Test {

    /// <summary>
    /// Tests for the Commons ExtendedProperties class. This is an identical
    /// copy of the ConfigurationTestCase, which will disappear when
    /// the Configuration class does
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    [TestFixture]
    public class CommonsExtPropTestCase:BaseTestCase {
	/// <summary> Comparison directory.
	/// </summary>
	private const System.String COMPARE_DIR = "../test/configuration/compare";

	/// <summary> Results directory.
	/// </summary>
	private const System.String RESULTS_DIR = "../test/configuration/results";

	/// <summary> Test configuration
	/// </summary>
	private const System.String TEST_CONFIG = "../test/configuration/test.config";


	/// <summary>
	/// Runs the test.
	/// </summary>
	[Test]
	public virtual void  Test_run() {
	    System.IO.StreamWriter result = null;
	    ExtendedProperties c = null;
	    try {
		assureResultsDirectoryExists(RESULTS_DIR);
		c = new ExtendedProperties(TEST_CONFIG);
		result = new System.IO.StreamWriter(getFileName(RESULTS_DIR, "output", "res"));
	    } catch (System.Exception e) {
		throw new System.Exception("Cannot setup CommonsExtPropTestCase!", e);
	    }

	    message(result, "Testing order of keys ...");
	    showIterator(result, c.Keys);

	    message(result, "Testing retrieval of CSV values ...");
	    showVector(result, c.GetVector("resource.loader"));

	    message(result, "Testing subset(prefix).getKeys() ...");
	    ExtendedProperties subset = c.Subset("file.resource.loader");
	    showIterator(result, subset.Keys);

	    message(result, "Testing getVector(prefix) ...");
	    showVector(result, subset.GetVector("path"));

	    message(result, "Testing getString(key) ...");
	    result.Write(c.GetString("config.string.value"));
	    result.Write("\n\n");

	    message(result, "Testing getBoolean(key) ...");
	    result.Write(c.GetBoolean("config.boolean.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getByte(key) ...");
	    result.Write(c.GetByte("config.byte.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getShort(key) ...");
	    result.Write(c.GetShort("config.short.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getInt(key) ...");
	    result.Write(c.GetInt("config.int.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getLong(key) ...");
	    result.Write(c.GetLong("config.long.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getFloat(key) ...");
	    result.Write(c.GetFloat("config.float.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing getDouble(key) ...");
	    result.Write(c.GetDouble("config.double.value").ToString());
	    result.Write("\n\n");

	    message(result, "Testing escaped-comma scalar...");
	    result.Write(c.GetString("escape.comma1"));
	    result.Write("\n\n");

	    message(result, "Testing escaped-comma vector...");
	    showVector(result, c.GetVector("escape.comma2"));
	    result.Write("\n\n");

	    result.Flush();
	    result.Close();

	    if (!isMatch(RESULTS_DIR, COMPARE_DIR, "output", "res", "cmp")) {
		Assertion.Fail("Output incorrect.");
	    }
	}

	private void  showIterator(System.IO.StreamWriter result, IEnumerator i) {
	    while (i.MoveNext()) {
		result.Write((System.String) i.Current);
		result.Write("\n");
	    }
	    result.Write("\n");
	}

	private void  showVector(System.IO.StreamWriter result, System.Collections.ArrayList v) {
	    for (int j = 0; j < v.Count; j++) {
		result.WriteLine((System.String) v[j]);
	    }
	    result.Write("\n");
	}

	private void  message(System.IO.StreamWriter result, System.String message) {
	    result.WriteLine("--------------------------------------------------");
	    result.WriteLine(message);
	    result.WriteLine("--------------------------------------------------");
	    result.WriteLine("");
	}
    }
}
