using System;
using System.Reflection;

using NUnit.Framework;

using Commons.Collections;
using NVelocity.Util.Introspection;
using NVelocity.Runtime;

namespace NVelocity.Test {
    /// <summary>
    /// Test Velocity Introspector
    /// </summary>
    [TestFixture]
    public class IntrospectorTest {

	[Test]
	public void Test_Evaluate() {
	    RuntimeServices rs = RuntimeSingleton.RuntimeServices;
	    Introspector i = new Introspector(rs);
	    MethodInfo mi = i.getMethod(typeof(VelocityTest), "Test_Evaluate", null);
	    Assertion.AssertNotNull("Expected to find VelocityTest.Test_Evaluate", mi);
	    Assertion.Assert("method not found", mi.ToString().Equals("Void Test_Evaluate()"));

	    mi = i.getMethod(typeof(ExtendedProperties), "GetString", new Object[] { "parm1", "parm2" });
	    Assertion.AssertNotNull("Expected to find ExtendedProperties.GetString(String, String)", mi);
	    Assertion.Assert("method not found", mi.ToString().Equals("System.String GetString(System.String, System.String)"));
	}


    }
}
