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

namespace Castle.Facilities.ActiveRecord.Tests.Model1
{
	using Castle.Facilities.ActiveRecord;


	[ActiveRecord]
	public class OrderItem : ActiveRecordBase
	{
		private int _id;
		private int _quantity;
		private Order _parent;
		private Product _product;

		public OrderItem()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int Quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}

		[BelongsTo( typeof(Order) )]
		public Order Order
		{
			get { return _parent; }
			set { _parent = value; }
		}

		[HasOne( typeof(Product) )]
		public Product Product
		{
			get { return _product; }
			set { _product = value; }
		}
	}
}
