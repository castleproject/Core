// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace DataBindSample.Controllers
{
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
		private static Category[] FindAllCategories()
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
