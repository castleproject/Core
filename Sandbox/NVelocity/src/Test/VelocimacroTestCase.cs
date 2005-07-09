using System;
using TestProvider = NVelocity.Test.Provider.TestProvider;
using NVelocity.App;
using NVelocity;

using NUnit.Framework;

namespace NVelocity.Test {

    /// <summary>
    /// This class tests strange Velocimacro issues.
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    [TestFixture]
    public class VelocimacroTestCase {
	private System.String template1 = "#macro(fooz $a)$a#end#macro(bar $b)#fooz($b)#end#foreach($i in [1..3])#bar($i)#end";
	private System.String result1 = "123";

	public VelocimacroTestCase() {
	    try {
		/*
		*  setup local scope for templates
		*/
		Velocity.SetProperty(NVelocity.Runtime.RuntimeConstants_Fields.VM_PERM_INLINE_LOCAL, true);
		Velocity.Init();
	    } catch (System.Exception e) {
		throw new System.Exception("Cannot setup VelocimacroTestCase!");
	    }
	}

	/// <summary>
	/// Runs the test.
	/// </summary>
	[Test]
	public virtual void Test_Run() {
	    VelocityContext context = new VelocityContext();

	    System.IO.StringWriter writer = new System.IO.StringWriter();
	    Velocity.Evaluate(context, writer, "vm_chain1", template1);

	    System.String out_Renamed = writer.ToString();

	    if (!result1.Equals(out_Renamed)) {
		Assertion.Fail("output incorrect.");
	    }
	}
    }
}
