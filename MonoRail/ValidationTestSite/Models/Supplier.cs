namespace ValidationTestSite.Models
{
	using System.Collections;
	using System.Collections.Generic;

	public class Supplier
	{
		private int id;
		private ArrayList products;
		private List<Product> products2;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public ArrayList Products
		{
			get { return products; }
			set { products = value; }
		}

		public List<Product> Products2
		{
			get { return products2; }
			set { products2 = value; }
		}
	}
}
