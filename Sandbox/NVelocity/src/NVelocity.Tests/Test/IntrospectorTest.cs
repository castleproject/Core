using Commons.Collections;

namespace NVelocity.Test
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using NVelocity.Runtime;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// Test Velocity Introspector
	/// </summary>
	[TestFixture]
	public class IntrospectorTest
	{
		[Test]
		public void Test_Evaluate()
		{
			RuntimeServices rs = RuntimeSingleton.RuntimeServices;
			Introspector i = new Introspector(rs);
			MethodInfo mi = i.GetMethod(typeof(VelocityTest), "Test_Evaluate", null);
			Assert.IsNotNull(mi, "Expected to find VelocityTest.Test_Evaluate");
			Assert.IsTrue(mi.ToString().Equals("Void Test_Evaluate()"), "method not found");

			mi = i.GetMethod(typeof(ExtendedProperties), "GetString", new Object[] {"parm1", "parm2"});
			Assert.IsNotNull(mi, "Expected to find ExtendedProperties.GetString(String, String)");
			Assert.IsTrue(mi.ToString().Equals("System.String GetString(System.String, System.String)"), "method not found");
		}


	}
}