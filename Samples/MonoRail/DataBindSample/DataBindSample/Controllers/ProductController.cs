namespace DataBindSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;
	using DataBindSample.Model;

	[Layout("default")]
	public class ProductController : SmartDispatcherController
	{
		/// <summary>
		/// Presents the administration menu
		/// </summary>
		public void Index()
		{
			
		}
		
		/// <summary>
		/// Presents a form so the user can enter
		/// the data for a new product
		/// </summary>
		public void New()
		{
			// Available categories to show
			PropertyBag["categories"] = FindAllCategories();
		}
		
		/// <summary>
		/// Creates the product
		/// </summary>
		/// <param name="prod"></param>
		[AccessibleThrough(Verb.Post)]
		public void Create([DataBind("product")] Product prod)
		{
			// Pretend to create it 
			
			// working working working...
			
			// Put the product on the property bag so we can 
			// present the data on the confirmation 
			
			PropertyBag["action_executed"] = "Product Created";
			PropertyBag["product"] = prod;
			
			RenderView("confirmation");
		}
		
		/// <summary>
		/// Presents a form so the user can edit the product
		/// </summary>
		/// <param name="id">Product id</param>
		public void Edit(int id)
		{
			// Available categories to show
			PropertyBag["categories"] = FindAllCategories();

			// Pretend to load the Product from the database
			
			Product prodLoaded = new Product(id, "Freezer", true);
			
			PropertyBag["product"] = prodLoaded;
		}

		/// <summary>
		/// Updates the product on the fake database
		/// </summary>
		/// <param name="prod"></param>
		[AccessibleThrough(Verb.Post)]
		public void Update([DataBind("product")] Product prod)
		{
			// Pretend to update it 
			
			// working working working...
			
			// Put the product on the property bag so we can 
			// present the data on the confirmation 
			
			PropertyBag["action_executed"] = "Product Updated";
			PropertyBag["product"] = prod;
			
			RenderView("confirmation");
		}

		/// <summary>
		/// In a real application we'd be loading
		/// this from the database
		/// </summary>
		private Category[] FindAllCategories()
		{
			return new Category[]
				{
					new Category("Kitchen stuff"), 
					new Category("Living-room stuff"), 
					new Category("Bedroom stuff")
				};
		}
	}
}
