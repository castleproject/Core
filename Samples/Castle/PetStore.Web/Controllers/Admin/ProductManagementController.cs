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

namespace PetStore.Web.Controllers.Admin
{
	using System;
	using System.IO;
	using System.Web;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using PetStore.Model;

	/// <summary>
	/// This controller illustrates a CRUD without 
	/// Scaffolding. Note that this controller does
	/// not use the service layer (only for the brevity's sake)
	/// </summary>
	[Layout("admin")]
	public class ProductManagementController : AbstractSecureController
	{
		private String productImagesDir;

		/// <summary>
		/// Guess who is going to set this property?
		/// Yeah, this is inversion of control, baby.
		/// </summary>
		public string ProductImagesDir
		{
			get { return productImagesDir; }
			set { productImagesDir = value; }
		}

		public void List()
		{
			Product[] products = Product.FindAll();
			
			PropertyBag.Add( "list", PaginationHelper.CreatePagination(products, 10) );
		}

		public void New()
		{
			// This is one approach just to reuse the same form
			// on the edit action and to preserve the inputs 
			// values
			PropertyBag.Add( "product", new Product() );

			// To populate the select html control
			PropertyBag.Add( "categories", Category.FindAll() );

			// Only necessary as we might call this action directly (see below)
			// from other action
			RenderView("New");
		}

		public void Create([DataBind(Prefix="product")] Product product, HttpPostedFile picture)
		{
			if (picture.ContentLength == 0)
			{
				Flash["error"] = "You must attach a picture to the product";
				New(); 
				return;
			}

			String filename = String.Format( "{0}{1}", 
				Guid.NewGuid().ToString("N"), Path.GetExtension(picture.FileName) );

			picture.SaveAs( Path.Combine(GetPictureCompleteDir(), filename) );

			product.PictureFile = filename;

			product.Save();

			Redirect("ProductManagement", "list");
		}

		private String GetPictureCompleteDir()
		{
			return Path.Combine(Context.ApplicationPhysicalPath, ProductImagesDir);
		}
	}
}
