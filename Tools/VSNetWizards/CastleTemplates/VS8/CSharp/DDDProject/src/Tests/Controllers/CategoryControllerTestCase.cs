namespace !NAMESPACE!.Tests.Controllers
{
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;
	using !NAMESPACE!.Web.Controllers;
	using Rhino.Mocks;

	[TestFixture]
	public class CategoryControllerTestCase : BaseControllerTest
	{
		private MockRepository mockRepository;
		private ICategoryRepository repositoryMock;
		private CategoryController controller;

		[SetUp]
		public void Init()
		{
			mockRepository = new MockRepository();
			repositoryMock = mockRepository.CreateMock<ICategoryRepository>();
			controller = new CategoryController(repositoryMock);
			PrepareController(controller);
		}

		[Test]
		public void InsertSaveExpectANewAndalidCategory()
		{
			repositoryMock.Create(Category.Create("Category 1"));
			mockRepository.ReplayAll();

			controller.InsertSave("Category 1");

			Assert.IsNotNull(controller.PropertyBag["category"]);

			mockRepository.VerifyAll();
		}

		[Test]
		public void InsertSaveExpectError()
		{
			controller.InsertSave(null);

			Assert.IsNull(controller.PropertyBag["category"]);
			Assert.IsNotNull(controller.Flash["error"]);
		}
	}
}
