// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace MoreComplexSample
{
	using System;
	using Castle.ActiveRecord;
	using NHibernate.Criterion;

	[ActiveRecord("LineItem")]
	public class LineItem : ActiveRecordBase<LineItem>
	{
		private Guid id;
		private Order order;
		private Product product;
		private int? quantity;

		public LineItem()
		{
		}

		public LineItem(Order order, Product product)
		{
			this.order = order;
			this.product = product;
		}

		[PrimaryKey(PrimaryKeyType.Guid)]
		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo("OrderId", NotNull = true, UniqueKey = "ConstraintName")]
		public Order Order
		{
			get { return order; }
			set { order = value; }
		}

		[BelongsTo("ProductId", NotNull = true, UniqueKey = "ConstraintName")]
		public Product Product
		{
			get { return product; }
			set { product = value; }
		}

		[Property]
		public int? Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public static LineItem Find(Order order, Product product)
		{
			return FindOne(Expression.Eq("Order", order), Expression.Eq("Product", product));
		}
	}
}
