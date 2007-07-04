namespace !NAMESPACE!.Core.ProductModule
{
	using System;
	using !NAMESPACE!.Core.Infraestructure;
	using Castle.ActiveRecord;

	[ActiveRecord("Product")]
	public class Product : IIdentifiable
	{
		private int id;
		private string name;
		private double price;
		private Category category;

		public Product()
		{
		}

		public Product(string name, double price, Category category)
		{
			this.name = name;
			this.price = price;
			this.category = category;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public double Price
		{
			get { return price; }
			set { price = value; }
		}

		[BelongsTo("categoryid")]
		public Category Category
		{
			get { return category; }
			set { category = value; }
		}

		public static Product Create(string name, double price, Category category)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			if (category == null)
			{
				throw new ArgumentNullException("category");
			}

			if (price <= 0)
			{
				throw new ArgumentException("Price cannot be zero or negative");
			}

			return new Product(name, price, category);
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			Product product = obj as Product;
			if (product == null) return false;
			return id == product.id;
		}

		public override int GetHashCode()
		{
			return id;
		}
	}
}
