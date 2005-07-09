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
    using System.Collections;

    /// <summary> Tests for the Configuration class.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version> $Id: ConfigurationTestCase.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    /// *
    /// </version>
    /// <deprecated>Will be removed when Configuration class is removed
    ///
    /// </deprecated>
    public class ConfigurationTestCase:BaseTestCase {
	/// <summary> Comparison directory.
	/// </summary>
	private const System.String COMPARE_DIR = "../test/configuration/compare";

	/// <summary> Results directory.
	/// </summary>
	private const System.String RESULTS_DIR = "../test/configuration/results";

	/// <summary> Test configuration
	/// </summary>
	private const System.String TEST_CONFIG = "../test/configuration/test.config";

	/// <summary> Creates a new instance.
	/// *
	/// </summary>
	public ConfigurationTestCase():base("ConfigurationTestCase") {}

	/// <summary> Runs the test.
	/// </summary>
	public virtual void  runTest() {
	    try {
		assureResultsDirectoryExists(RESULTS_DIR);

		Configuration c = new Configuration(TEST_CONFIG);

		System.IO.StreamWriter result = new System.IO.StreamWriter(getFileName(RESULTS_DIR, "output", "res"));

		message(result, "Testing order of keys ...");
		showIterator(result, c.Keys);

		message(result, "Testing retrieval of CSV values ...");
		showVector(result, c.getVector("resource.loader"));

		message(result, "Testing subset(prefix).getKeys() ...");
		Configuration subset = c.subset("file.resource.loader");
		showIterator(result, subset.Keys);

		message(result, "Testing getVector(prefix) ...");
		showVector(result, subset.getVector("path"));

		message(result, "Testing getString(key) ...");
		result.write(c.getString("config.string.value"));
		result.Write("\n\n");

		message(result, "Testing getBoolean(key) ...");
		//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Boolean.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
		result.Write(new Boolean(c.getBoolean("config.boolean.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getByte(key) ...");
		result.Write(new Byte(c.getByte("config.byte.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getShort(key) ...");
		result.Write(new Short(c.getShort("config.short.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getInt(key) ...");
		result.Write(new Integer(c.getInt("config.int.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getLong(key) ...");
		result.Write(new Long(c.getLong("config.long.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getFloat(key) ...");
		result.Write(new Float(c.getFloat("config.float.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing getDouble(key) ...");
		result.Write(new Double(c.getDouble("config.double.value")).ToString());
		result.Write("\n\n");

		message(result, "Testing escaped-comma scalar...");
		result.write(c.getString("escape.comma1"));
		result.Write("\n\n");

		message(result, "Testing escaped-comma vector...");
		showVector(result, c.getVector("escape.comma2"));
		result.Write("\n\n");

		result.Flush();
		result.Close();

		if (!isMatch(RESULTS_DIR, COMPARE_DIR, "output", "res", "cmp")) {
		    fail("Output incorrect.");
		}
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup ConfigurationTestCase!");
		SupportClass.WriteStackTrace(e, Console.Error);
		System.Environment.Exit(1);
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
		//UPGRADE_TODO: Method java.util.Vector.get was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1095"'
		result.Write((System.String) v.get(j));
		result.Write("\n");
	    }
	    result.Write("\n");
	}

	private void  message(System.IO.StreamWriter result, System.String message) {
	    result.Write("--------------------------------------------------\n");
	    result.Write(message + "\n");
	    result.Write("--------------------------------------------------\n");
	    result.Write("\n");
	}
    }
}
