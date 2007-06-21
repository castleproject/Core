namespace !NAMESPACE!.Tests
{
	using NUnit.Framework;
	using Castle.MonoRail.TestSupport;
	using !NAMESPACE!.Controllers;

	/// <summary>
	/// More about testing controllers can be found at 
	/// http://using.castleproject.org/display/MR/TDDingControllers
	/// http://using.castleproject.org/display/MR/TDDing+WizardSteps
	/// If you want to test filters, see 
	/// http://using.castleproject.org/display/MR/Unit+Testing+Filters
	/// </summary>
	[TestFixture]
	public class ContactControllerTestCase : BaseControllerTest
	{
		private ContactController controller;

		[SetUp]
		public void Init()
		{
			controller = new ContactController();
			PrepareController(controller);
		}

		[Test]
		public void IndexShouldMakeTheCountryListAvailableToTheView()
		{
			controller.Index();

			Assert.IsNotNull(controller.PropertyBag["countries"]);
		}
	}
}
