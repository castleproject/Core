namespace !NAMESPACE!.Core.ProductModule.Services
{
	using Castle.Services.Transaction;
	using !NAMESPACE!.Core.ProductModule;
	using !NAMESPACE!.Core.ProductModule.Repositories;

	[Transactional]
	public class ProductService
	{
		private readonly IProductRepository productRepository;

		public ProductService(IProductRepository productRepository)
		{
			this.productRepository = productRepository;
		}

		public virtual void Create(Product product)
		{
			productRepository.Create(product);
		}

		[Transaction]
		public virtual void Update(Product product)
		{
			productRepository.Update(product);
		}

		[Transaction]
		public virtual void Delete(Product product)
		{
			productRepository.Delete(product);
		}

		[Transaction]
		public virtual void BatchDelete(int[] productIds)
		{
			foreach (int productId in productIds)
			{
				Product product = productRepository.FetchById(productId);
				Delete(product);
			}
		}
	}
}
