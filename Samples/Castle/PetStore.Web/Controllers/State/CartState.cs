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

namespace PetStore.Web.Controllers.State
{
	using System;
	using System.Collections;

	/// <summary>
	/// Session state pattern to hold the 
	/// cart data.
	/// </summary>
	[Serializable]
	public class CartState : IEnumerable
	{
		private IList items = new ArrayList();

		public void Add(int quantity, int productId)
		{
			items.Add(new CartItem(quantity, productId));
		}

		public void Remove(CartItem item)
		{
			items.Remove(item);
		}

		public IList Items
		{
			get { return items; }
		}

		public class CartItem
		{
			private readonly int quantity;
			private readonly int productId;

			public CartItem(int quantity, int productId)
			{
				this.quantity = quantity;
				this.productId = productId;
			}

			public int Quantity
			{
				get { return quantity; }
			}

			public int ProductId
			{
				get { return productId; }
			}
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}

		#endregion
	}
}
