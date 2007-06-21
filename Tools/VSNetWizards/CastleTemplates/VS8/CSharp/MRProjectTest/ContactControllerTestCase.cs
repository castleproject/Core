namespace !NAMESPACE!.Tests
{
	using NUnit.Framework;
	using Castle.MonoRail.TestSupport;
	using !NAMESPACE!.Controllers;

	[TestFixture]
	public class ContactControllerTestCase : AbstractMRTestCase
	{
		public void AboutUsAction()
		{
			ContactController controller = new ContactController();
			controller.aboutUs();
			
			// Now you can do other assertions
		}
	}
}
