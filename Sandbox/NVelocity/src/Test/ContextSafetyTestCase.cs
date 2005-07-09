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
    using TestProvider = org.apache.velocity.test.provider.TestProvider;

    /// <summary> Tests if we are context safe : can we switch objects in the context
    /// and re-merge the template safely.
    /// *
    /// NOTE:
    /// This class should not extend RuntimeTestCase because this test
    /// is run from the VelocityTestSuite which in effect a runtime
    /// test suite and the test suite initializes the Runtime. Extending
    /// RuntimeTestCase causes the Runtime to be initialized twice.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: ContextSafetyTestCase.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class ContextSafetyTestCase:BaseTestCase, TemplateTestBase {
	public ContextSafetyTestCase():base("ContextSafetyTestCase") {

	    try {
		Velocity.setProperty(Velocity.FILE_RESOURCE_LOADER_PATH, org.apache.velocity.test.TemplateTestBase_Fields.FILE_RESOURCE_LOADER_PATH);

		Velocity.init();
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup ContextSafetyTestCase!");
		SupportClass.WriteStackTrace(e, Console.Error);
		System.Environment.Exit(1);
	    }
	}

	/// <summary> Runs the test.
	/// </summary>
	public virtual void  runTest() {
	    /*
	    *  make a Vector and String array because
	    *  they are treated differently in Foreach()
	    */
	    System.Collections.ArrayList v = new System.Collections.ArrayList();

	    v.Add(new System.String("vector hello 1".ToCharArray()));
	    v.Add(new System.String("vector hello 2".ToCharArray()));
	    v.Add(new System.String("vector hello 3".ToCharArray()));

	    System.String[] strArray = new System.String[3];

	    strArray[0] = "array hello 1";
	    strArray[1] = "array hello 2";
	    strArray[2] = "array hello 3";

	    VelocityContext context = new VelocityContext();

	    try {
		assureResultsDirectoryExists(org.apache.velocity.test.TemplateTestBase_Fields.RESULT_DIR);

		/*
		*  get the template and the output
		*/

		Template template = RuntimeSingleton.getTemplate(getFileName(null, "context_safety", org.apache.velocity.test.TemplateTestBase_Fields.TMPL_FILE_EXT))
		;

		System.IO.FileStream fos1 = new System.IO.FileStream(getFileName(org.apache.velocity.test.TemplateTestBase_Fields.RESULT_DIR, "context_safety1", org.apache.velocity.test.TemplateTestBase_Fields.RESULT_FILE_EXT), System.IO.FileMode.Create);

		System.IO.FileStream fos2 = new System.IO.FileStream(getFileName(org.apache.velocity.test.TemplateTestBase_Fields.RESULT_DIR, "context_safety2", org.apache.velocity.test.TemplateTestBase_Fields.RESULT_FILE_EXT), System.IO.FileMode.Create);

		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		System.IO.StreamWriter writer1 = new BufferedWriter(new System.IO.StreamWriter(fos1));
		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		System.IO.StreamWriter writer2 = new BufferedWriter(new System.IO.StreamWriter(fos2));

		/*
		*  put the Vector into the context, and merge
		*/

		context.put("vector", v);
		template.merge(context, writer1);
		writer1.Flush();
		writer1.Close();

		/*
		*  now put the string array into the context, and merge
		*/

		context.put("vector", strArray);
		template.merge(context, writer2);
		writer2.Flush();
		writer2.Close();

		if (!isMatch(org.apache.velocity.test.TemplateTestBase_Fields.RESULT_DIR, org.apache.velocity.test.TemplateTestBase_Fields.COMPARE_DIR, "context_safety1", org.apache.velocity.test.TemplateTestBase_Fields.RESULT_FILE_EXT, org.apache.velocity.test.TemplateTestBase_Fields.CMP_FILE_EXT) || !isMatch(org.apache.velocity.test.TemplateTestBase_Fields.RESULT_DIR, org.apache.velocity.test.TemplateTestBase_Fields.COMPARE_DIR, "context_safety2", org.apache.velocity.test.TemplateTestBase_Fields.RESULT_FILE_EXT, org.apache.velocity.test.TemplateTestBase_Fields.CMP_FILE_EXT)) {
		    fail("Output incorrect.");
		}
	    } catch (System.Exception e) {
		fail(e.Message);
	    }
	}
    }
}
