namespace !NAMESPACE!.Core.ProductModule.Services
{
	using Castle.Services.Transaction;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;

	[Transactional]
	public class CategoryService
	{
		private readonly ICategoryRepository repository;

		public CategoryService(ICategoryRepository repository)
		{
			this.repository = repository;
		}

		public virtual void Create(Category category)
		{
			repository.Create(category);
		}

		[Transaction]
		public virtual void Update(Category category)
		{
			repository.Update(category);
		}

		[Transaction]
		public virtual void Delete(Category category)
		{
			repository.Delete(category);
		}
	}
}
