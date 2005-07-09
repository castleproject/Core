using System;
using TestProvider = NVelocity.Test.Provider.TestProvider;
using NVelocity.App;
using NVelocity;

using NUnit.Framework;

namespace NVelocity.Test {

    /// <summary>
    /// This class is intended to test the App.Velocity class.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    [TestFixture]
    public class VelocityAppTestCase : BaseTestCase {
	private System.IO.StringWriter compare1 = new System.IO.StringWriter();
	private System.String input1 = "My name is $name -> $Floog";
	private System.String result1 = "My name is jason -> floogie woogie";

	public VelocityAppTestCase() {
	    try {
		Velocity.SetProperty(NVelocity.Runtime.RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, NVelocity.Test.TemplateTestBase_Fields.FILE_RESOURCE_LOADER_PATH);
		Velocity.Init();
	    } catch (System.Exception e) {
		throw new System.Exception("Cannot setup VelocityAppTestCase!", e);
	    }
	}

	/// <summary>
	/// Runs the test.
	/// </summary>
	[Test]
	public virtual void Test_Run() {
	    VelocityContext context = new VelocityContext();
	    context.Put("name", "jason");
	    context.Put("Floog", "floogie woogie");

	    Velocity.Evaluate(context, compare1, "evaltest", input1);

	    /*
	    FIXME: Not tested right now.

	    StringWriter result2 = new StringWriter();
	    Velocity.mergeTemplate("mergethis.vm",  context, result2);

	    StringWriter result3 = new StringWriter();
	    Velocity.invokeVelocimacro("floog", "test", new String[2], 
	    context, result3);*/

	    if (!result1.Equals(compare1.ToString())) {
		Assertion.Fail("Output incorrect.");
	    }
	}

    }
}
