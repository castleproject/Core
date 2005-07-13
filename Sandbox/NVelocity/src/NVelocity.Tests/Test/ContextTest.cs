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
		public void ParamArraySupport1()
		{
			VelocityContext c = new VelocityContext();
			c.Put("x", new Something() );
			
			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Print( \"aaa\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("aaa", sw.ToString());

			sw = new StringWriter();

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents()");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("", sw.ToString());

			sw = new StringWriter();

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents( \"x\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("x", sw.ToString());

			sw = new StringWriter();

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents( \"x\", \"y\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("x,y", sw.ToString());
		}

		[Test]
		public void ParamArraySupport2()
		{
			VelocityContext c = new VelocityContext();
			c.Put("x", new Something2() );
			
			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents( \"hammett\", 1 )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("hammett1", sw.ToString());

			sw = new StringWriter();

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents( \"hammett\", 1, \"x\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("hammett1x", sw.ToString());

			sw = new StringWriter();

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.Contents( \"hammett\", 1, \"x\", \"y\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("hammett1x,y", sw.ToString());
		}

		[Test]
		public void Hashtable1()
		{
			VelocityContext c = new VelocityContext();

			Hashtable x = new Hashtable();
			x.Add( "item", "value1" );

			c.Put( "x", x );
			
			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$x.get_Item( \"item\" )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("value1", sw.ToString());
		}

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

	public class Something
	{
		public String Print( String arg )
		{
			return arg;
		}

		public String Contents( params String[] args )
		{
			return String.Join( ",", args );
		}
	}

	public class Something2
	{
		public String Contents( String name, int age, params String[] args )
		{
			return name + age + String.Join( ",", args );
		}
	}
}