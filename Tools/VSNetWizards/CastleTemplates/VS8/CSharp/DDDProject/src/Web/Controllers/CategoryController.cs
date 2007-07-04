namespace !NAMESPACE!.Web.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;

	public class CategoryController : BaseController
	{
		private readonly ICategoryRepository repository;

		public CategoryController(ICategoryRepository repository)
		{
			this.repository = repository;
		}

		[AccessibleThrough(Verb.Post)]
		public void InsertSave(string name)
		{
			try
			{
				Category c = Category.Create(name);
				repository.Create(c);

				PropertyBag["category"] = c;
			}
			catch (Exception e)
			{
				Flash["error"] = e;
			}

			CancelLayout();
		}
	}
}
