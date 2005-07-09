namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;

	/// <summary>
	/// Tests to make sure that the VelocityContext is functioning correctly
	/// </summary>
	[TestFixture]
	public class ContextTest
	{
		[Test]
		public void CaseInsensitive()
		{
			// normal case sensitive context
			VelocityContext c = new VelocityContext();
			c.Put("firstName", "Cort");
			c.Put("LastName", "Schaefer");

			// verify the output, $lastName should not be resolved
			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = ve.Evaluate(c, sw, "ContextTest.CaseInsensitive", "Hello $firstName $lastName");
			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("Hello Cort $lastName", sw.ToString());

			// create a context based on a case insensitive hashtable
			Hashtable ht = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());
			ht.Add("firstName", "Cort");
			ht.Add("LastName", "Schaefer");
			c = new VelocityContext(ht);

			// verify the output, $lastName should be resolved
			sw = new StringWriter();
			ok = ve.Evaluate(c, sw, "ContextTest.CaseInsensitive", "Hello $firstName $lastName");
			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("Hello Cort Schaefer", sw.ToString());

			// create a context based on a case insensitive hashtable, verify that stuff added to the context after it is created if found case insensitive
			ht = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());
			ht.Add("firstName", "Cort");
			c = new VelocityContext(ht);
			c.Put("LastName", "Schaefer");

			// verify the output, $lastName should be resolved
			sw = new StringWriter();
			ok = ve.Evaluate(c, sw, "ContextTest.CaseInsensitive", "Hello $firstName $lastName");
			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("Hello Cort Schaefer", sw.ToString());
		}
	}
}