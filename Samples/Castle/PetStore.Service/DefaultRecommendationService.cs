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

namespace PetStore.Service
{
	using System;

	using PetStore.Model;

	/// <summary>
	/// A clever recommendation service should use some 
	/// branch of AI to suggest meaningfull products 
	/// (think adaptive resonance theory / http://jroller.com/page/hammett?entry=finally_something_interesting_on_my)
	/// based on the history of the customer (if available).
	/// But we just pick up some random products
	/// </summary>
	public class DefaultRecommendationService : IRecommendationService
	{
		private readonly Random random;
		private readonly IProductDataAccess productDataAccess;

		public DefaultRecommendationService(IProductDataAccess productDataAccess)
		{
			this.productDataAccess = productDataAccess;
			this.random = new Random((int) DateTime.Now.Ticks);
		}

		public Product[] GetProducts(Customer customer)
		{
			int[] ids = productDataAccess.GetProductIds();

			if (ids.Length == 0) return new Product[0];

			// We select two
			
			int firstIndex = random.Next(0, ids.Length - 1);
			int secondIndex = random.Next(0, ids.Length - 1);

			if (firstIndex != secondIndex)
			{
				return new Product[] { 
										 Product.Find(ids[firstIndex]), 
										 Product.Find(ids[secondIndex]) };
			}
			else
			{
				return new Product[] { Product.Find(ids[firstIndex]) };
			}
		}
	}
}
