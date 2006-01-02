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

namespace PetStore.Model
{
	using System;

	using Castle.ActiveRecord;

	using Iesi.Collections;


	[ActiveRecord]
	public class Order : ActiveRecordBase
	{
		private int id;
		private decimal total;
		private DateTime creation;
		private Customer customer;
		private ISet items = new HashedSet();

		public Order()
		{
			creation = DateTime.Now;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public DateTime Creation
		{
			get { return creation; }
			set { creation = value; }
		}

		[Property]
		public decimal Total
		{
			get { return total; }
			set { total = value; }
		}

		[BelongsTo("customer_id")]
		public Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}

		[HasMany( typeof(OrderItem) )]
		public ISet Items
		{
			get { return items; }
			set { items = value; }
		}
	}
}
