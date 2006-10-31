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

namespace GettingStartedSample.Models
{
	using System;
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Product : ActiveRecordBase
	{
		private int id;
		private String name;
		private decimal price;
		private Supplier supplier;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public decimal Price
		{
			get { return price; }
			set { price = value; }
		}

		[BelongsTo("SupplierId")]
		public Supplier Supplier
		{
			get { return supplier; }
			set { supplier = value; }
		}
		
		public static Product[] FindAll()
		{
			return (Product[]) FindAll(typeof(Product));
		}

		public static Product FindById(int id)
		{
			return (Product) FindByPrimaryKey(typeof(Product), id);
		}
	}
}
