namespace Tests.Controllers
{
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;
	using !NAMESPACE!.Core.ProductModule.Services;
	using !NAMESPACE!.Web.Controllers;
	using Rhino.Mocks;

	[TestFixture]
	public class ProductControllerTestCase : BaseControllerTest
	{
		private ProductService productServiceMock;
		private MockRepository mockRepository;
		private ProductController productController;
		private IProductRepository productRepositoryMock;
		private ICategoryRepository categoryRepositoryMock;

		[SetUp]
		public void Init()
		{
			mockRepository = new MockRepository();

			productRepositoryMock = mockRepository.CreateMock<IProductRepository>();
			productServiceMock = mockRepository.CreateMock<ProductService>(productRepositoryMock);

			productController = new ProductController(productServiceMock, 
				productRepositoryMock, categoryRepositoryMock);
			PrepareController(productController);
		}

		[Test]
		public void IndexShouldListTwoExistingProducts()
		{
			Expect.Call(productRepositoryMock.FetchAll()).Return(new Product[]
				{
					Product.Create("Product 1", 10, Category.Create("c1")),
					Product.Create("Product 2", 20, Category.Create("c2"))
				}
			);

			mockRepository.ReplayAll();

			productController.Index();

			Assert.IsNotNull(productController.PropertyBag["products"]);
			Product[] products = (Product[])productController.PropertyBag["products"];
			Assert.AreEqual(2, products.Length);
			
			Assert.AreEqual("Product 1", products[0].Name);
			Assert.AreEqual(10.0, products[0].Price);

			Assert.AreEqual("Product 2", products[1].Name);
			Assert.AreEqual(20.0, products[1].Price);

			mockRepository.VerifyAll();
		}

		[Test]
		public void InsertSave()
		{
			Category c = Category.Create("Category 1");
			c.Id = 1;

			Product product = new Product("Some product", 55, c);
			productServiceMock.Create(product);
			LastCall.Repeat.Once();

			mockRepository.ReplayAll();

			productController.InsertSave(product);

			mockRepository.VerifyAll();
		}

		[Test]
		public void InsertExpectOneCategory()
		{
			Expect.Call(categoryRepositoryMock.FetchAll()).Return(new Category[]
				{
					Category.Create("Category 1")
				}
			);

			mockRepository.ReplayAll();

			productController.Insert();

			Category[] categories = (Category[])productController.PropertyBag["categories"];
			Assert.IsNotNull(categories);
			Assert.AreEqual(1, categories.Length);

			mockRepository.VerifyAll();
		}

		[Test]
		public void DeleteShouldDelegateToServiceAndCallRepositoryFetchById()
		{
			productServiceMock.BatchDelete(new int[] { 1, 2, 3 });
			LastCall.On(productServiceMock).Repeat.Once();

			mockRepository.ReplayAll();

			productController.Delete(new int[] { 1, 2, 3 });

			mockRepository.VerifyAll();

			Assert.IsNull(productController.Flash["error"]);
		}
	}
}
