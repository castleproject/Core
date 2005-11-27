// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace PetStore.Web.Controllers
{
	using PetStore.Service;


	public class ProductController : BaseSiteController
	{
		private readonly CategoryService categoryService;
		private readonly IProductService productService;

		public ProductController(CategoryService categoryService, IProductService productService)
		{
			this.categoryService = categoryService;
			this.productService = productService;
		}

		public void List()
		{
			AddCategoriesToPropertyBag();

			PropertyBag.Add("products", productService.FindAll());
		}

		public void ListByCategory(int category)
		{
			AddCategoriesToPropertyBag();

			PropertyBag.Add("category", categoryService.Find(category));
			PropertyBag.Add("products", productService.FindByCategory(category));
		}

		public void Details(int productid)
		{
			
		}

		private void AddCategoriesToPropertyBag()
		{
			PropertyBag.Add("categories", categoryService.ObtainCategories());
		}
	}
}
