namespace !NAMESPACE!.Web.Controllers
{
	using System;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;
	using !NAMESPACE!.Core.ProductModule.Services;

	public class ProductController : BaseController
	{
		private readonly ProductService productService;
		private readonly IProductRepository productRepository;
		private readonly ICategoryRepository categoryRepository;

		public ProductController(ProductService productService, IProductRepository productRepository,
			ICategoryRepository categoryRepository)
		{
			this.productService = productService;
			this.productRepository = productRepository;
			this.categoryRepository = categoryRepository;
		}

		public void Index()
		{
			PropertyBag["products"] = productRepository.FetchAll();
		}

		public void Insert()
		{
			AddCategoriesToPropertyBag();
			RenderView("ProductForm");
		}

		[AccessibleThrough(Verb.Post)]
		public void InsertSave([DataBind("product")] Product product)
		{
			productService.Create(product);
			RedirectToAction("Index");
		}

		private void AddCategoriesToPropertyBag()
		{
			PropertyBag["categories"] = categoryRepository.FetchAll();
		}

		public void Edit([ARFetch("id")] Product product)
		{
			PropertyBag["product"] = product;
			AddCategoriesToPropertyBag();
			RenderView("ProductForm");
		}

		[AccessibleThrough(Verb.Post)]
		public void EditSave([DataBind("product")] Product product)
		{
			productRepository.Update(product);
			RedirectToAction("index");
		}

		[AccessibleThrough(Verb.Post)]
		public void Delete(int[] product)
		{
			try
			{
				productService.BatchDelete(product);
			}
			catch (Exception e)
			{
				Flash["error"] = e;
			}

			RedirectToAction("Index");
		}
	}
}
