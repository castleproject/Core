namespace !NAMESPACE!.Tests
{
	using System;

	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;

	[TestFixture]
	public class HomeControllerTestCase : AbstractMRTestCase
	{
		[Test]
		public void IndexAction()
		{
			DoGet("home/index.rails");

			// Use the assert family of methods available in the base class
			// for example:
			// AssertReplyContains("something");
			// AssertReplyStartsWith("something");
		}
	}
}
