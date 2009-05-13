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

namespace MoreComplexSample
{
	using System;
	using Castle.ActiveRecord;
	using Iesi.Collections.Generic;

	public enum OrderStatus : short
	{
		Created,
		Cancelled,
		Processed,
		Dispatched
	}
	
	[ActiveRecord("`Order`")]
	public class Order : ActiveRecordBase
	{
		private int id;
		private DateTime createdAt;
		private DateTime? dispatchedAt;
		private Customer customer;
		private OrderStatus status;
		private float total;
		private ISet<Product> products = new HashedSet<Product>();

		public Order()
		{
			createdAt = DateTime.Now;
		}

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(Update=false)]
		public DateTime CreatedAt
		{
			get { return createdAt; }
			set { createdAt = value; }
		}

		[Property(Insert=false)]
		public DateTime? DispatchedAt
		{
			get { return dispatchedAt; }
			set { dispatchedAt = value; }
		}

		[BelongsTo("CustomerId")]
		public Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}

		[Property]
		public float Total
		{
			get { return total; }
			set { total = value; }
		}

		[Property]
		public OrderStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		[HasAndBelongsToMany(
			Table="LineItem",
			ColumnKey="OrderId", ColumnRef="ProductId", Inverse = true, Lazy = true)]
		public ISet<Product> Products
		{
			get { return products; }
			set { products = value; }
		}
	}
}
