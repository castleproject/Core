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

namespace PetStore.Service
{
	using System;

	using PetStore.Model;

	/// <summary>
	/// This service doesn't add much, as a matter of fact
	/// we're being extremely purists here and 
	/// the service just sits between the presentation layer
	/// and the data access layer. We, for this project, don't
	/// want the presentation layer using the data access layer 
	/// directly. But, as I said, we're just being purists.
	/// </summary>
	public class DefaultProductService : IProductService
	{
		private readonly IProductDataAccess productDataAccess;

		public DefaultProductService(IProductDataAccess productDataAccess)
		{
			this.productDataAccess = productDataAccess;
		}

		public Product Find(int productId)
		{
			return productDataAccess.Find(productId);
		}

		public Product[] FindAll()
		{
			return productDataAccess.FindAll();
		}

		public Product[] FindByCategory(int categoryId)
		{
			return productDataAccess.FindByCategory(categoryId);
		}
	}
}
