//using Spring2.Core.Types;
//using VelocityNET.App;
//using VelocityNET.Context;

namespace NVelocity.Test
{
	using System;
	using System.IO;
	using App;
	using NUnit.Framework;

	/// <summary>
	/// Test Velocity processing
	/// </summary>
	[TestFixture]
	public class VTLTest
	{
		public class A
		{
			public String T1
			{
				get { return "0"; }
			}
		}

		[Test]
		[Ignore("Known issues from version 1.3x and will be resolved with next parser port")]
		public void VTLTest1()
		{
//	    VelocityCharStream vcs = new VelocityCharStream(new StringReader(":=t#${A.T1}ms"), 1, 1);
//	    Parser p = new Parser(vcs);
//	    SimpleNode root = p.process();
//
//	    String nodes = String.Empty;
//	    if (root != null) {
//		Token t = root.FirstToken;
//		nodes += t.kind.ToString();
//		while (t != root.LastToken) {
//		    t = t.next;
//		    nodes += "," + t.kind.ToString();
//		}
//	    }
//
//	    throw new System.Exception(nodes);

			VelocityEngine velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			VelocityContext c = new VelocityContext();
			c.Put("A", new A());

			// modified version so Bernhard could continue
			StringWriter sw = new StringWriter();
			Boolean ok = velocityEngine.Evaluate(c, sw, "VTLTest1", "#set($hash = \"#\"):=t${hash}${A.T1}ms");
			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(":=t#0ms", sw.ToString());

			// the actual problem reported
			sw = new StringWriter();
			ok = velocityEngine.Evaluate(c, sw, "VTLTest1", ":=t#${A.T1}ms");
			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(":=t#0ms", sw.ToString());
		}
	}
}