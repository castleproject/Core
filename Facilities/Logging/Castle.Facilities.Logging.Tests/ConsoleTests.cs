namespace Castle.Facilities.Logging.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    /// <summary>
	/// Summary description for ConsoleTests.
	/// </summary>
	[TestFixture]
	public class ConsoleTests
	{
		public ConsoleTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        [Test] public void CaptureConsoleOutTest()
        {
            StringWriter test = new StringWriter();
            Console.SetOut(test);
            Console.Write("test");

            string output = test.GetStringBuilder().ToString();
            Assert.AreEqual(output, "test");
        }
	}
}
