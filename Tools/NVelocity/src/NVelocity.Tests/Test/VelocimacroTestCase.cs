namespace NVelocity.Test
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;
	using NVelocity.Runtime;

	/// <summary>
	/// This class tests strange Velocimacro issues.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	[TestFixture]
	public class VelocimacroTestCase
	{
		private String template1 = "#macro(fooz $a)$a#end#macro(bar $b)#fooz($b)#end#foreach($i in [1..3])#bar($i)#end";
		private String result1 = "123";

		public VelocimacroTestCase()
		{
			try
			{
				Velocity.SetProperty(RuntimeConstants.VM_PERM_INLINE_LOCAL, true);
				Velocity.Init();
			}
			catch (Exception)
			{
				throw new Exception("Cannot setup VelocimacroTestCase!");
			}
		}

		/// <summary>
		/// Runs the test.
		/// </summary>
		[Test]
		public virtual void Test_Run()
		{
			VelocityContext context = new VelocityContext();

			StringWriter writer = new StringWriter();
			Velocity.Evaluate(context, writer, "vm_chain1", template1);

			String out_Renamed = writer.ToString();

			Assert.AreEqual(result1, out_Renamed);
		}
	}
}