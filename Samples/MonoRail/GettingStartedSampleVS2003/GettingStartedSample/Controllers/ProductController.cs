// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace GettingStartedSample.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using GettingStartedSample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class ProductController : SmartDispatcherController
	{
		public void List()
		{
			PropertyBag["products"] = Product.FindAll();
		}
		
		public void New()
		{
			PropertyBag["suppliers"] = Supplier.FindAll();
		}
		
		public void Create([DataBind("product")] Product prod)
		{
			try
			{
				prod.Create();
			
				RedirectToAction("list");
			}
			catch(Exception ex)
			{
				Flash["error"] = ex.Message;
				Flash["product"] = prod;
				
				RedirectToAction("new");
			}
		}
		
		public void Edit(int id)
		{
			PropertyBag["product"] = Product.FindById(id);
			PropertyBag["suppliers"] = Supplier.FindAll();
		}

		public void Edit(int id, [FlashBinder] Product product)
		{
			PropertyBag["suppliers"] = Supplier.FindAll();
		}

		public void Update([DataBind("product")] Product prod)
		{
			try
			{
				prod.Update();
			
				RedirectToAction("list");
			}
			catch(Exception ex)
			{
				Flash["error"] = ex.Message;
				Flash["product"] = prod;
				
				RedirectToAction("edit", "id=" + prod.Id);
			}
		}
		
		public void Delete(int id)
		{
			Product product = Product.FindById(id);
			
			product.Delete();
			
			RedirectToAction("list");
		}
	}
}
